using issuetracker.Entities;
using issuetracker.Services;
using issuetracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace issuetracker.mvc.Controllers;

[Authorize]
public class IssuesController : Controller
{
	private readonly IIssuesService issuesService;
	private readonly IProjectsService projectsService;
	private readonly ITagsServices tagsServices;
	private readonly IPriorityService priorityService;
	private readonly UserManager<AppUser> userManager;

	public IssuesController(IIssuesService issuesService, IProjectsService projectsService, ITagsServices tagsServices, UserManager<AppUser> userManager, IPriorityService priorityService)
	{
		this.issuesService = issuesService;
		this.projectsService = projectsService;
		this.tagsServices = tagsServices;
		this.userManager = userManager;
		this.priorityService = priorityService;
	}

	[HttpGet]
	public async Task<IActionResult> Index()
	{
		List<IssueViewModel> model = new();


		foreach (var issue in await issuesService.GetAllIssuesAsync())
		{


			IssueViewModel issueViewModel = new()
			{
				IssueId = issue.Id.ToString(),
				Status = Enum.GetName(typeof(Status), issue.Status),
				ProjectName = issue.Project.Name,
				Title = issue.Title,
				Priority = issue.Priority ?? new Priority() { Color = "#6699ff", Name = "Not Set" }
			};


			model.Add(issueViewModel);
		}
		return View(model);
	}

	[HttpGet]
	public async Task<IActionResult> Issue(string id)
	{

		var issue = await issuesService.GetIssueByIdAsync(Guid.Parse(id));

		if (issue == null)
		{
			ViewBag.Error = $"No Issue found with that ID {id}.";
			return View("NotFound");
		}

		IssueDetailViewModel model = new()
		{
			IssueId = issue.Id.ToString(),
			Title = issue.Title,
			Description = issue.Description,
			CreatedBy = issue.CreatedBy,
			CreatedOn = issue.CreatedOn,
			TargetResolutionDate = issue.TargetResolutionDate,
			ActualResolutionDate = issue.ActualResolutionDate,
			ProjectName = issue.Project.Name,
			Status = Enum.GetName(typeof(Status), issue.Status),
			Priority = issue.Priority ?? new Priority() { Color = "#6699ff", Name = "Not Set" },
			ResolutionSummary = issue.ResoliotionSummary,
			Tags = issue.Tags
		};

		foreach (var user in issue.AssignedTo)
		{
			var roles = await userManager.GetRolesAsync(user);
			AssignUserViewModel assignedToUserViewModel = new()
			{
				Id = user.Id,
				Email = user.Email,
				Image = user.Image,
				Name = $"{user.FirstName} {user.LastName}",
				Roles = roles.ToList()
			};
			model.AssignedTo.Add(assignedToUserViewModel);
		}



		return View(model);
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
				TagId = tag.Id.ToString(),
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
			TargetResolutionDate = DateTime.UtcNow.AddYears(1)
		};

		var project = await projectsService.GetProjectByIdAsync(Guid.Parse(model.ProjectId));

		if (project == null)
		{
			ModelState.AddModelError("", "The project you selected does not exist.");
			return View(model);
		}

		var user = await userManager.Users.Include(x => x.AssignedProjects).SingleOrDefaultAsync(x => x.Email == User.Identity.Name);

		if (!user.AssignedProjects.Contains(project))
		{
			ModelState.AddModelError("ProjectId", "sorry, you cant create issue in this project. you can only create issue in projects you are assigned to.");
			return View(model);
		}

		issue.Project = project;



		// if the user create new tag
		if (model.CreateTagViewModel.Name != null)
		{
			Tag newTag = new()
			{
				Name = model.CreateTagViewModel.Name,
				Color = model.CreateTagViewModel.Color
			};
			newTag = await tagsServices.CreateTagAsync(newTag);
			issue.Tags.Add(newTag);
		}


		// if the user selected from existings tags
		foreach (var tag in model.Tags)
		{
			var _tag = await tagsServices.GetTagByIdAsync(Guid.Parse(tag.TagId));
			if (_tag == null)
			{
				ViewBag.Error = $"No Tag found with this ID {tag.TagId}.";
				return View("NotFound");
			}

			if (tag.IsSelected)
			{
				issue.Tags.Add(_tag);
			}
		}


		await issuesService.CreateIssueAsync(issue);

