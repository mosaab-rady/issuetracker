using issuetracker.Entities;
using issuetracker.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace issuetracker.mvc.Controllers;

public class UsersController : Controller
{
	private readonly UserManager<AppUser> userManager;
	private readonly RoleManager<IdentityRole> roleManager;
	public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
	{
		this.userManager = userManager;
		this.roleManager = roleManager;
	}

	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var users = await userManager.Users.ToListAsync();

		List<UserViewModel> model = new();

		foreach (var user in users)
		{
			UserViewModel userViewModel = new()
			{
				Id = user.Id,
				UserName = $"{user.FirstName} {user.LastName}",
				Email = user.Email,
				Image = user.Image,
				Roles = (await userManager.GetRolesAsync(user)).ToList()
			};

			model.Add(userViewModel);
		}

		return View(model);
	}
}