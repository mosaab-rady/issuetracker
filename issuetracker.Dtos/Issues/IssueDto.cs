namespace issuetracker.Dtos;

public class IssueDto
{
	public string Id { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public DateTime CreatedOn { get; set; }
	public string CreatedBy { get; set; }
	public DateTime TargetResolutionDate { get; set; }
	public DateTime ActualResolutionDate { get; set; }
	public string Status { get; set; }
	public PriorityDto Priority { get; set; }
	public string ProjectName { get; set; }
	public string ResoliotionSummary { get; set; }
}
