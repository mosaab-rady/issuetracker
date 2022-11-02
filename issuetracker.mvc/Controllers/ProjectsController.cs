using issuetracker.Entities;
using issuetracker.Services;
using issuetracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slugify;

namespace issuetracker.mvc.Controllers;

[Authorize]
public class ProjectsController : Controller
{
	private readonly IProjectsService projectsService;
	private readonly IIssuesService issuesService;
	private readonly UserManager<AppUser> userManager;
	public ProjectsController(IProjectsService projectsService, UserManager<AppUser> userManager, IIssuesService issuesService)
	{
		this.projectsService = projectsService;

		this.userManager = userManager;
		this.issuesService = issuesService;
	}

	[Authorize(Roles = "manager")]
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var projects = await projectsService.GetAllProjectsAsync();
		List<ProjectViewModel> model = new();
		foreach (var project in projects)
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

			model.Add(projectViewModel);
		}

		return View(model);
	}


	[HttpGet]
	public async Task<IActionResult> Project(string id)
	{
		Project project = await projectsService.GetProjectWithUsersAsync(slug: id);

		if (project == null)
		{
			ViewBag.Error = $"No Project found with that name {id}.";
			return View("NotFound");
		}

		List<IssueViewModel> unAssignedIssues = new();
		List<IssueViewModel> overDueIssues = new();
		List<IssueViewModel> openIssues = new();
		List<IssueViewModel> closedIssues = new();

		foreach (var issue in await issuesService.GetIssuesInProjectWithUsersAsync(project.Id))
		{
			IssueViewModel issueViewModel = new()
			{
				IssueId = issue.Id.ToString(),
				Priority = issue.Priority ?? new Priority() { Color = "#6699ff", Name = "Not Set" },
				Status = Enum.GetName(typeof(Status), issue.Status),
				ProjectName = project.Name,
				Title = issue.Title
			};

			if (issue.AssignedTo.Count() == 0)
			{
				unAssignedIssues.Add(issueViewModel);
			}

			if (issue.TargetResolutionDate < DateTime.UtcNow && issue.Status == Status.Open)
			{
				overDueIssues.Add(issueViewModel);
			}

			if (issue.Status == Status.Open)
			{
				openIssues.Add(issueViewModel);
			}

			if (issue.Status == Status.Closed)
			{
				closedIssues.Add(issueViewModel);
			}

		}


		List<AssignUserViewModel> assignedToUserViewModels = new();

		foreach (var user in project.AssignedTo)
		{
			var roles = await userManager.GetRolesAsync(user);

			AssignUserViewModel assignedToUserViewModel = new()
			{
				Email = user.Email,
				Name = $"{user.FirstName} {user.LastName}",
				Id = user.Id,
				Image = user.Image,
				Roles = roles.ToList()
			};

			assignedToUserViewModels.Add(assignedToUserViewModel);
		}

		ProjectDetailViewModel projectDetailViewModel = new()
		{
			Id = project.Id.ToString(),
			Name = project.Name,
			StartDate = project.StartDate,
			TargetEndDate = project.TargetEndDate,
			ActualEndDate = project.ActualEndDate,
			CreatedOn = project.CreatedOn,
			CreatedBy = project.CreatedBy,
			UnAssignedIssued = unAssignedIssues,
			OverDueIssues = overDueIssues,
			OpenIssues = openIssues,
			ClosedIssue = closedIssues,
			AssignedTo = assignedToUserViewModels
		};


		return View(projectDetailViewModel);
	}


	[Authorize(Roles = "manager")]
	[HttpGet]
	public IActionResult Create()
	{
		return View();
	}

	[Authorize(Roles = "manager")]
	[HttpPost]
	public async Task<IActionResult> Create(CreateProjectViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		string slug = Slugify(model.Name);

		Project project = new()
		{
			Name = model.Name,
			StartDate = model.StartDate.ToUniversalTime(),
			TargetEndDate = model.TargetEndDate.ToUniversalTime(),
			CreatedBy = User.Identity.Name,
			Slug = slug
		};

		await projectsService.CreateProjectAsync(project);

		return RedirectToAction("index", "projects");
	}



	[Authorize(Roles = "manager")]
	[HttpGet]
	public async Task<IActionResult> EditUsersInProject(string projectId)
	{

		var project = await projectsService.GetProjectByIdWithUsersAsync(Guid.Parse(projectId));

		if (project == null)
		{
			ViewBag.Error = $"No Project found with that Id {projectId}";
			return View("NotFound");
		}

		ViewBag.projectId = projectId;
		ViewBag.projectSlug = project.Slug;


		List<EditUserInProjectViewModel> model = new();


		foreach (var user in await userManager.Users.ToListAsync())
		{
			EditUserInProjectViewModel editUserInProjectViewModel = new()
			{
				UserId = user.Id,
				Email = user.Email,
				Image = user.Image,
				Roles = (await userManager.GetRolesAsync(user)).ToList()
			};

			if (project.AssignedTo.Contains(user))
			{
				editUserInProjectViewModel.IsSelected = true;
			}
			else
			{
				editUserInProjectViewModel.IsSelected = false;
			}

			model.Add(editUserInProjectViewModel);
		}

		return View(model);
	}


	[Authorize(Roles = "manager")]
	[HttpPost]
	public async Task<IActionResult> EditUsersInProject(List<EditUserInProjectViewModel> model, string projectId)
	{

		if (!ModelState.IsValid) return View(model);


		var project = await projectsService.GetProjectByIdWithUsersAsync(Guid.Parse(projectId));

		if (project == null)
		{
			ViewBag.Error = $"No Project found with that Id {projectId}";
			return View("NotFound");
		}


		foreach (EditUserInProjectViewModel editUserInProjectViewModel in model)
		{
			var user = await userManager.FindByIdAsync(editUserInProjectViewModel.UserId);

			if (editUserInProjectViewModel.IsSelected && !(project.AssignedTo.Contains(user)))
			{
				await projectsService.AssignUser(user, project.Id);
			}
			else if (!editUserInProjectViewModel.IsSelected && project.AssignedTo.Contains(user))
			{
				await projectsService.UnAssignUser(user, project.Id);
			}
		}

		return RedirectToAction("project", "projects", new { id = project.Slug });
	}




	[Authorize(Roles = "manager")]
	[HttpPost]
	public async Task<IActionResult> Delete(string id)
	{
		if (!ModelState.IsValid) return View();


		var project = await projectsService.GetProjectByIdAsync(Guid.Parse(id));
		if (project is null)
		{
			ViewBag.Error = $"No Project found with this id {id}";
			return View("NotFound");
		}


		await projectsService.DeleteProjectByIdAsync(project.Id);


		return RedirectToAction("index", "projects");
	}

	[Authorize(Roles = "manager")]
	[HttpGet]
	public async Task<IActionResult> Edit(string id)
	{
		var project = await projectsService.GetProjectByIdAsync(Guid.Parse(id));
		if (project is null)
		{
			ViewBag.Error = $"No Project found with this id {id}.";
			return View("NotFound");
		}


		EditProjectViewModel editProjectViewModel = new()
		{
			Id = project.Id.ToString(),
			Name = project.Name,
			Slug = project.Slug,
			StartDate = project.StartDate,
			TargetEndDate = project.TargetEndDate,
			ActualEndDate = project.ActualEndDate
		};

		return View(editProjectViewModel);
	}


	[Authorize(Roles = "manager")]
	[HttpPost]
	public async Task<IActionResult> Edit(EditProjectViewModel model, string id)
	{
		if (!ModelState.IsValid) return View(model);
		var project = await projectsService.GetProjectByIdAsync(Guid.Parse(id));

		if (project is null)
		{
			ViewBag.Error = $"No Project found with this Id {id}";
			return View("NotFound");
		}

		project.Name = model.Name;
		project.Slug = Slugify(model.Name);
		project.StartDate = model.StartDate.ToUniversalTime();
		project.TargetEndDate = model.TargetEndDate.ToUniversalTime();
		project.ActualEndDate = model.ActualEndDate.ToUniversalTime();

		await projectsService.UpdateProjectByIdAsync(project.Id, project);

		return RedirectToAction("project", "projects", new { id = project.Slug });
	}


	private string Slugify(string name)
	{
		SlugHelper helper = new();
		string slug = helper.GenerateSlug(name);
		return slug;
	}
}