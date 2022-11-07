using System.ComponentModel.DataAnnotations;

namespace issuetracker.Dtos;

public class UpdateProjectDto
{
	[Required]
	public string Id { get; set; }

	[Required]
	public string Name { get; set; }

	[Required]
	public DateTime StartDate { get; set; }

	[Required]
	public DateTime TargetEndDate { get; set; }
	public DateTime ActualEndDate { get; set; }
}