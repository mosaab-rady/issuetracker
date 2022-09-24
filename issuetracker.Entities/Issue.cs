using System.ComponentModel.DataAnnotations;

namespace issuetracker.Entities;

public class Issue
{
	[Key]
	public Guid Id { get; set; }

	[Required]
	public string Title { get; set; }

	[Required]
	public string Slug { get; set; }

	[Required]
	public string Description { get; set; }

	[Required]
	public DateTimeOffset CreatedOn { get; set; }


	public DateTimeOffset TargetResolutionDate { get; set; }

	public DateTimeOffset ActualResolutionDate { get; set; }

	[Required]
	public Project Project { get; set; }

	public Status Status { get; set; }

	public Priority Priority { get; set; }

	public string ResoliotionSummary { get; set; }


	public List<Tag> Tags { get; set; }

}