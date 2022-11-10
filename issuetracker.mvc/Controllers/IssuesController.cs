using issuetracker.Email;
using issuetracker.Entities;
using issuetracker.Hubs;
using issuetracker.Services;
using issuetracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace issuetracker.mvc.Controllers;

[Authorize]
public class IssuesController : Controller
{
	private readonly IIssuesService issuesService;
	private readonly IProjectsService projectsService;
	private readonly ITagsServices tagsServices;
	private readonly IPriorityService priorityService;
	private readonly IcommentsService commentsService;
	private readonly UserManager<AppUser> userManager;

	private readonly IHubContext<ChatHub> hubContext;

	private readonly IEmailSender emailSender;

	private readonly IConfiguration configuration;
	public IssuesController(IIssuesService issuesService,
												 IProjectsService projectsService,
												 ITagsServices tagsServices,
												 UserManager<AppUser> userManager,
												 IPriorityService priorityService,
												 IcommentsService commentsService,
												 IHubContext<ChatHub> hubContext,
												 IConfiguration configuration,
												 IEmailSender emailSender
												 )
	{
		this.issuesService = issuesService;
		this.projectsService = projectsService;
		this.tagsServices = tagsServices;
		this.userManager = userManager;
		this.priorityService = priorityService;
		this.commentsService = commentsService;
		this.hubContext = hubContext;
		this.configuration = configuration;
		this.emailSender = emailSender;
	}

	[Authorize(Roles = "manager")]
	[HttpGet]
	public async Task<IActionResult> Index(string project, string priority, string status)
	{

		AllIsuesViewModel model = new()
		{
			Project = project,
			Priority = priority,
			Status = status,
			Statuses = new List<string>() { "Open", "Closed" }
		};

		List<IssueViewModel> issueViewModels = new();


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



			issueViewModels.Add(issueViewModel);
		}


		model.Projects = from _project in await projectsService.GetAllProjectsAsync()
										 select _project.Name;
		model.Priorities = from _priority in await priorityService.GetAllPrioritiesAsync()
											 select _priority.Name;

		// filters
		if (project != null)
		{
			issueViewModels = issueViewModels.FindAll(x => x.ProjectName == project);
		}
		if (priority != null)
		{
			issueViewModels = issueViewModels.FindAll(x => x.Priority.Name == priority);
		}
		if (status != null)
		{
			issueViewModels = issueViewModels.FindAll(x => x.Status == status);
		}



		model.issueViewModels = issueViewModels;

