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
public class RolesController : ControllerBase
{
	private readonly RoleManager<IdentityRole> rolesManager;
	private readonly UserManager<AppUser> userManager;
	private readonly IMapper mapper;

	public RolesController(IMapper mapper, RoleManager<IdentityRole> rolesManager, UserManager<AppUser> userManager)
	{
		this.mapper = mapper;
		this.rolesManager = rolesManager;
		this.userManager = userManager;
	}



	// 1) get all Roles
	[HttpGet]
	public async Task<List<RoleDto>> GetAllRoles()
	{
		List<IdentityRole> roles = await rolesManager.Roles.ToListAsync();

		List<RoleDto> roleDtos = mapper.Map<List<RoleDto>>(roles);

		return roleDtos;
	}


	// 2) get Role By id
	[HttpGet("{id}")]
	public async Task<ActionResult<RoleDto>> GetRoleById(string id)
	{
		IdentityRole role = await rolesManager.FindByIdAsync(id);

		if (role is null)
		{
			return Problem(
				detail: $"No Role found with this Id '{id}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		RoleDto roleDto = mapper.Map<RoleDto>(role);

		return roleDto;
	}


	// 3) create new role
	[HttpPost]
	public async Task<IActionResult> CreateRole(CreateRoleDto createRoleDto)
	{
		IdentityRole identityRole = mapper.Map<IdentityRole>(createRoleDto);

		var result = await rolesManager.CreateAsync(identityRole);

		if (!result.Succeeded)
		{
			return BadRequest(new { title = "Bad Request", errors = result.Errors });
		}

		RoleDto roleDto = mapper.Map<RoleDto>(identityRole);

		return CreatedAtAction(nameof(CreateRole), roleDto);


	}


	// 4) Update Role
	[HttpPut("{id}")]
	public async Task<ActionResult<RoleDto>> UpdateRole(CreateRoleDto createRoleDto, string id)
	{
		IdentityRole identityRole = await rolesManager.FindByIdAsync(id);

		if (identityRole is null)
		{
			return Problem(
				detail: $"No Role found with this Id '{id}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		identityRole.Name = createRoleDto.Name;

		IdentityResult result = await rolesManager.UpdateAsync(identityRole);

		if (!result.Succeeded)
		{
			return BadRequest(new { title = "Bad Request", errors = result.Errors });
		}

		RoleDto roleDto = mapper.Map<RoleDto>(identityRole);

		return roleDto;
	}



	// 5) Delete Role
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteRoleById(string id)
	{
		IdentityRole identityRole = await rolesManager.FindByIdAsync(id);


		if (identityRole is null)
		{
			return Problem(
				detail: $"No Role found with this Id '{id}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		IdentityResult result = await rolesManager.DeleteAsync(identityRole);

		if (!result.Succeeded)
		{
			return BadRequest(new { title = "Bad Request", errors = result.Errors });
		}

		return NoContent();
	}



	// 6) Get Users In Role
	[HttpGet("{roleId}/users")]
	public async Task<ActionResult<List<UserDto>>> GetUsersInRole(string roleId)
	{
		IdentityRole identityRole = await rolesManager.FindByIdAsync(roleId);

		if (identityRole is null)
		{
			return Problem(
				detail: $"No Role found with this Id '{roleId}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		IEnumerable<AppUser> users = await userManager.GetUsersInRoleAsync(identityRole.Name);

		IEnumerable<UserDto> userDtos = mapper.Map<IEnumerable<UserDto>>(users);

		foreach (UserDto userDto in userDtos)
		{
			AppUser appUser = mapper.Map<AppUser>(userDto);
			userDto.Roles = (await userManager.GetRolesAsync(appUser)).ToList();
		}

		return userDtos.ToList();

	}


	// 7) Edit Users In Role
	[HttpPost("{roleId}/editusers")]
	public async Task<ActionResult<List<UserDto>>> EditUsersInRole(List<AssignUserDto> assignUserDtos, string roleId)
	{
		IdentityRole identityRole = await rolesManager.FindByIdAsync(roleId);

		if (identityRole is null)
		{
			return Problem(
				detail: $"No Role found with this Id '{roleId}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		IEnumerable<AppUser> usersInRole = await userManager.GetUsersInRoleAsync(identityRole.Name);

		foreach (AssignUserDto assignUserDto in assignUserDtos)
		{
			AppUser appUser = await userManager.FindByIdAsync(assignUserDto.UserId);

			if (appUser is null)
			{
				return Problem(
					detail: $"No User found with this Id '{assignUserDto.UserId}'.",
					statusCode: StatusCodes.Status404NotFound);
			}

			if (assignUserDto.IsSelected && !usersInRole.Contains(appUser))
			{
				await userManager.AddToRoleAsync(appUser, identityRole.Name);
			}
			else if (!assignUserDto.IsSelected && usersInRole.Contains(appUser))
			{
				await userManager.RemoveFromRoleAsync(appUser, identityRole.Name);
			}


		}


		usersInRole = await userManager.GetUsersInRoleAsync(identityRole.Name);

		IEnumerable<UserDto> userDtos = mapper.Map<IEnumerable<UserDto>>(usersInRole);

		foreach (UserDto userDto in userDtos)
		{
			AppUser appUser = mapper.Map<AppUser>(userDto);
			userDto.Roles = (await userManager.GetRolesAsync(appUser)).ToList();
		}

		return userDtos.ToList();
	}



}