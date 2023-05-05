using Autofac;
using Deals.Business.Modules;
using Infrastructure.Modules;

namespace API;
public static class Bootstrapper
{
    public static void IntegrateContainer(this ContainerBuilder builder)
    {
        builder.RegisterModule(new InfrastructureModule());
        builder.RegisterModule(new BusinessModule());
    }
}