		return View(model);
	}

	[HttpGet]
	public async Task<IActionResult> Issue(string id)
	{

		var issue = await issuesService.GetIssueByIdWithUsersAsync(Guid.Parse(id));

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
	public async Task<IActionResult> AssignedToMe()
	{
		var user = await userManager.Users
		.Include(user => user.AssignedIssues)
			.ThenInclude(issue => issue.Priority)
		.Include(user => user.AssignedIssues)
			.ThenInclude(issue => issue.Project)
		.SingleOrDefaultAsync(user => user.Email == User.Identity.Name);

		if (user is null)
		{
			ViewBag.Error = "somthing went wrong, please log out and try logging in again.";
			return View("NotFound");
		}

		AssignedToMeViewModel model = new();

		foreach (var issue in user.AssignedIssues)
		{
			IssueViewModel issueViewModel = new()
			{
				IssueId = issue.Id.ToString(),
				Title = issue.Title,
				ProjectName = issue.Project.Name,
				Status = Enum.GetName(typeof(Status), issue.Status),
				Priority = issue.Priority ?? new Priority() { Color = "#6699ff", Name = "Not Set" },
			};

			if (issue.TargetResolutionDate < DateTime.UtcNow && issue.Status == Status.Open)
			{
				model.OverDueIssues.Add(issueViewModel);
			}

			if (issue.Status == Status.Open)
			{
				model.OpenIssues.Add(issueViewModel);
			}
		}



		return View(model);
	}


	[HttpGet]
	public async Task<IActionResult> ReportedByMe()
	{
		var issues = await issuesService.GetIssuesReportedByUser(User.Identity.Name);
		List<IssueViewModel> model = new();

		foreach (var issue in issues)
		{
			IssueViewModel issueViewModel = new()
			{
				IssueId = issue.Id.ToString(),
				Title = issue.Title,
				ProjectName = issue.Project.Name,
				Status = Enum.GetName(typeof(Status), issue.Status),
				Priority = issue.Priority ?? new Priority() { Color = "#6699ff", Name = "Not Set" }
			};

			model.Add(issueViewModel);
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

		// send email to users that have a role of "manager"
		IEnumerable<AppUser> managerUsers = await userManager.GetUsersInRoleAsync("manager");

		string issueUrl = $"{Request.Scheme}://{Request.Host}/issues/issue/{issue.Id}";

		foreach (var manager in managerUsers)
		{
			await emailSender.SendIssueCreatedAsync(manager.Email, issueUrl, project.Name);
		}

		return RedirectToAction("index", "issues");
	}

	[Authorize(Roles = "manager, lead")]
	[HttpGet]
	public async Task<IActionResult> EditUsersInIssue(string issueId)
	{
		var issue = await issuesService.GetIssueByIdWithUsersAsync(Guid.Parse(issueId));

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

	[Authorize(Roles = "manager, lead")]
	[HttpPost]
	public async Task<IActionResult> EditUsersInIssue(List<EditUserInIssueViewModel> model, string issueId)
	{
		if (!ModelState.IsValid) return View(model);

		var issue = await issuesService.GetIssueByIdWithUsersAsync(Guid.Parse(issueId));

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
					// send email to the assigned user
					string issueUrl = $"{Request.Scheme}://{Request.Host}/issues/issue/{issue.Id}";
					await emailSender.SendAssignToIssueAsync(user.Email, issueUrl, issue.Title, project.Name, User.Identity.Name, User.Identity.Name);
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

	[Authorize(Roles = "manager")]
	[HttpPost]
	public async Task<IActionResult> DeleteIssue(string id)
	{
		var issue = await issuesService.GetIssueByIdWithUsersAsync(Guid.Parse(id));
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
		var issue = await issuesService.GetIssueByIdWithUsersAsync(Guid.Parse(id));
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

		var issue = await issuesService.GetIssueByIdWithUsersAsync(Guid.Parse(id));
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

	[Authorize(Roles = "manager, lead")]
	[HttpGet]
	public async Task<IActionResult> Edit(string id)
	{
		var issue = await issuesService.GetIssueByIdWithUsersAsync(Guid.Parse(id));
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
			Priorities = (await priorityService.GetAllPrioritiesAsync()).ToList(),
			ResolutionSummary = issue.ResoliotionSummary
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

	[Authorize(Roles = "manager, lead")]
	[HttpPost]
	public async Task<IActionResult> Edit(EditIssueViewModel model, string id)
	{
		if (!ModelState.IsValid) return View(model);

		var issue = await issuesService.GetIssueByIdWithUsersAsync(Guid.Parse(id));

		if (issue == null)
		{
			ViewBag.Error = $"No Issue found with this ID {id}";
			return View("NotFound");
		}


		issue.Title = model.Title;
		issue.Description = model.Description;
		issue.TargetResolutionDate = model.TargetResolutionDate.ToUniversalTime();
		issue.ResoliotionSummary = model.ResolutionSummary;

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



	[HttpGet]
	public async Task<IActionResult> Comments(string issueId)
	{
		var issue = await issuesService.GetIssueByIdWithUsersAsync(Guid.Parse(issueId));

		if (issue == null)
		{
			ViewBag.Error = $"No issue found with this Id {issueId}.";
			return View("NotFound");
		}


		var comments = await commentsService.GetAllCommentsOnIssue(issue.Id);

		CommentsOnIssueViewModel model = new();

		foreach (var comment in comments)
		{
			CommentViewModel commentViewModel = new()
			{
				CommentText = comment.CommentText,
				CreatedOn = ConvertDate(comment.CreatedOn),
				UserName = $"{comment.User.FirstName} {comment.User.LastName}",
				UserImage = comment.User.Image,

			};
			model.Comments.Add(commentViewModel);
		}

		model.IssueDescription = issue.Description;
		model.IssueTitle = issue.Title;

		ViewBag.issueId = issue.Id;

		return View(model);
	}


	[HttpPost]
	public async Task<IActionResult> CreateComment([FromBody] CreateCommentViewModel model, string issueId)
	{

		if (!ModelState.IsValid) return View("comments");

		AppUser user = await userManager.FindByEmailAsync(User.Identity.Name);

		if (user == null)
		{
			ViewBag.Error = $"No user found, login and try again.";
			return View("NotFound");
		}

		Issue issue = await issuesService.GetIssueByIdWithUsersAsync(Guid.Parse(issueId));
		if (user == null)
		{
			ViewBag.Error = $"No Issue found with this Id {issueId}.";
			return View("NotFound");
		}

		Comment comment = new()
		{
			User = user,
			Issue = issue,
			CommentText = model.Comment
		};

		await commentsService.CreateComment(comment);

		await hubContext.Clients.Group(issueId).SendAsync(
			"ReceiveMessage",
			comment.CommentText,
			ConvertDate(comment.CreatedOn),
			$"{user.FirstName} {user.LastName}",
			user.Image
			);



		return Ok();
	}



	private string ConvertDate(DateTime date)
	{
		const int SECOND = 1;
		const int MINUTE = 60 * SECOND;
		const int HOUR = 60 * MINUTE;
		const int DAY = 24 * HOUR;
		const int MONTH = 30 * DAY;

		var ts = new TimeSpan(DateTime.UtcNow.Ticks - date.Ticks);
		double delta = Math.Abs(ts.TotalSeconds);

		if (delta < 1 * MINUTE)
			return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

		if (delta < 2 * MINUTE)
			return "a minute ago";

		if (delta < 45 * MINUTE)
			return ts.Minutes + " minutes ago";

		if (delta < 90 * MINUTE)
			return "an hour ago";

		if (delta < 24 * HOUR)
			return ts.Hours + " hours ago";

		if (delta < 48 * HOUR)
			return "yesterday";

		if (delta < 30 * DAY)
			return ts.Days + " days ago";

		if (delta < 12 * MONTH)
		{
			int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
			return months <= 1 ? "one month ago" : months + " months ago";
		}
		else
		{
			int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
			return years <= 1 ? "one year ago" : years + " years ago";
		}
	}



}