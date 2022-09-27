using issuetracker.Entities;
using issuetracker.Services;
using issuetracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Slugify;

namespace issuetracker.mvc.Controllers;

[Authorize]
public class ProjectsController : Controller
{
	private readonly IProjectsService projectsService;
	private readonly UserManager<AppUser> userManager;
	public ProjectsController(IProjectsService projectsService, UserManager<AppUser> userManager)
	{
		this.projectsService = projectsService;

		this.userManager = userManager;
	}

	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var projects = await projectsService.GetAllProjectsAsync();
		return View(projects);
	}


	[HttpGet]
	public async Task<IActionResult> Project(string id)
	{
		Project project = await projectsService.GetOneProjectAsync(slug: id);

		List<Issue> unAssignedIssues = new();
		List<Issue> overDueIssues = new();
		List<Issue> openIssues = new();
		List<Issue> closedIssues = new();

		foreach (var issue in project.Issues)
		{
			if (issue.AssignedTo.Count() == 0)
			{
				unAssignedIssues.Add(issue);
			}

			if (issue.TargetResolutionDate > DateTime.UtcNow)
			{
				overDueIssues.Add(issue);
			}

			if (issue.Status == Status.Open)
			{
				openIssues.Add(issue);
			}

			if (issue.Status == Status.Closed)
			{
				closedIssues.Add(issue);
			}

		}


		List<AssignedToUserViewModel> assignedToUserViewModels = new();

		foreach (var user in project.AssignedTo)
		{
			var roles = await userManager.GetRolesAsync(user);

			AssignedToUserViewModel assignedToUserViewModel = new()
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



	[HttpGet]
	public IActionResult Create()
	{
		return View();
	}


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



	private string Slugify(string name)
	{
		SlugHelper helper = new();
		string slug = helper.GenerateSlug(name);
		return slug;
	}
}