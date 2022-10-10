using System.ComponentModel.DataAnnotations;

namespace issuetracker.ViewModels;

public class CreateCommentViewModel
{
	[Required]
	public string IssueId { get; set; }

	[Required]
	public string Comment { get; set; }
}