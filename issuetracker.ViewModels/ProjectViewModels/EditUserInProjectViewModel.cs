namespace issuetracker.ViewModels;

public class EditUserInProjectViewModel
{
	public string UserId { get; set; }
	public string Email { get; set; }
	public string Image { get; set; }
	public bool IsSelected { get; set; }
	public List<string> Roles { get; set; }
}