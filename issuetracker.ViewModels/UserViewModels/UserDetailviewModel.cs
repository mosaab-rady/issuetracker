namespace issuetracker.ViewModels;


public class UserDetailviewModel
{
	public UserDetailviewModel()
	{
		Roles = new List<string>();
		Projects = new List<ProjectViewModel>();
		OpenIssues = new List<IssueViewModel>();
		ClosedIssues = new List<IssueViewModel>();
	}
	public string UserId { get; set; }
	public string UserName { get; set; }
	public string Email { get; set; }
	public string Image { get; set; }
	public List<string> Roles { get; set; }
	public List<ProjectViewModel> Projects { get; set; }
	public List<IssueViewModel> OpenIssues { get; set; }
	public List<IssueViewModel> ClosedIssues { get; set; }
}