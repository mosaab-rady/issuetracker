using System.ComponentModel.DataAnnotations;

namespace issuetracker.Entities;

public class Project
{
	[Key]
	public Guid Id { get; set; }

	[Required]
	public string Name { get; set; }

	[Required]
	public string Slug { get; set; }

	[Required]
	public DateTime StartDate { get; set; }

	[Required]
	public DateTime TargetEndDate { get; set; }

	public DateTime ActualEndDate { get; set; }

	[Required]
	public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

	[Required]
	public string CreatedBy { get; set; }

	public List<AppUser> AssignedTo { get; set; }

	public List<Issue> Issues { get; set; }
}