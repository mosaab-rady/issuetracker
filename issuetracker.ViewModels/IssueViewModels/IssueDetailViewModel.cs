using issuetracker.Entities;

namespace issuetracker.ViewModels;

public class IssueDetailViewModel
{
	public IssueDetailViewModel()
	{
		AssignedTo = new List<AssignUserViewModel>();
		Tags = new List<Tag>();
		Priority = new Priority();
	}
	public string IssueId { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public DateTime CreatedOn { get; set; }
	public DateTime TargetResolutionDate { get; set; }
	public string CreatedBy { get; set; }
	public DateTime ActualResolutionDate { get; set; }
	public string ProjectName { get; set; }
	public string Status { get; set; }
	public Priority Priority { get; set; }
	public string ResolutionSummary { get; set; }
	public List<AssignUserViewModel> AssignedTo { get; set; }
	public List<Tag> Tags { get; set; }
}