using System.ComponentModel.DataAnnotations;

namespace issuetracker.Dtos;

public class CreateIssueDto
{
	[Required]
	public string Title { get; set; }

	[Required]
	public string Description { get; set; }

	[Required]
	public string ProjectId { get; set; }
}