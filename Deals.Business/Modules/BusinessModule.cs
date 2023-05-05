using Autofac;
using Deals.Business.Interface;


namespace Deals.Business.Modules;

public class BusinessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DashboardBusiness>().As<IDashboardBusiness>().InstancePerLifetimeScope();
        builder.RegisterType<IdentityBusiness>().As<IIdentityBusiness>().InstancePerLifetimeScope();
        builder.RegisterType<ProjectBusiness>().As<IProjectBusiness>().InstancePerLifetimeScope();
        builder.RegisterType<ProjectCredBusiness>().As<IProjectCredBusiness>().InstancePerLifetimeScope();
        builder.RegisterType<TaxonomyBusiness>().As<ITaxonomyBusiness>().InstancePerLifetimeScope();
        builder.RegisterType<UserBusiness>().As<IUserBusiness>().InstancePerLifetimeScope();

        builder.RegisterType<Implementation.Ctm.DashboardBusiness>().As<Interface.Ctm.IDashboardBusiness>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.IdentityBusiness>().As<Interface.Ctm.IIdentityBusiness>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.ProjectBusiness>().As<Interface.Ctm.IProjectBusiness>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.ProjectCredBusiness>().As<Interface.Ctm.IProjectCredBusiness>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.TaxonomyBusiness>().As<Interface.Ctm.ITaxonomyBusiness>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.UserBusiness>().As<Interface.Ctm.IUserBusiness>().InstancePerLifetimeScope();

        builder.RegisterType<Implementation.Cfib.ProjectBusiness>().As<Interface.Cfib.IProjectBusiness>().InstancePerLifetimeScope();
    }
}
