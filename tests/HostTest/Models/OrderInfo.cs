using System;
using System.Collections.Generic;
using System.Linq;
using Nwpie.HostTest.Data;

namespace Nwpie.HostTest.Models
{
    public interface ICustomerInfo
    {
        int Custid { get; set; }
        string City { get; set; }
        string Region { get; set; }
        //ICollection<OrderInfo> OrderInfo { get; set; }
    }

    public class CustomerInfo : ICustomerInfo
    {
        public CustomerInfo()
        {
            OrderInfo = new HashSet<OrderInfo>();
        }

        public int Custid { get; set; }
        public string City { get; set; }
        public string Region { get; set; }

        public virtual ICollection<OrderInfo> OrderInfo { get; set; }
    }

    public interface IOrderInfo
    {
        int OrderId { get; set; }
        DateTime OrderDate { get; set; }
        //ICollection<OrdDetails> OrderDetails { get; set; }
    }

    public class OrderInfo : IOrderInfo
    {
        public OrderInfo()
        {
            OrderDetails = new HashSet<OrdDetails>();
        }

        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public ICollection<OrdDetails> OrderDetails { get; set; }
    }

    public interface IOrdDetails
    {
        int Orderid { get; set; }
        int Productid { get; set; }
        decimal Unitprice { get; set; }
        short Qty { get; set; }
        decimal Discount { get; set; }
        //OrderInfo Order { get; set; }
    }

    public class OrdDetails : IOrdDetails
    {
        public int Orderid { get; set; }
        public int Productid { get; set; }
        public decimal Unitprice { get; set; }
        public short Qty { get; set; }
        public decimal Discount { get; set; }

        public virtual OrderInfo Order { get; set; }
    }

    public class CustomerOrderDetail_Entity :
        ICustomerInfo,
        IOrderInfo,
        IOrdDetails
    {
        public int Custid { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int Orderid { get; set; }
        public int Productid { get; set; }
        public decimal Unitprice { get; set; }
        public short Qty { get; set; }
        public decimal Discount { get; set; }
    }

    public static class CustomerOrderDetail_Entity_Extension
    {
        public static CustomerInfo ToFirstCustomerInfo(this IEnumerable<CustomerOrderDetail_Entity> entities)
        {
            return entities?.ToCustomerInfo()?.FirstOrDefault();
        }

        public static CustomerInfo ToCustomerInfo(this IEnumerable<CustomerOrderDetail_Entity> entities, int custId)
        {
            var customerInfo = DBHelper.CachedCustomerInfo(custId);
            var orders = entities
                ?.Where(o => o.Custid == custId)
                ?.GroupBy(o => o.Custid)
                ?.SelectMany(group => group);
            if (orders?.Count() > 0 != true)
            {
                return customerInfo;
            }

            customerInfo.OrderInfo = new List<OrderInfo>();

            // fill orders
            foreach (var order in orders)
            {
                var orderId = order.OrderId;
                if (orderId <= 0)
                {
                    continue;
                }

                var orderInfo = DBHelper.CachedOrderInfo(orderId);
                customerInfo.OrderInfo.Add(orderInfo);

                // fill order details
                var orderDetails = DBHelper.CachedOrderDetails(orderId);
                if (orderDetails?.Count() > 0 != true)
                {
                    continue;
                }

                orderInfo.OrderDetails = orderDetails.LinkOrder(orderInfo).ToList();
            }

            return customerInfo;
        }

        public static IEnumerable<CustomerInfo> ToCustomerInfo(this IEnumerable<CustomerOrderDetail_Entity> entities)
        {
            var customers = entities.GroupBy(o => o.Custid);
            if (customers?.Count() > 0 != true)
            {
                return null;
            }

            var orders = entities.GroupBy(o => new { o.Custid, o.OrderId });
            var customerlist = new List<CustomerInfo>();
            foreach (var customer in customers)
            {
                var custId = customer.Key;
                var customerInfo = DBHelper.CachedCustomerInfo(custId);

                var myOrders = orders.Where(o => o.Key.Custid == custId);
                if (myOrders?.Count() > 0)
                {
                    customerInfo.OrderInfo = new List<OrderInfo>();
                    foreach (var order in myOrders)
                    {
                        var orderId = order.Key.OrderId;
                        var orderInfo = DBHelper.CachedOrderInfo(orderId);
                        orderInfo.OrderDetails = DBHelper.CachedOrderDetails(orderId)?.ToList();
                        orderInfo.OrderDetails.LinkOrder(orderInfo);

                        customerInfo.OrderInfo.Add(orderInfo);
                    }
                }

                customerlist.Add(customerInfo);
            }

            return customerlist;
        }

        public static IEnumerable<OrdDetails> LinkOrder(this IEnumerable<OrdDetails> details, OrderInfo order)
        {
            if (details?.Count() > 0 != true)
            {
                return details;
            }

            foreach (var detail in details)
            {
                detail.Order = order;
            }

            return details;
        }
    }
}
