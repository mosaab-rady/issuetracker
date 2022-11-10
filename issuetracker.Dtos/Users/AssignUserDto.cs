using System.ComponentModel.DataAnnotations;

namespace issuetracker.Dtos;

public class AssignUserDto
{
	[Required]
	public string UserId { get; set; }

	public string Email { get; set; }

	[Required]
	public bool IsSelected { get; set; }
}