using AutoMapper;
using issuetracker.Dtos;
using issuetracker.Entities;
using issuetracker.Services;
using issuetracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slugify;

namespace issuetracker.Api.Controllers;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
	private readonly IProjectsService projectsService;
	private readonly UserManager<AppUser> userManager;
	private readonly IMapper mapper;

	public ProjectsController(IProjectsService projectsService, UserManager<AppUser> userManager, IMapper mapper)
	{
		this.projectsService = projectsService;
		this.userManager = userManager;
		this.mapper = mapper;
	}


	[Authorize(Roles = "manager")]
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Project>>> GetAllProjects()
	{
		var projects = await projectsService.GetAllProjectsAsync();

		IEnumerable<ProjectDto> model = mapper.Map<IEnumerable<ProjectDto>>(projects);

		return Ok(model);
	}



	[HttpGet("{id}")]
	public async Task<IActionResult> GetProjectById(string id)
	{
		var project = await projectsService.GetProjectByIdWithUsersAsync(Guid.Parse(id));

		if (project is null)
		{
			return Problem(
				detail: "No project found with that ID.",
				statusCode: StatusCodes.Status404NotFound);
		}


		var projectDto = mapper.Map<ProjectDto>(project);

		foreach (var user in projectDto.AssignedTo)
		{
			AppUser appUser = mapper.Map<AppUser>(user);
			var roles = (await userManager.GetRolesAsync(appUser)).ToList();
			user.Roles = roles;
		}

		return Ok(projectDto);
	}

	[Authorize(Roles = "manager")]
	[HttpPost]
	public async Task<IActionResult> CreateProject(CreateProjectDto model)
	{
		Project project = mapper.Map<Project>(model);
		project.Slug = Slugify(project.Name);
		project.CreatedBy = User.Identity.Name;
		project.StartDate = project.StartDate.ToUniversalTime();
		project.TargetEndDate = project.TargetEndDate.ToUniversalTime();

		try
		{
			await projectsService.CreateProjectAsync(project);
		}
		catch (DbUpdateException)
		{
			return Problem(
				detail: $"A Project with the name {project.Name} already exist.",
				statusCode: StatusCodes.Status400BadRequest);
		}

		return CreatedAtAction(
			nameof(GetProjectById),
			new { id = project.Id },
			mapper.Map<ProjectDto>(project));
	}


	[Authorize(Roles = "manager")]
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteProjectById(string id)
	{
		Project project = await projectsService.GetProjectByIdAsync(Guid.Parse(id));

		if (project is null)
		{
			return Problem(
				detail: $"No Project found With this Id '{id}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		await projectsService.DeleteProjectByIdAsync(project.Id);

		return NoContent();
	}


	private string Slugify(string name)
	{
		SlugHelper helper = new();
		string slug = helper.GenerateSlug(name);
		return slug;
	}
}