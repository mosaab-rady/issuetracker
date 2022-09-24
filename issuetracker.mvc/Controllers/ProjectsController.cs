using issuetracker.Entities;
using issuetracker.Services;
using issuetracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Slugify;

namespace issuetracker.mvc.Controllers;

public class ProjectsController : Controller
{
	private readonly IProjectsService projectsService;
	public ProjectsController(IProjectsService projectsService)
	{
		this.projectsService = projectsService;
	}

	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var projects = await projectsService.GetAllProjectsAsync();
		return View(projects);
	}

	[HttpGet]
	public IActionResult Create()
	{
		CreateProjectViewModel model = new();
		return View(model);
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