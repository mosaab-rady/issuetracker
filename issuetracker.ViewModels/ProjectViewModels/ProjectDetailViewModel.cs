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
	public List<Issue> UnAssignedIssued { get; set; }
	public List<Issue> OverDueIssues { get; set; }
	public List<Issue> OpenIssues { get; set; }
	public List<Issue> ClosedIssue { get; set; }
	public List<AssignedToUserViewModel> AssignedTo { get; set; }
}