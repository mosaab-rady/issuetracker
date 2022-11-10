using System.ComponentModel.DataAnnotations;

namespace issuetracker.Dtos;

public class CreateTagDto
{
	[Required]
	public string Name { get; set; }

	[Required]
	public string Color { get; set; }
}
