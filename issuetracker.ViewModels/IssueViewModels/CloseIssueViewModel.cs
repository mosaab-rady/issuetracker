using System.ComponentModel.DataAnnotations;

namespace issuetracker.ViewModels;


public class CloseIssueViewModel
{
	[Required]
	[Display(Name = "Issue Id")]
	public string IssueId { get; set; }

	public string Title { get; set; }

	[Required]
	[Display(Name = "Summary")]
	public string ResolutionSummary { get; set; }
}