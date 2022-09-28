using System.ComponentModel.DataAnnotations;
using issuetracker.Entities;

namespace issuetracker.ViewModels;

public class CreateIssueViewModel
{
	[Required]
	public string Title { get; set; }

	[Required]
	public string Description { get; set; }
	public List<AssignTagViewModel> Tags { get; set; }

	[Required]
	public string ProjectId { get; set; }

	public List<AssignProjectViewModel> projects { get; set; }
}