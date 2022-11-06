using System.ComponentModel.DataAnnotations;

namespace issuetracker.Dtos;

public class CreateProjectDto
{
	[Required]
	public string Name { get; set; }

	[Required]
	public DateTime StartDate { get; set; }

	[Required]
	public DateTime TargetEndDate { get; set; }
}