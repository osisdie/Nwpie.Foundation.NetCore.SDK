using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Nwpie.Foundation.Abstractions.Utilities
{
    public static class NetworkUtils
    {
        public static string GetLocalIPAddress()
        {
            if (null == m_IP)
            {
                try
                {
                    var firstUpInterface = NetworkInterface.GetAllNetworkInterfaces()
                        .OrderByDescending(c => c.Speed)
                        .FirstOrDefault(c =>
                            c.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                            c.OperationalStatus == OperationalStatus.Up
                        );

                    if (null != firstUpInterface)
                    {
                        var props = firstUpInterface.GetIPProperties();
                        m_IP = props.UnicastAddresses
                            .Where(c => c.Address.AddressFamily == AddressFamily.InterNetwork)
                            .Select(c => c.Address)
                            .FirstOrDefault()?.ToString();
                    }

                    if (null == m_IP || AnyIP == m_IP)
                    {
                        // Might exception on linux
                        var host = Dns.GetHostEntry(Dns.GetHostName());
                        foreach (var ip in host.AddressList)
                        {
                            if (AddressFamily.InterNetwork == ip.AddressFamily)
                            {
                                m_IP = ip.ToString();
                                break;
                            }
                        }
                    }
                }
                catch { }
            }

            if (null == m_IP)
            {
                m_IP = AnyIP;
            }

            return m_IP;
        }

        public const string AnyIP = "::1";

        static string m_IP = null;
        public static string IP
        {
            get
            {
                if (null != m_IP)
                {
                    return m_IP;
                }

                m_IP = GetLocalIPAddress();
                return m_IP;
            }
        }
    }
}
