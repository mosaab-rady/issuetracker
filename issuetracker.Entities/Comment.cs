using System.ComponentModel.DataAnnotations;

namespace issuetracker.Entities;

public class Comment
{
	[Key]
	public Guid Id { get; set; }

	[Required]
	public AppUser User { get; set; }

	[Required]
	public Issue Issue { get; set; }

	[Required]
	public DateTimeOffset CreatedOn { get; set; }
}