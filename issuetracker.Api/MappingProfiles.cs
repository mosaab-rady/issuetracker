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
		CreateMap<Project,ProjectWithUsersDto>();
	}
}