using System.ComponentModel.DataAnnotations;

namespace issuetracker.Dtos;

public class UpdateIssueDto
{
	[Required]
	public string Id { get; set; }
	[Required]
	public string Title { get; set; }

	[Required]
	public string Description { get; set; }

	public DateTime TargetResolutionDate { get; set; }

	public string ResoliotionSummary { get; set; }
}