using System.ComponentModel.DataAnnotations;

namespace issuetracker.Dtos;

public class AssignRoleDto
{
	[Required]
	public string RoleId { get; set; }

	[Required]
	public string Name { get; set; }

	[Required]
	public bool IsSelected { get; set; }
}