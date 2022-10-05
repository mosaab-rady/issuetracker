namespace issuetracker.ViewModels;

public class EditIssueAssignedToUserViewModel
{
	public string IssueId { get; set; }
	public string IssueTitle { get; set; }
	public string IssueStatus { get; set; }
	public string ProjectName { get; set; }
	public bool IsSelected { get; set; }
}