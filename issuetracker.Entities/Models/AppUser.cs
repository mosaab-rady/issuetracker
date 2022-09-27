using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace issuetracker.Entities;

public class AppUser : IdentityUser
{
	[Required]
	public string FirstName { get; set; }

	[Required]
	public string LastName { get; set; }

	[Required]
	public string Image { get; set; }

	public List<Issue> AssignedIssues { get; set; }

	public List<Project> AssignedProjects { get; set; }
}