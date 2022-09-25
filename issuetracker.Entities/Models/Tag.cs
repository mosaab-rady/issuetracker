using System.ComponentModel.DataAnnotations;

namespace issuetracker.Entities;

public class Tag
{
	[Key]
	public Guid Id { get; set; }

	[Required]
	public string Name { get; set; }

	[Required]
	public string Color { get; set; }

	public List<Issue> Issues { get; set; }
}