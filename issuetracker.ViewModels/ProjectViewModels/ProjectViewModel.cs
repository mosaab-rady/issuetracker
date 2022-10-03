namespace issuetracker.ViewModels;

public class ProjectViewModel
{
	public string Id { get; set; }
	public string Slug { get; set; }
	public string Name { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime TargetEndDate { get; set; }
	public DateTime ActualEndDate { get; set; }
}