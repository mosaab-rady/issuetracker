using issuetracker.Entities;
using issuetracker.Services;
using issuetracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace issuetracker.mvc.Controllers;

[Authorize]
public class IssuesController : Controller
{
	private readonly IIssuesService issuesService;
	private readonly IProjectsService projectsService;
	private readonly ITagsServices tagsServices;

	public IssuesController(IIssuesService issuesService, IProjectsService projectsService, ITagsServices tagsServices)
	{
		this.issuesService = issuesService;
		this.projectsService = projectsService;
		this.tagsServices = tagsServices;
	}

	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var issues = await issuesService.GetAllIssuesAsync();
		return View(issues);
	}

	[HttpGet]
	public async Task<IActionResult> Issue(string id)
	{
		var issue = await issuesService.GetIssueByIdAsync(Guid.Parse(id));
		return View(issue);
	}

	[HttpGet]
	public async Task<IActionResult> Create()
	{
		CreateIssueViewModel model = new();

		foreach (var proj in await projectsService.GetAllProjectsAsync())
		{
			AssignProjectViewModel assignProjectViewModel = new()
			{
				Id = proj.Id.ToString(),
				Name = proj.Name
			};

			model.projects.Add(assignProjectViewModel);
		}

		foreach (var tag in await tagsServices.GetAllTagsAsync())
		{
			AssignTagViewModel assignTagViewModel = new()
			{
				Name = tag.Name,
				Color = tag.Color
			};

			model.Tags.Add(assignTagViewModel);
		}

		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> Create(CreateIssueViewModel model)
	{
		if (!ModelState.IsValid) return View(model);

		Issue issue = new()
		{
			Title = model.Title,
			Description = model.Description,
			CreatedBy = User.Identity.Name,
			Tags = new List<Tag>(),
			Status = Status.Open,
		};

		var project = await projectsService.GetProjectByIdAsync(Guid.Parse(model.ProjectId));

		if (project == null)
		{
			ModelState.AddModelError("", "The project you selected does not exist.");
			return View(model);
		}

		issue.Project = project;


		foreach (var tag in model.Tags)
		{
			var _tag = await tagsServices.GetTagByIdAsync(Guid.Parse(tag.TagId));
			if (_tag == null)
			{
				ModelState.AddModelError("", $"No tag with that name {tag.Name}");
				return View(model);
			}

			if (tag.IsSelected)
			{
				issue.Tags.Add(_tag);
			}
		}


		await issuesService.CreateIssueAsync(issue);

		return RedirectToAction("index", "issues");
	}

}