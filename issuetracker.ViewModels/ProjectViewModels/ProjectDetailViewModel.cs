using issuetracker.Entities;

namespace issuetracker.ViewModels;

public class ProjectDetailViewModel
{
	public string Id { get; set; }
	public string Name { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime TargetEndDate { get; set; }
	public DateTime ActualEndDate { get; set; }
	public DateTime CreatedOn { get; set; }
	public string CreatedBy { get; set; }
	public List<IssueViewModel> UnAssignedIssued { get; set; }
	public List<IssueViewModel> OverDueIssues { get; set; }
	public List<IssueViewModel> OpenIssues { get; set; }
	public List<IssueViewModel> ClosedIssue { get; set; }
	public List<AssignUserViewModel> AssignedTo { get; set; }
}