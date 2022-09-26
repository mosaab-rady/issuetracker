namespace issuetracker.ViewModels;

public class EditRoleViewModel
{
	public EditRoleViewModel()
	{
		Users = new List<string>();
	}
	public string Id { get; set; }
	public string Name { get; set; }
	public List<string> Users { get; set; }
}