using System.ComponentModel.DataAnnotations;

namespace issuetracker.ViewModels;

public class CreateRoleViewModel
{
	[Required]
	public string Name { get; set; }
}