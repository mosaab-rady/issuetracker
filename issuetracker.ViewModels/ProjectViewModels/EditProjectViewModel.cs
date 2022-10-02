using System.ComponentModel.DataAnnotations;

namespace issuetracker.ViewModels;

public class EditProjectViewModel
{
	public string Id { get; set; }

	[Required]
	public string Name { get; set; }

	public string Slug { get; set; }

	[Required]
	[Display(Name = "Start Date")]
	public DateTime StartDate { get; set; }

	[Required]
	[Display(Name = "Target end Date")]
	public DateTime TargetEndDate { get; set; }

	[Display(Name = "Actual End Date")]
	public DateTime ActualEndDate { get; set; }
}