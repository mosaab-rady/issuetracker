using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace issuetracker.ViewModels;

public class RegisterViewModel
{
	[Required]
	[Display(Name = "First Name")]
	public string FirstName { get; set; }

	[Required]
	[Display(Name = "Last Name")]
	public string LastName { get; set; }

	[Required]
	[EmailAddress]
	[Remote(action: "IsEmailNotUsed", "Account")]
	public string Email { get; set; }

	[Required]
	[DataType(DataType.Password)]
	public string Password { get; set; }

	[Required]
	[DataType(DataType.Password)]
	[Compare("Password", ErrorMessage = "Password and confirm password must be the same.")]
	[Display(Name = "Confirm Password")]
	public string ConfirmPassword { get; set; }


	[Required(ErrorMessage = "Please add your image.")]
	public IFormFile Image { get; set; }
}