using System.ComponentModel.DataAnnotations;
using issuetracker.Entities;

namespace issuetracker.ViewModels;

public class CreateIssueViewModel
{
	public CreateIssueViewModel()
	{
		Tags = new List<AssignTagViewModel>();
		projects = new List<AssignProjectViewModel>();
	}


	[Required]
	public string Title { get; set; }

	[Required]
	public string Description { get; set; }
	public List<AssignTagViewModel> Tags { get; set; }

	public CreateTagViewModel CreateTagViewModel { get; set; }

	[Required]
	public string ProjectId { get; set; }

	public List<AssignProjectViewModel> projects { get; set; }
}