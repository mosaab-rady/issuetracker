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
	public DateTimeOffset StartDate { get; set; }

	[Required]
	public DateTimeOffset TargetEndDate { get; set; }

	public DateTimeOffset ActualEndDate { get; set; }

	[Required]
	public DateTimeOffset CreatedOn { get; set; }

	public List<Issue> Issues { get; set; }
}