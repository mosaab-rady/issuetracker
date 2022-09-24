using System.ComponentModel.DataAnnotations;

namespace issuetracker.Entities;

public class AppUser
{
	[Required]
	public string FirstName { get; set; }

	[Required]
	public string LastName { get; set; }

	[Required]
	public string Image { get; set; }
	public List<Project> AssignedProjects { get; set; }
	public List<Issue> AssignedIssues { get; set; }
}