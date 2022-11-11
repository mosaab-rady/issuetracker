using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using issuetracker.mvc.Models;
using issuetracker.Services;
using Microsoft.AspNetCore.Authorization;
using issuetracker.ViewModels;
using issuetracker.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace issuetracker.mvc.Controllers;

[Authorize]
public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;

	private readonly IIssuesService issuesService;
	private readonly IProjectsService projectsService;

	public HomeController(ILogger<HomeController> logger,
											 IProjectsService projectsService,
											 IIssuesService issuesService
											)
	{
		_logger = logger;
		this.issuesService = issuesService;
		this.projectsService = projectsService;
	}

	[Authorize(Roles = "manager")]
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

			issue.AssignedTo = await issuesService.GetIssueUsersAsync(issue.Id);

			if (issue.AssignedTo.Count() == 0)
			{
				model.UnAssignedIssues.Add(issueViewModel);
			}

			if (issue.TargetResolutionDate < DateTime.UtcNow && issue.Status == Status.Open)
			{
				model.OverDueIssues.Add(issueViewModel);
			}

			if (DateTime.UtcNow < issue.CreatedOn.AddDays(7))
			{
				model.RecentlyOpenedIssues.Add(issueViewModel);
			}

		}




		foreach (var project in await projectsService.GetAllProjectsAsync())
		{
			project.Issues = await projectsService.GetIssuesInProjectAsync(project.Id);

			ProjectOverviewViewModel projectOverviewViewModel = new()
			{
				ProjectID = project.Id.ToString(),
				ProjectName = project.Name,
				Slug = project.Slug,
				TargetEndDate = project.TargetEndDate,
				ActaulEndDate = project.ActualEndDate,

				ClosedIssues = (from issue in project.Issues
												where issue.Status == Status.Closed
												select issue).Count(),

				OpenIssues = (from issue in project.Issues
											where issue.Status == Status.Open
											select issue).Count(),

				UnAssignedIssues = (from issue in project.Issues
														where issue.AssignedTo.Count == 0
														select issue).Count(),

				OverdueIssues = (from issue in project.Issues
												 where issue.TargetResolutionDate < DateTime.UtcNow &&
												 issue.Status == Status.Open
												 select issue).Count(),
			};

			model.Projects.Add(projectOverviewViewModel);
		}

		return View(model);
	}

}
