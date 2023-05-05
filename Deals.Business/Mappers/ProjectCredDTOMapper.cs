using AutoMapper;
using Deals.Domain.Models;
using DTO;
using Infrastructure;
using Infrastructure.Repository;

namespace Deals.Business.Mappers
{
    public class ProjectCredMapper : Profile
    {
        public ProjectCredMapper()
        {
            CreateMap<AddProjectCredDTO, ProjectCredDetail>(MemberList.None);
            CreateMap<AddProjectCredDTO, ProjectCredLookup>(MemberList.None);
            CreateMap<AddProjectCredDTO, ProjectPublicWebsite>(MemberList.None);

            CreateMap<UpdateProjectCredDTO, ProjectCredDetail>(MemberList.None)
               .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
               .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifieddBy, opt => opt.MapFrom(src => src.ModifiedBy));
            CreateMap<UpdateProjectCredDTO, ProjectCredLookup>(MemberList.None).
                ForMember(dest => dest.ProjectCredLookupId, opt => opt.Ignore());
            CreateMap<UpdateProjectCredDTO, ProjectPublicWebsite>(MemberList.None).
                ForMember(dest => dest.ProjectPublicWebsiteId, opt => opt.Ignore());

            CreateMap<PaginatedList<ProjectDownloadCredDTO>, PageModel<ProjectDownloadCredDTO>>(MemberList.None).
              ForMember(dest => dest.Data, opt => opt.MapFrom(src => src));

            CreateMap<ProjectCredDetail, ProjectCredDTO>(MemberList.None);
            CreateMap<UpdateProjectCredDTO, ProjectCredDTO>(MemberList.None);
            CreateMap<UpdateProjectCredDTO, AddProjectCredDTO>(MemberList.None);
            CreateMap<ProjectCredWebsitesInfoDTO, ProjectPublicWebsite>(MemberList.None).
              ForMember(dest => dest.WebsiteUrl, opt => opt.MapFrom(src => src.WebsiteLink)).
               ForMember(dest => dest.QuotedinAnnouncements, opt => opt.MapFrom(src => src.PwCNameQuoted));

        }

        public static class ProjectCredDTOMapper
        {
            public static ProjectCredDetail ProjectCredDetailsDTOMapper(IMapper mapper, UpdateProjectCredDTO updateproject, ProjectCredDetail projectCredDetails, Guid projectId)
            {
                var projectCredInfo = mapper.Map<UpdateProjectCredDTO,ProjectCredDetail>(updateproject, projectCredDetails);
                projectCredInfo.ProjectId = projectId;
                return projectCredInfo;
            }

            public static ProjectPublicWebsite ProjectCredWebsiteDTOMapper(IMapper mapper, ProjectCredWebsitesInfoDTO projectWebsites, Guid projectId, DateTime createdOn, Guid createdBy)
            {
                var projectCredWebsitesInfo = mapper.Map<ProjectPublicWebsite>(projectWebsites);
                projectCredWebsitesInfo.ProjectId = projectId;
                projectCredWebsitesInfo.CreatedBy = createdBy;
                projectCredWebsitesInfo.CreatedOn = createdOn;

                return projectCredWebsitesInfo;
            }
        }
    }
}
