using System;
using System.Collections.Generic;
using System.Linq;
using Nwpie.Foundation.Common.Extras;
using Consul;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.ServiceNode
{
    public class Consul_Test : TestBase
    {
        public Consul_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test consul service")]
        public void ServiceDicovery_Test()
        {
            var serverUrls = new List<Uri>();
            var consulClient = ComponentMgr.Instance.TryResolve<IConsulClient>();
            Assert.NotNull(consulClient);

            var services = consulClient?.Agent?.Services()?.Result?.Response
                ?.Where(service => service.Value.Tags.Any(t => t == "foundation"))
                ?.ToList();
            Assert.NotEmpty(services);
            Assert.NotNull(services.First().Key);
            Assert.NotNull(services.First().Value);
            Assert.NotNull(services.First().Value.Address);
            Assert.True(services.First().Value.Port > 0);
        }
    }
}
