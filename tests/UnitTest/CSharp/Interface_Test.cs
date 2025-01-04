using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nwpie.Foundation.ServiceNode.HealthCheck.Interfaces;
using Nwpie.Foundation.ServiceNode.HealthCheck.Services;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.MiniSite.Storage.ServiceCore.Files.Download;
using Nwpie.Foundation.Auth.SDK.Extensions;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Common.Extras;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.CSharp
{
    public class Interface_Test : TestBase
    {
        public Interface_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Casting_Test()
        {
            var tokenService = ComponentMgr.Instance.GetDefaultJwtTokenService();
            Assert.True(tokenService is ITokenService);
            Assert.NotNull(tokenService as ITokenService);
            Assert.True(tokenService is IJwtAuthService);
            if (tokenService is IJwtAuthService svcJwt)
            {
                Assert.NotNull(svcJwt);
            }

            Assert.False(tokenService is IAesAuthService);
            if (!(tokenService is IAesAuthService _))
            {
                Assert.Null(null);
            }
            Assert.Null(tokenService as IAesAuthService);
            Assert.Null(null as IJwtAuthService);
            Assert.Null(null as IAesAuthService);
        }

        [Fact]
        public void IsAssignable_Test()
        {
            Assert.True(typeof(IRepository).IsAssignableFrom(typeof(EmptyRepository)));
            Assert.True(typeof(IRepository).IsAssignableFrom(typeof(RepositoryBase)));
            Assert.True(typeof(IRepository).IsAssignableFrom(typeof(HlckEcho_Repository)));
            Assert.True(typeof(IRepository).IsAssignableFrom(typeof(IRepository)));
            Assert.True(typeof(IRepository).IsAssignableFrom(typeof(IHlckEcho_Repository)));

            Assert.False(typeof(IHlckEcho_Repository).IsAssignableFrom(typeof(IRepository)));
            Assert.False(typeof(IHlckEcho_Repository).IsAssignableFrom(typeof(EmptyRepository)));
            Assert.False(typeof(IHlckEcho_Repository).IsAssignableFrom(typeof(RepositoryBase)));
            Assert.True(typeof(IHlckEcho_Repository).IsAssignableFrom(typeof(HlckEcho_Repository)));

            var emptyRepository = new EmptyRepository();
            Assert.True(emptyRepository is IRepository);
            Assert.True(emptyRepository is IEmptyRepository);
            Assert.False(emptyRepository is IHlckEcho_Repository);
        }

        [Fact]
        public void ImplementedInterfaces_Test()
        {
            var includeBaseInterface = false;
            var configureType = typeof(IDomainService);
            var services = new ServiceCollection();
            var inputAssemblies = new List<Assembly> { typeof(FileDownload_Service).Assembly };

            inputAssemblies.SelectMany(x => x.GetReferencedAssemblies())
                .Where(t => false == inputAssemblies
                    .Any(a => a.FullName == t.FullName)
                )
                .Distinct()
                .ToList()
                .ForEach(x =>
                    inputAssemblies.Add(AppDomain.CurrentDomain.Load(x))
                );

            var interfacesFromAssemblies = inputAssemblies
                .SelectMany(o => o.DefinedTypes
                    .Where(x => x.IsInterface)
                    //.Where(x => x.GetInterfaces().Contains(configureType))
                    .Where(x => configureType.IsAssignableFrom(x))
                );

            if (false == includeBaseInterface)
            {
                interfacesFromAssemblies = interfacesFromAssemblies
                    .Where(x => x.FullName != configureType.FullName);
            }

            foreach (var @interface in interfacesFromAssemblies)
            {
                var implementedTypes = inputAssemblies
                    .SelectMany(o => o.DefinedTypes
                        .Where(x => x.IsClass)
                        //.Where(x => x.GetInterfaces().Contains(configureType))
                        .Where(x => @interface.IsAssignableFrom(x))
                    );


                foreach (var type in implementedTypes)
                {
                    services.TryAdd(new ServiceDescriptor(
                        @interface,
                        type)
                    );
                }
            }

            Assert.True(services.Count > 0);
        }
    }
}
