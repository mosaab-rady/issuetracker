namespace issuetracker.Dtos;

public class UserDto
{
	public UserDto()
	{
		Roles = new List<string>();
	}
	public string Id { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Email { get; set; }
	public string Image { get; set; }
	public List<string> Roles { get; set; }
}