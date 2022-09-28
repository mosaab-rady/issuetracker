namespace issuetracker.ViewModels;

public class EditRoleViewModel
{
	public EditRoleViewModel()
	{
		Users = new List<AssignedToUserViewModel>();
	}
	public string Id { get; set; }
	public string Name { get; set; }
	public List<AssignedToUserViewModel> Users { get; set; }
}