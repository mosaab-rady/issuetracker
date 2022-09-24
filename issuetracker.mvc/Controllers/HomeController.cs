using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using issuetracker.mvc.Models;
using issuetracker.Services;

namespace issuetracker.mvc.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly IProjectsService service;

	public HomeController(ILogger<HomeController> logger, IProjectsService service)
	{
		_logger = logger;
		this.service = service;
	}

	public async Task<IActionResult> Index()
	{
		var projects = await service.GetAllProjectsAsync();
		return View(projects);
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
