using Deals.Domain;
using DTO;
using AutoMapper;
using Deals.Domain.Models;
using Infrastructure;
using Infrastructure.Repository;

namespace Deals.Business.Mappers
{
    public class ProjectMapper : Profile
    {
        public ProjectMapper()
        {
            CreateMap<Project, ProjectDTO>(MemberList.None);

            CreateMap<AddProjectDTO, Project>(MemberList.None)
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<UpdateProjectDTO, Project>(MemberList.None);

            CreateMap<PaginatedList<Project>, PageModel<ProjectDTO>>(MemberList.None).
                ForMember(dest => dest.Data, opt => opt.MapFrom(src => src));

            CreateMap<PaginatedList<ProjectDTO>, PageModel<ProjectDTO>>(MemberList.None).
               ForMember(dest => dest.Data, opt => opt.MapFrom(src => src));

            CreateMap<PaginatedList<AuditLogsDTO>, PageModel<AuditLogsDTO>>(MemberList.None).
               ForMember(dest => dest.Data, opt => opt.MapFrom(src => src));
        }
    }
}
