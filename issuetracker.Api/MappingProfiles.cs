using AutoMapper;
using issuetracker.Dtos;
using issuetracker.Entities;

namespace issuetracker.Api.Helpers;

public class MappingProfiles : Profile
{
	public MappingProfiles()
	{
		CreateMap<Project, ProjectDto>();
		CreateMap<AppUser, UserDto>();
		CreateMap<UserDto, AppUser>();
		CreateMap<CreateProjectDto, Project>();
		CreateMap<Project, ProjectWithUsersDto>();
		CreateMap<Priority, PriorityDto>();
		CreateMap<CreatePriorityDto, Priority>();
		CreateMap<Tag, TagDto>();
		CreateMap<CreateTagDto, Tag>();


		CreateMap<Issue, IssueDto>()
		.ForMember(issueDto => issueDto.Status, opt => opt.MapFrom(src => Enum.GetName(typeof(Status), src.Status)))
		.ForMember(issueDto => issueDto.ProjectName, opt => opt.MapFrom(src => src.Project.Name));

		CreateMap<CreateIssueDto, Issue>();

	}
}