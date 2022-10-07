namespace issuetracker.ViewModels;

public class DashboardViewModel
{
	public DashboardViewModel()
	{
		OverDueIssues = new List<IssueViewModel>();
		RecentlyOpenedIssues = new List<IssueViewModel>();
		UnAssignedIssues = new List<IssueViewModel>();
		Projects = new List<ProjectOverviewViewModel>();
	}
	public List<ProjectOverviewViewModel> Projects { get; set; }
	public List<IssueViewModel> OverDueIssues { get; set; }
	public List<IssueViewModel> RecentlyOpenedIssues { get; set; }
	public List<IssueViewModel> UnAssignedIssues { get; set; }
}