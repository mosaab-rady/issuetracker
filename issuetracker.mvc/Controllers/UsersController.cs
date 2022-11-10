using issuetracker.Email;
using issuetracker.Entities;
using issuetracker.Services;
using issuetracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace issuetracker.mvc.Controllers;

[Authorize(Roles = "manager")]
public class UsersController : Controller
{
	private readonly UserManager<AppUser> userManager;
	private readonly RoleManager<IdentityRole> roleManager;
	private readonly IProjectsService projectsService;
	private readonly IIssuesService issuesService;
	private readonly IEmailSender emailSender;
	public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IProjectsService projectsService, IIssuesService issuesService, IEmailSender emailSender)
	{
		this.userManager = userManager;
		this.roleManager = roleManager;
		this.projectsService = projectsService;
		this.issuesService = issuesService;
		this.emailSender = emailSender;
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



	[HttpGet]
	public async Task<IActionResult> UserInformation(string id)
	{
		var user = await userManager.Users.Include(x => x.AssignedIssues)
			.ThenInclude(issue => issue.Priority)
			.Include(x => x.AssignedIssues)
			.ThenInclude(issue => issue.Project)
			.Include(x => x.AssignedProjects)
			.SingleOrDefaultAsync(x => x.Id == id);


		if (user == null)
		{
			ViewBag.Error = $"No User found with this ID {id}";
			return View("NotFound");
		}

		UserDetailviewModel model = new()
		{
			UserId = user.Id,
			UserName = $"{user.FirstName} {user.LastName}",
			Email = user.Email,
			Image = user.Image,
			Roles = (await userManager.GetRolesAsync(user)).ToList()
		};

		foreach (var issue in user.AssignedIssues)
		{
			IssueViewModel issueViewModel = new()
			{
				IssueId = issue.Id.ToString(),
				Priority = issue.Priority ?? new Priority() { Color = "#6699ff", Name = "Not Set" },
				Status = Enum.GetName(typeof(Status), issue.Status),
				ProjectName = issue.Project.Name,
				Title = issue.Title
			};

			if (issue.Status == Status.Open)
			{
				model.OpenIssues.Add(issueViewModel);
			}
			else if (issue.Status == Status.Closed)
			{
				model.ClosedIssues.Add(issueViewModel);
			}
		}

		foreach (var project in user.AssignedProjects)
		{
			ProjectViewModel projectViewModel = new()
			{
				ProjectId = project.Id.ToString(),
				Name = project.Name,
				Slug = project.Slug,
				StartDate = project.StartDate,
				TargetEndDate = project.TargetEndDate,
				ActualEndDate = project.ActualEndDate
			};
			model.Projects.Add(projectViewModel);
		}

		return View(model);
	}



	[HttpPost]
	public async Task<IActionResult> Delete(string id)
	{
		var user = await userManager.FindByIdAsync(id);
		if (user is null)
		{
			ViewBag.Error = $"No User found with this ID {id}";
			return View("NotFound");
		}

		DeleteImage(user.Image);

		await userManager.DeleteAsync(user);

		return RedirectToAction("index", "users");
	}


	[HttpGet]
	public async Task<IActionResult> EditRolesInUser(string id)
	{
		var user = await userManager.FindByIdAsync(id);

		if (user is null)
		{
			ViewBag.Error = $"No User found with this Id {id}";
			return View("NotFound");
		}

		List<EditRoleInUserViewModel> model = new();

		foreach (var role in await roleManager.Roles.ToListAsync())
		{
			EditRoleInUserViewModel editRoleInUserViewModel = new()
			{
				RoleId = role.Id,
				Name = role.Name,
			};

			if (await userManager.IsInRoleAsync(user, role.Name))
			{
				editRoleInUserViewModel.IsSelected = true;
			}
			else
			{
				editRoleInUserViewModel.IsSelected = false;
			}
			model.Add(editRoleInUserViewModel);
		}


		ViewBag.UserId = user.Id;

		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> EditRolesInUser(List<EditRoleInUserViewModel> model, string id)
	{
		if (!ModelState.IsValid) return View(model);

		var user = await userManager.FindByIdAsync(id);
		if (user is null)
		{
			ViewBag.Error = $"No User found with this ID {id}";
			return View("NotFound");
		}

		var currentUserRoles = await userManager.GetRolesAsync(user);

		foreach (var role in model)
		{
			IdentityResult result = new();

			if (role.IsSelected && !currentUserRoles.Contains(role.Name))
			{
				result = await userManager.AddToRoleAsync(user, role.Name);
			}
			else if (!role.IsSelected && currentUserRoles.Contains(role.Name))
			{
				result = await userManager.RemoveFromRoleAsync(user, role.Name);
			}

			if (!result.Succeeded)
			{
				foreach (var err in result.Errors)
				{
					ModelState.AddModelError("", err.Description);
					return View(model);
				}
			}
		}

		return RedirectToAction("userinformation", "users", new { id = user.Id });
	}


	[HttpGet]
	public async Task<IActionResult> EditProjectsAssignedToUser(string id)
	{
		var user = await userManager.Users.Include(x => x.AssignedProjects).SingleOrDefaultAsync(x => x.Id == id);

		if (user is null)
		{
			ViewBag.Error = $"No User found with this Id {id}";
			return View("NotFound");
		}


		var projects = await projectsService.GetAllProjectsAsync();

		List<EditProjectAssignedToUserViewModel> model = new();

		foreach (var project in projects)
		{
			EditProjectAssignedToUserViewModel editProjectAssignedToUser = new()
			{
				ProjectId = project.Id.ToString(),
				ProjectName = project.Name
			};

			if (user.AssignedProjects.Contains(project))
			{
				editProjectAssignedToUser.IsSelected = true;
			}
			else
			{
				editProjectAssignedToUser.IsSelected = false;
			}
			model.Add(editProjectAssignedToUser);
		}


		ViewBag.UserId = user.Id;

		return View(model);
	}


	[HttpPost]
	public async Task<IActionResult> EditProjectsAssignedToUser(List<EditProjectAssignedToUserViewModel> model, string id)
	{
		if (!ModelState.IsValid) return View(model);

		var user = await userManager.Users.Include(x => x.AssignedProjects).SingleOrDefaultAsync(x => x.Id == id);

		if (user is null)
		{
			ViewBag.Error = $"No User found with this ID {id}";
			return View("NotFound");
		}

		foreach (var projectModel in model)
		{
			var project = await projectsService.GetProjectByIdAsync(Guid.Parse(projectModel.ProjectId));
			if (project is null)
			{
				ViewBag.Error = $"No Project found with this ID {projectModel.ProjectId}";
				return View("NotFound");
			}

			if (projectModel.IsSelected && !user.AssignedProjects.Contains(project))
			{
				await projectsService.AssignUser(user, project.Id);
			}
			else if (!projectModel.IsSelected && user.AssignedProjects.Contains(project))
			{
				await projectsService.UnAssignUser(user, project.Id);
			}
		}


		return RedirectToAction("userinformation", "users", new { id = user.Id });
	}




	[HttpGet]
	public async Task<IActionResult> EditIssuesAssignedToUser(string id)
	{
		var user = await userManager.Users.Include(x => x.AssignedIssues)
			.Include(x => x.AssignedProjects)
			.ThenInclude(project => project.Issues)
			.SingleOrDefaultAsync(x => x.Id == id);

		if (user is null)
		{
			ViewBag.Error = $"No User found with this Id {id}";
			return View("NotFound");
		}


		List<EditIssueAssignedToUserViewModel> model = new();

		List<Issue> availableIssuesToThisUser = new();

		foreach (var AssignedProject in user.AssignedProjects)
		{
			availableIssuesToThisUser.AddRange(from issue in AssignedProject.Issues
																				 where issue.Status == Status.Open
																				 select issue);
		}


		foreach (var availableIssue in availableIssuesToThisUser)
		{
			EditIssueAssignedToUserViewModel editIssueAssignedToUserViewModel = new()
			{
				IssueId = availableIssue.Id.ToString(),
				IssueTitle = availableIssue.Title,
				IssueStatus = Enum.GetName(typeof(Status), availableIssue.Status),
				ProjectName = availableIssue.Project.Name
			};

			if (user.AssignedIssues.Contains(availableIssue))
			{
				editIssueAssignedToUserViewModel.IsSelected = true;
			}
			else
			{
				editIssueAssignedToUserViewModel.IsSelected = false;
			}

			model.Add(editIssueAssignedToUserViewModel);
		}


		ViewBag.UserId = user.Id;
		return View(model);
	}



	[HttpPost]
	public async Task<IActionResult> EditIssuesAssignedToUser(List<EditIssueAssignedToUserViewModel> model, string id)
	{
		if (!ModelState.IsValid) return View(model);

		var user = await userManager.Users.Include(x => x.AssignedIssues)
			.Include(x => x.AssignedProjects)
			.ThenInclude(project => project.Issues)
			.SingleOrDefaultAsync(x => x.Id == id);

		if (user is null)
		{
			ViewBag.Error = $"No User found with this Id {id}";
			return View("NotFound");
		}


		List<Issue> availableIssuesToThisUser = new();

		foreach (var AssignedProject in user.AssignedProjects)
		{
			availableIssuesToThisUser.AddRange(from issue in AssignedProject.Issues
																				 where issue.Status == Status.Open
																				 select issue);
		}


		foreach (var assignedIssueViewModel in model)
		{
			var issue = await issuesService.GetIssueByIdWithUsersAsync(Guid.Parse(assignedIssueViewModel.IssueId));
			if (availableIssuesToThisUser.Contains(issue) && assignedIssueViewModel.IsSelected && !user.AssignedIssues.Contains(issue))
			{
				await issuesService.AssignUser(user, issue.Id);
				// send email to the assigned user
				string issueUrl = $"{Request.Scheme}://{Request.Host}/issues/issue/{issue.Id}";
				await emailSender.SendAssignToIssueAsync(user.Email, issueUrl, issue.Title, issue.Project.Name, User.Identity.Name, User.Identity.Name);
			}
			else if (availableIssuesToThisUser.Contains(issue) && !assignedIssueViewModel.IsSelected && user.AssignedIssues.Contains(issue))
			{
				await issuesService.UnAssignUser(user, issue.Id);
			}
		}

		return RedirectToAction("userinformation", "users", new { id = id });

	}

	private void DeleteImage(string filename)
	{
		string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", filename);
		System.IO.File.Delete(filePath);
	}

}