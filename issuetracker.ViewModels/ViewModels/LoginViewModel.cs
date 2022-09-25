using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace issuetracker.ViewModels;

public class LoginViewModel
{
	[Required]
	[EmailAddress]
	public string Email { get; set; }

	[Required]
	[DataType(DataType.Password)]
	public string Password { get; set; }


	[DisplayName("Remember me")]
	public bool RememberMe { get; set; }
}