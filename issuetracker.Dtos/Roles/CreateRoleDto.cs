using System.ComponentModel.DataAnnotations;

namespace issuetracker.Dtos;

public class CreateRoleDto
{
	[Required]
	public string Name { get; set; }
}