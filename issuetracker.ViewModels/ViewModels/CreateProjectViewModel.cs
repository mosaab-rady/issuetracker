using System.ComponentModel.DataAnnotations;

namespace issuetracker.ViewModels;

public class CreateProjectViewModel
{
	[Required]
	public string Name { get; set; }

	[Required]
	[Display(Name = "Start Date")]
	public DateTime StartDate { get; set; }

	[Required]
	[Display(Name = "Target End Date")]
	public DateTime TargetEndDate { get; set; }

}