		return RedirectToAction("index", "issues");
	}


	[HttpGet]
	public async Task<IActionResult> EditUsersInIssue(string issueId)
	{
		var issue = await issuesService.GetIssueByIdAsync(Guid.Parse(issueId));

		if (issue == null)
		{
			ViewBag.Error = $"No Issue found with this ID {issueId}.";
			return View("NotFound");
		}

		ViewBag.issueId = issue.Id;



		List<EditUserInIssueViewModel> model = new();

		var project = issue.Project;

		foreach (var user in await userManager.Users.Include(x => x.AssignedProjects).ToListAsync())
		{
			if (user.AssignedProjects.Contains(project))
			{
				EditUserInIssueViewModel editUserInIssueViewModel = new()
				{
					UserId = user.Id,
					Email = user.Email,
					Image = user.Image,
					Roles = (await userManager.GetRolesAsync(user)).ToList()
				};

				if (issue.AssignedTo.Contains(user))
				{
					editUserInIssueViewModel.IsSelected = true;
				}
				else
				{
					editUserInIssueViewModel.IsSelected = false;
				}

				model.Add(editUserInIssueViewModel);
			}
		}

		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> EditUsersInIssue(List<EditUserInIssueViewModel> model, string issueId)
	{
		if (!ModelState.IsValid) return View(model);

		var issue = await issuesService.GetIssueByIdAsync(Guid.Parse(issueId));

		if (issue == null)
		{
			ViewBag.Error = $"No Issue found with this ID {issueId}.";
			return View("NotFound");
		}


		var project = issue.Project;

		foreach (EditUserInIssueViewModel editUserInIssueViewModel in model)
		{
			var user = await userManager.Users.Include(x => x.AssignedProjects).SingleOrDefaultAsync(x => x.Id == editUserInIssueViewModel.UserId);

			if (user.AssignedProjects.Contains(project))
			{

				if (editUserInIssueViewModel.IsSelected && !(issue.AssignedTo.Contains(user)))
				{
					await issuesService.AssignUser(user, issue.Id);
				}
				else if (!editUserInIssueViewModel.IsSelected && issue.AssignedTo.Contains(user))
				{
					await issuesService.UnAssignUser(user, issue.Id);
				}

			}
			else
			{
				ModelState.AddModelError("", "The user you selected is not assigned to this project.");
				return View(model);
			}
		}

		return RedirectToAction("issue", "issues", new { id = issue.Id });

	}


	[HttpPost]
	public async Task<IActionResult> DeleteIssue(string id)
	{
		var issue = await issuesService.GetIssueByIdAsync(Guid.Parse(id));
		if (issue == null)
		{
			ViewBag.Error = $"No Issue found with this ID {id}."; ;
			return View("NotFound");
		}

		await issuesService.DeleteIssueAsync(issue.Id);

		return RedirectToAction("index", "issues");
	}


	[HttpGet]
	public async Task<IActionResult> CloseIssue(string id)
	{
		var issue = await issuesService.GetIssueByIdAsync(Guid.Parse(id));
		if (issue == null)
		{
			ViewBag.Error = $"No Issue found with this ID {id}";
			return View("NotFound");
		}


		CloseIssueViewModel closeIssueViewModel = new()
		{
			IssueId = issue.Id.ToString(),
			Title = issue.Title
		};


		return View(closeIssueViewModel);
	}


	[HttpPost]
	public async Task<IActionResult> CloseIssue(CloseIssueViewModel model, string id)
	{
		if (!ModelState.IsValid) return View(model);

		var issue = await issuesService.GetIssueByIdAsync(Guid.Parse(id));
		if (issue == null)
		{
			ViewBag.Error = $"No Issue found with this ID {id}";
			return View("NotFound");
		}


		issue.ResoliotionSummary = model.ResolutionSummary;
		issue.Status = Status.Closed;
		issue.ActualResolutionDate = DateTime.UtcNow;

		await issuesService.UpdateIssueByIdAsync(issue.Id, issue);

		return RedirectToAction("issue", "issues", new { id = issue.Id });
	}


	[HttpGet]
	public async Task<IActionResult> Edit(string id)
	{
		var issue = await issuesService.GetIssueByIdAsync(Guid.Parse(id));
		if (issue == null)
		{
			ViewBag.Error = $"No Issue found with this ID {id}.";
			return View("NotFound");
		}

		EditIssueViewModel editIssueViewModel = new()
		{
			IssueId = issue.Id.ToString(),
			Title = issue.Title,
			Description = issue.Description,
			TargetResolutionDate = issue.TargetResolutionDate,
			Priorities = (await priorityService.GetAllPrioritiesAsync()).ToList()
		};

		foreach (var tag in await tagsServices.GetAllTagsAsync())
		{
			AssignTagViewModel assignTagViewModel = new()
			{
				Name = tag.Name,
				Color = tag.Color,
				TagId = tag.Id.ToString()
			};

			if (issue.Tags.Contains(tag))
			{
				assignTagViewModel.IsSelected = true;
			}
			else
			{
				assignTagViewModel.IsSelected = false;
			}

			editIssueViewModel.Tags.Add(assignTagViewModel);
		}

		if (issue.Priority != null)
		{
			editIssueViewModel.PriorityId = issue.Priority.Id.ToString();
		}


		return View(editIssueViewModel);
	}


	[HttpPost]
	public async Task<IActionResult> Edit(EditIssueViewModel model, string id)
	{
		if (!ModelState.IsValid) return View(model);

		var issue = await issuesService.GetIssueByIdAsync(Guid.Parse(id));

		if (issue == null)
		{
			ViewBag.Error = $"No Issue found with this ID {id}";
			return View("NotFound");
		}


		issue.Title = model.Title;
		issue.Description = model.Description;
		issue.TargetResolutionDate = model.TargetResolutionDate.ToUniversalTime();

		if (model.CreateTagViewModel.Name != null)
		{
			Tag newTag = new()
			{
				Name = model.CreateTagViewModel.Name,
				Color = model.CreateTagViewModel.Color
			};
			newTag = await tagsServices.CreateTagAsync(newTag);
			issue.Tags.Add(newTag);
		}

		foreach (var tag in model.Tags)
		{
			var _tag = await tagsServices.GetTagByIdAsync(Guid.Parse(tag.TagId));

			if (tag.IsSelected && !issue.Tags.Contains(_tag))
			{
				issue.Tags.Add(_tag);
			}
			else if (!tag.IsSelected && issue.Tags.Contains(_tag))
			{
				issue.Tags.Remove(_tag);
			}
		}

		if (model.CreatePriorityViewModel.Name != null)
		{
			Priority newPriority = new()
			{
				Name = model.CreatePriorityViewModel.Name,
				Color = model.CreatePriorityViewModel.Color
			};

			newPriority = await priorityService.CreatePriorityAsync(newPriority);
			issue.Priority = newPriority;
		}
		else
		{
			var priority = await priorityService.GetPriorityByIdAsync(Guid.Parse(model.PriorityId));
			issue.Priority = priority;
		}



		await issuesService.UpdateIssueByIdAsync(issue.Id, issue);

		return RedirectToAction("issue", "issues", new { id = issue.Id });
	}







}