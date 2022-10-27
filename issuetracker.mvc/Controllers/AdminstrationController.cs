using issuetracker.Entities;
using issuetracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace issuetracker.mvc.Controllers;

[Authorize(Roles = "manager")]
public class AdminstrationController : Controller
{
	private readonly RoleManager<IdentityRole> roleManager;
	private readonly UserManager<AppUser> userManager;

	public AdminstrationController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
	{
		this.userManager = userManager;
		this.roleManager = roleManager;
	}


	[HttpGet]
	public IActionResult ListRoles()
	{
		var roles = roleManager.Roles;
		return View(roles);
	}

	[HttpGet]
	public IActionResult CreateRole()
	{
		CreateRoleViewModel model = new();
		return View(model);
	}


	[HttpPost]
	public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
	{
		if (!ModelState.IsValid) return View(model);

		IdentityRole role = new() { Name = model.Name };

		var result = await roleManager.CreateAsync(role);

		if (!result.Succeeded)
		{
			foreach (var err in result.Errors)
			{
				ModelState.AddModelError("", err.Description);
			}
			return View(model);
		}

		return RedirectToAction("listroles", "adminstration");
	}



	[HttpGet]
	public async Task<IActionResult> EditRole(string id)
	{
		IdentityRole role = await roleManager.FindByIdAsync(id);

		if (role == null)
		{
			ViewBag.Error = $"No role found with that Id {id}.";
			return View("NotFound");
		}

		var model = new EditRoleViewModel { Id = role.Id, Name = role.Name };


		foreach (var user in await userManager.Users.ToListAsync())
		{
			if (await userManager.IsInRoleAsync(user, role.Name))
			{
				AssignUserViewModel assignedToUserViewModel = new()
				{
					Email = user.Email,
					Image = user.Image
				};

				model.Users.Add(assignedToUserViewModel);
			}
		}

		return View(model);
	}


	[HttpPost]
	public async Task<IActionResult> EditRole(EditRoleViewModel model)
	{
		if (!ModelState.IsValid) return View(model);

		IdentityRole role = await roleManager.FindByIdAsync(model.Id);

		if (role == null)
		{
			ViewBag.Error = $"No Role found with that Id {model.Id}";
			return View("NotFound");
		}

		role.Name = model.Name;

		var result = await roleManager.UpdateAsync(role);

		if (!result.Succeeded)
		{
			foreach (var err in result.Errors)
			{
				ModelState.AddModelError("", err.Description);
			}
			return View(model);
		}

		return RedirectToAction("listroles", "adminstration");
	}



	[HttpPost]
	public async Task<IActionResult> DeleteRole(string id)
	{
		IdentityRole role = await roleManager.FindByIdAsync(id);

		if (role == null)
		{
			ViewBag.Error = $"No Role found with that Id";
			return View("NotFound");
		}

		var result = await roleManager.DeleteAsync(role);

		if (!result.Succeeded)
		{
			foreach (var err in result.Errors)
			{
				ModelState.AddModelError("", err.Description);
			}
			return View("ListRoles");
		}

		return RedirectToAction("ListRoles", "adminstration");
	}



	[HttpGet]
	public async Task<IActionResult> EditUsersInRole(string roleId)
	{

		ViewBag.roleId = roleId;

		var role = await roleManager.FindByIdAsync(roleId);

		if (role == null)
		{
			ViewBag.Error = $"No Role found with that Id {roleId}";
			return View("NotFound");
		}

		var model = new List<EditUserInRoleViewModel>();

		foreach (var user in await userManager.Users.ToListAsync())
		{
			EditUserInRoleViewModel editUserInRoleViewModel = new()
			{
				Email = user.Email,
				UserId = user.Id,
				Image = user.Image
			};

			if (await userManager.IsInRoleAsync(user, role.Name))
			{
				editUserInRoleViewModel.IsSelected = true;
			}
			else
			{
				editUserInRoleViewModel.IsSelected = false;
			}

			model.Add(editUserInRoleViewModel);
		}

		return View(model);
	}




	[HttpPost]
	public async Task<IActionResult> EditUsersInRole(List<EditUserInRoleViewModel> model, string roleId)
	{
		if (!ModelState.IsValid) return View(model);

		var role = await roleManager.FindByIdAsync(roleId);

		if (role == null)
		{
			ViewBag.Error = $"No Role found with that Id {roleId}";
			return View("NotFound");
		}

		foreach (var editUserInRoleViewModel in model)
		{
			var user = await userManager.FindByIdAsync(editUserInRoleViewModel.UserId);

			if (editUserInRoleViewModel.IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
			{
				await userManager.AddToRoleAsync(user, role.Name);
			}
			else if (!editUserInRoleViewModel.IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
			{
				await userManager.RemoveFromRoleAsync(user, role.Name);
			}
		}


		return RedirectToAction("editrole", new { Id = roleId });
	}

}