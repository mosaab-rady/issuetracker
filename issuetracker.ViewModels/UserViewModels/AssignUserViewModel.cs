namespace issuetracker.ViewModels;

public class AssignUserViewModel
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string Image { get; set; }
	public string Email { get; set; }
	public List<string> Roles { get; set; }
}