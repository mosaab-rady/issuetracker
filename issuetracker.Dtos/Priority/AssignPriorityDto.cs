using System.ComponentModel.DataAnnotations;

namespace issuetracker.Dtos;

public class AssignPriorityDto
{
	[Required]
	public Guid PriorityId { get; set; }

	public string Name { get; set; }
}