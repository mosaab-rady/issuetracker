namespace issuetracker.ViewModels;

public class ProjectOverviewViewModel
{
	public string ProjectID { get; set; }
	public string ProjectName { get; set; }
	public string Slug { get; set; }
	public DateTime TargetEndDate { get; set; }
	public DateTime ActaulEndDate { get; set; }
	public int OverdueIssues { get; set; }
	public int UnAssignedIssues { get; set; }
	public int OpenIssues { get; set; }
	public int ClosedIssues { get; set; }
	public int AllIssues
	{
		get
		{
			return ClosedIssues + OpenIssues;
		}
	}
}