using System.ComponentModel.DataAnnotations;
using issuetracker.Entities;

namespace issuetracker.ViewModels;


public class EditIssueViewModel
{
	public EditIssueViewModel()
	{
		Tags = new List<AssignTagViewModel>();
		Priorities = new List<Priority>();
	}
	[Required]
	[Display(Name = "Issue ID")]
	public string IssueId { get; set; }

	[Required]
	public string Title { get; set; }

	[Required]
	public string Description { get; set; }

	[Display(Name = "Target Resolution Date")]
	public DateTime TargetResolutionDate { get; set; }

	public List<Priority> Priorities { get; set; }
	public string PriorityId { get; set; }
	public CreatePriorityViewModel CreatePriorityViewModel { get; set; }

	[Display(Name = "Resolution Summary")]
	public string ResolutionSummary { get; set; }
	public List<AssignTagViewModel> Tags { get; set; }
	public CreateTagViewModel CreateTagViewModel { get; set; }

}