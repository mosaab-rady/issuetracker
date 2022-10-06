using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using issuetracker.mvc.Models;
using issuetracker.Services;
using Microsoft.AspNetCore.Authorization;
using issuetracker.ViewModels;
using issuetracker.Entities;

namespace issuetracker.mvc.Controllers;

[Authorize]
public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;

	private readonly IIssuesService issuesService;

	public HomeController(ILogger<HomeController> logger, IProjectsService projectsService, IIssuesService issuesService)
	{
		_logger = logger;
		this.issuesService = issuesService;
	}

	public async Task<IActionResult> Index()
	{
		DashboardViewModel model = new();

		foreach (var issue in await issuesService.GetAllIssuesAsync())
		{
			IssueViewModel issueViewModel = new()
			{
				IssueId = issue.Id.ToString(),
				Priority = issue.Priority ?? new Priority() { Color = "#6699ff", Name = "Not Set" },
				Status = Enum.GetName(typeof(Status), issue.Status),
				ProjectName = issue.Project.Name,
				Title = issue.Title
			};

			if (issue.AssignedTo.Count() == 0)
			{
				model.UnAssignedIssues.Add(issueViewModel);
			}

			if (issue.TargetResolutionDate < DateTime.UtcNow && issue.Status == Status.Open)
			{
				model.OverDueIssues.Add(issueViewModel);
			}

			if (DateTime.UtcNow < issue.CreatedOn.AddDays(5))
			{
				model.RecentlyOpenedIssues.Add(issueViewModel);
			}

			if (issue.Status == Status.Closed)
			{
				model.ClosedIssues.Add(issueViewModel);
			}

		}

		return View(model);
	}


	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
