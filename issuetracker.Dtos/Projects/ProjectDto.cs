namespace issuetracker.Dtos;
public class ProjectDto
{

	public string Id { get; set; }
	public string Name { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime TargetEndDate { get; set; }
	public DateTime ActualEndDate { get; set; }
	public DateTime CreatedOn { get; set; }
	public string CreatedBy { get; set; }
}
