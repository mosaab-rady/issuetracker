namespace issuetracker.ViewModels;

public class AssignedToMeViewModel
{
	public AssignedToMeViewModel()
	{
		OverDueIssues = new List<IssueViewModel>();
		OpenIssues = new List<IssueViewModel>();
	}
	public List<IssueViewModel> OverDueIssues { get; set; }
	public List<IssueViewModel> OpenIssues { get; set; }
}