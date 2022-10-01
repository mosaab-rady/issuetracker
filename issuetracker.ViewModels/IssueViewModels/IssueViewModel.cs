using issuetracker.Entities;

namespace issuetracker.ViewModels;

public class IssueViewModel
{
	public string IssueId { get; set; }
	public string Title { get; set; }
	public string ProjectName { get; set; }
	public string Status { get; set; }
	public Priority Priority { get; set; }
}