namespace issuetracker.ViewModels;

public class AllIsuesViewModel
{
	public string Project { get; set; }
	public IEnumerable<string> Projects { get; set; }
	public string Priority { get; set; }
	public IEnumerable<string> Priorities { get; set; }
	public string Status { get; set; }
	public IEnumerable<string> Statuses { get; set; }
	public List<IssueViewModel> issueViewModels { get; set; }
}