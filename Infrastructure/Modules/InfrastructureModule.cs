using Autofac;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Ctm;

namespace Infrastructure.Modules;
public class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<IdentityRepository>().As<Infrastructure.Interfaces.IIdentityRepository>().InstancePerLifetimeScope();
        builder.RegisterType<UsersRepository>().As<IUsersRepository>().InstancePerLifetimeScope();
        builder.RegisterType<RolesRepository>().As<IRolesRepository>().InstancePerLifetimeScope();
        builder.RegisterType<ProjectRepository>().As<IProjectRepository>().InstancePerLifetimeScope();
        builder.RegisterType<ProjectCredRepository>().As<IProjectCredRepository>().InstancePerLifetimeScope();
        builder.RegisterType<TaxonomyRepository>().As<ITaxonomyRepository>().InstancePerLifetimeScope();
        builder.RegisterType<TaxonomyCategoryRepository>().As<ITaxonomyCategoryRepository>().InstancePerLifetimeScope();
        builder.RegisterType<TaxonomyEngagementTypesRepository>().As<ITaxonomyEngagementTypesRepository>().InstancePerLifetimeScope();
        builder.RegisterType<EngagementTypesRepository>().As<IEngagementTypesRepository>().InstancePerLifetimeScope();
        builder.RegisterType<DashboardRepository>().As<Infrastructure.Interfaces.IDashboardRepository>().InstancePerLifetimeScope();
        builder.RegisterType<ProjectMailDetailsRepository>().As<IProjectMailDetailsRepository>().InstancePerLifetimeScope();

        builder.RegisterType<Implementation.Ctm.IdentityRepository>().As<Interfaces.Ctm.IIdentityRepository>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.UsersRepository>().As<Interfaces.Ctm.IUsersRepository>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.RolesRepository>().As<Interfaces.Ctm.IRolesRepository>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.ProjectRepository>().As<Interfaces.Ctm.IProjectRepository>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.ProjectCredRepository>().As<Interfaces.Ctm.IProjectCredRepository>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.TaxonomyRepository>().As<Interfaces.Ctm.ITaxonomyRepository>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.TaxonomyCategoryRepository>().As<Interfaces.Ctm.ITaxonomyCategoryRepository>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.TaxonomyEngagementTypesRepository>().As<Interfaces.Ctm.ITaxonomyEngagementTypesRepository>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.EngagementTypesRepository>().As<Interfaces.Ctm.IEngagementTypesRepository>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.DashboardRepository>().As<Infrastructure.Interfaces.Ctm.IDashboardRepository>().InstancePerLifetimeScope();
        builder.RegisterType<Implementation.Ctm.ProjectMailDetailsRepository>().As<Interfaces.Ctm.IProjectMailDetailsRepository>().InstancePerLifetimeScope();

        builder.RegisterType<Implementation.Cfib.ProjectRepository>().As<Interfaces.Cfib.IProjectRepository>().InstancePerLifetimeScope();
        builder.RegisterType<ProjectsAuditLogRepository>().As<IProjectsAuditLogRepository>().InstancePerLifetimeScope();
    }
}
