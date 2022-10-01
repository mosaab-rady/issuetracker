namespace issuetracker.ViewModels;

public class EditRoleViewModel
{
	public EditRoleViewModel()
	{
		Users = new List<AssignUserViewModel>();
	}
	public string Id { get; set; }
	public string Name { get; set; }
	public List<AssignUserViewModel> Users { get; set; }
}