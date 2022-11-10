using AutoMapper;
using issuetracker.Dtos;
using issuetracker.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace issuetracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "manager")]
public class UsersController : ControllerBase
{
	private readonly UserManager<AppUser> userManager;
	private readonly RoleManager<IdentityRole> roleManager;
	private readonly IMapper mapper;
	public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
	{
		this.userManager = userManager;
		this.roleManager = roleManager;
		this.mapper = mapper;
	}


	// 1) get all users
	[HttpGet]
	public async Task<IActionResult> GetAllUsers()
	{
		List<AppUser> users = await userManager.Users.ToListAsync();

		List<UserDto> usersDto = mapper.Map<List<UserDto>>(users);

		foreach (var user in usersDto)
		{
			user.Roles = (await userManager.GetRolesAsync(mapper.Map<AppUser>(user))).ToList();
		}

		return Ok(usersDto);
	}


	// 2) get user by Id
	[HttpGet("{id}")]
	public async Task<IActionResult> GetUserById(string id)
	{
		AppUser user = await userManager.FindByIdAsync(id);

		if (user is null)
		{
			return Problem(
				detail: $"No user found with this Id '{id}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		UserDto userDto = mapper.Map<UserDto>(user);
		userDto.Roles = (await userManager.GetRolesAsync(user)).ToList();

		return Ok(userDto);
	}


	// 3) get issues assigned to user
	[HttpGet("{userId}/issues")]
	public async Task<IActionResult> GetIssuesAssignedToUser(string userId)
	{
		AppUser appUser = await userManager.Users
		.Include(user => user.AssignedIssues)
		.ThenInclude(issue => issue.Priority)
		.Include(user => user.AssignedIssues)
		.ThenInclude(issue => issue.Project)
		.SingleOrDefaultAsync(user => user.Id == userId);

		if (appUser is null)
		{
			return Problem(
				detail: $"No user found with this Id '{userId}'.",
				statusCode: StatusCodes.Status404NotFound);
		}



		List<IssueDto> issueDtos = mapper.Map<List<IssueDto>>(appUser.AssignedIssues);

		return Ok(issueDtos);

	}

	// 4) get projects assigned to user
	[HttpGet("{id}/projects")]
	public async Task<IActionResult> GetProjectAssignedToUser(string id)
	{
		AppUser appUser = await userManager.Users.Include(user => user.AssignedProjects)
																					 .SingleOrDefaultAsync(user => user.Id == id);

		if (appUser is null)
		{
			return Problem(
			detail: $"No user found with this Id '{id}'.",
			statusCode: StatusCodes.Status404NotFound);
		}

		List<ProjectDto> projectDtos = mapper.Map<List<ProjectDto>>(appUser.AssignedProjects);

		return Ok(projectDtos);
	}


	// 5) delete user by id
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteUserById(string id)
	{
		AppUser appUser = await userManager.FindByIdAsync(id);

		if (appUser is null)
		{
			return Problem(
				detail: $"No user found with this Id '{id}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		var result = await userManager.DeleteAsync(appUser);

		if (!result.Succeeded)
		{
			return Problem();
		}

		return NoContent();
	}



	// 6) update user by id

}