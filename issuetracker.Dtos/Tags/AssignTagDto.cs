using System.ComponentModel.DataAnnotations;

namespace issuetracker.Dtos;

public class AssignTagDto
{
	[Required]
	public Guid TagId { get; set; }

	public string Name { get; set; }

	[Required]
	public bool IsSelected { get; set; }
}