using Autofac;
using Nwpie.Foundation.Common.Extras;

namespace Nwpie.xUnit.App_Start
{
    public static class DefaultComponentUtil
    {
        public static void InitialAutofac()
        {
            var builder = ComponentMgr.Instance.DIBuilder
                ?? new ContainerBuilder();
            builder.RegisterModule<TestComponentModule>();

            ComponentMgr.Instance.ManualBuild(builder);
        }
    }
}
