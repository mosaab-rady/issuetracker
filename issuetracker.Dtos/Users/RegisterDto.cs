using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace issuetracker.Dtos;

public class RegisterDto
{
	[Required]
	public string FirstName { get; set; }

	[Required]
	public string LastName { get; set; }

	[Required]
	[EmailAddress]
	public string Email { get; set; }

	[Required]
	public string Password { get; set; }

	[Required]
	[Compare("Password", ErrorMessage = "Password and confirm password must be the same.")]
	public string ConfirmPassword { get; set; }

	public IFormFile Image { get; set; }
}