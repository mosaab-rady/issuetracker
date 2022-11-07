namespace issuetracker.Dtos;

public class ProjectWithUsersDto : ProjectDto
{
	public ProjectWithUsersDto()
	{
		AssignedTo = new List<UserDto>();
	}
	public List<UserDto> AssignedTo { get; set; }

}