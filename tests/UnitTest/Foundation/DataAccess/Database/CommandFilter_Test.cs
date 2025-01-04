using System;
using System.Linq.Expressions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.ServiceNode.HealthCheck.Contracts;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.DataAccess.Database
{
    public class CommandFilter_Test : TestBase
    {
        public CommandFilter_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Expression_Test()
        {
            var request = new HlckEcho_Request()
            {
                Data = new HlckEcho_RequestModel
                {
                    RequestString = "exception",
                    PageIndex = 1,
                    PageSize = 1000,
                },
            };

            {
                var compiledFilter = FilterException().Compile();
                Assert.True(compiledFilter(request));
            }

            {
                var compiledFilter = FilterPaging().Compile();
                Assert.True(compiledFilter(request));
            }

            {
                request.Data.PageSize = int.MaxValue;
                var compiledFilter = FilterPaging().Compile();
                Assert.False(compiledFilter(request));
            }
        }

        public Expression<Func<HlckEcho_Request, bool>> FilterException()
        {
            Expression<Func<HlckEcho_Request, bool>> filter = o =>
                o.Data != null &&
                o.Data.RequestString != null &&
                o.Data.RequestString.Contains("exception");

            return filter;
        }

        public Expression<Func<HlckEcho_Request, bool>> FilterPaging()
        {
            Expression<Func<HlckEcho_Request, bool>> filter = o =>
                o.Data != null &&
                o.Data.PageIndex >= ConfigConst.MinPageIndex &&
                o.Data.PageSize >= ConfigConst.MinPageSize &&
                o.Data.PageSize <= ConfigConst.MaxPageSize;

            return filter;
        }
    }
}
