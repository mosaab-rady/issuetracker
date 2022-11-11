using AutoMapper;
using issuetracker.Dtos;
using issuetracker.Entities;
using issuetracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace issuetracker.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IssuesController : ControllerBase
{
	private readonly IIssuesService issuesService;
	private readonly IMapper mapper;
	private readonly IProjectsService projectsService;
	private readonly UserManager<AppUser> userManager;
	private readonly IPriorityService prioirityServices;
	private readonly ITagsServices tagsServices;

	public IssuesController(IIssuesService issuesService, IMapper mapper, IProjectsService projectsService, UserManager<AppUser> userManager, IPriorityService prioirityServices, ITagsServices tagsServices)
	{
		this.issuesService = issuesService;
		this.mapper = mapper;
		this.projectsService = projectsService;
		this.userManager = userManager;
		this.prioirityServices = prioirityServices;
		this.tagsServices = tagsServices;
	}


	// 1) get all Issues
	[HttpGet]
	[Authorize(Roles = "manager")]
	public async Task<IActionResult> GetAllIssues()
	{
		IEnumerable<Issue> issues = await issuesService.GetAllIssuesAsync();

		IEnumerable<IssueDto> issueDtos = mapper.Map<IEnumerable<IssueDto>>(issues);

		return Ok(issueDtos);

	}


	// 2) get issue By Id
	[HttpGet("{id}")]
	public async Task<IActionResult> GetIssueById(Guid id)
	{
		Issue issue = await issuesService.GetIssueByIdAsync(id);

		if (issue is null)
		{
			return Problem(
				detail: $"No Issue found with this Id '{id}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		IssueDto issueDto = mapper.Map<IssueDto>(issue);

		return Ok(issueDto);
	}


	// 3) Create Issue
	[HttpPost]
	public async Task<IActionResult> CreateIssue(CreateIssueDto CreateIssueDto)
	{
		Project project = await projectsService.GetProjectByIdAsync(Guid.Parse(CreateIssueDto.ProjectId));

		if (project is null)
		{
			return Problem(
				detail: $"The Project You selected does not exist.",
				statusCode: StatusCodes.Status404NotFound);
		}

		Issue issue = mapper.Map<Issue>(CreateIssueDto);

		issue.Project = project;
		issue.CreatedBy = User.Identity.Name;

		await issuesService.CreateIssueAsync(issue);

		IssueDto issueDto = mapper.Map<IssueDto>(issue);

		return CreatedAtAction(nameof(CreateIssue), issueDto);
	}


	// 4) update Issue
	[HttpPut("{id}")]
	[Authorize(Roles = "manager")]
	public async Task<IActionResult> UpdateIssueById(UpdateIssueDto updateIssueDto, Guid id)
	{
		Issue issue = await issuesService.GetIssueByIdAsync(id);

		if (issue is null)
		{
			return Problem(
				detail: $"No Issue found with this Id '{id}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		issue.Title = updateIssueDto.Title;
		issue.Description = updateIssueDto.Description;
		issue.TargetResolutionDate = updateIssueDto.TargetResolutionDate.ToUniversalTime();
		issue.ActualResolutionDate = updateIssueDto.ActualResolutionDate.ToUniversalTime();

		await issuesService.UpdateIssueByIdAsync(issue.Id, issue);


		IssueDto issueDto = mapper.Map<IssueDto>(issue);
		return Ok(issueDto);
	}


	// 5) Delete Issue
	[HttpDelete("{id}")]
	[Authorize(Roles = "manager")]
	public async Task<IActionResult> DeleteIssueById(Guid id)
	{
		Issue issue = await issuesService.GetIssueByIdAsync(id);

		if (issue is null)
		{
			return Problem(
				detail: $"No Issue found with this Id '{id}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		await issuesService.DeleteIssueAsync(issue.Id);

		return NoContent();
	}


	// 6) Edit Users in Issue
	[HttpPost("{issueId}/editusers")]
	[Authorize(Roles = "manager")]
	public async Task<IActionResult> EditUsersInIssue(List<AssignUserDto> assignUserDtos, Guid issueId)
	{
		Issue issue = await issuesService.GetIssueByIdAsync(issueId);

		if (issue is null)
		{
			return Problem(
				detail: $"No Ossue found with this Id '{issueId}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		List<AppUser> usersThatCanBeAssignedToThisIssue = await projectsService.GetUsersInProjectAsync(issue.Project.Id);

		List<AppUser> usersInIssue = await issuesService.GetIssueUsersAsync(issueId);

		foreach (AssignUserDto assignUserDto in assignUserDtos)
		{
			AppUser appUser = await userManager.FindByIdAsync(assignUserDto.UserId);

			if (appUser is null)
			{
				return Problem(
					detail: $"No user found with this Id '{assignUserDto.UserId}'.",
					statusCode: StatusCodes.Status404NotFound);
			}

			if (usersThatCanBeAssignedToThisIssue.Contains(appUser))
			{
				if (assignUserDto.IsSelected && !usersInIssue.Contains(appUser))
				{
					await issuesService.AssignUser(appUser, issueId);
				}
				else if (!assignUserDto.IsSelected && usersInIssue.Contains(appUser))
				{
					await issuesService.UnAssignUser(appUser, issueId);
				}
			}
			else
			{
				return Problem(
					detail: $"The user with Email '{assignUserDto.Email}' can not be selected. the user is not assigned to the project.",
					statusCode: StatusCodes.Status400BadRequest);
			}

		}

		usersInIssue = await issuesService.GetIssueUsersAsync(issueId);

		List<UserDto> userDtos = mapper.Map<List<UserDto>>(usersInIssue);

		return Ok(userDtos);
	}


	// 7) Get Issue Tags
	[HttpGet("{id}/tags")]
	public async Task<IActionResult> GetIssueTagsByIssueId(Guid id)
	{
		Issue issue = await issuesService.GetIssueByIdAsync(id);

		if (issue is null)
		{
			return Problem(
			detail: $"No Issue found with this Id '{id}'.",
			statusCode: StatusCodes.Status404NotFound);
		}

		var tags = await issuesService.GetIssueTagsAsync(issue.Id);

		List<TagDto> tagDtos = mapper.Map<List<TagDto>>(tags);

		return Ok(tagDtos);
	}



	// 8) Get Issue Project
	[HttpGet("{id}/project")]
	public async Task<IActionResult> GetIssueProject(Guid id)
	{
		Issue issue = await issuesService.GetIssueByIdAsync(id);

		if (issue is null)
		{
			return Problem(
			detail: $"No Issue found with this Id '{id}'.",
			statusCode: StatusCodes.Status404NotFound);
		}

		Project project = await issuesService.GetIssueProjectAsync(issue.Id);

		ProjectDto projectDto = mapper.Map<ProjectDto>(project);

		return Ok(projectDto);
	}


	// 9) Get Issue Priority
	[HttpGet("{id}/priority")]
	public async Task<IActionResult> GetIssuePriority(Guid id)
	{
		Issue issue = await issuesService.GetIssueByIdAsync(id);

		if (issue is null)
		{
			return Problem(
			detail: $"No Issue found with this Id '{id}'.",
			statusCode: StatusCodes.Status404NotFound);
		}

		Priority priority = await issuesService.GetIssuePriorityAsync(id);

		if (priority is null)
		{
			return Problem(
				detail: $"The Issue with Id '{id}' does not have Priority set.",
				statusCode: StatusCodes.Status404NotFound);
		}

		PriorityDto priorityDto = mapper.Map<PriorityDto>(priority);

		return Ok(priorityDto);
	}


	// 10) Get Issue Users
	[HttpGet("{id}/users")]
	public async Task<IActionResult> GetUsersAssignedToIssue(Guid id)
	{
		Issue issue = await issuesService.GetIssueByIdAsync(id);

		if (issue is null)
		{
			return Problem(
			detail: $"No Issue found with this Id '{id}'.",
			statusCode: StatusCodes.Status404NotFound);
		}

		List<AppUser> users = await issuesService.GetIssueUsersAsync(issue.Id);

		List<UserDto> userDtos = mapper.Map<List<UserDto>>(users);

		foreach (var user in userDtos)
		{
			user.Roles = (await userManager.GetRolesAsync(mapper.Map<AppUser>(user))).ToList();
		}

		return Ok(userDtos);
	}


	// 11) Update Issue Priority
	[HttpPost("{issueId}/Priority")]
	[Authorize(Roles = "manager")]
	public async Task<IActionResult> UpdateIssuePriority(AssignPriorityDto assignPriorityDto, Guid issueId)
	{
		Issue issue = await issuesService.GetIssueByIdAsync(issueId);

		if (issue is null)
		{
			return Problem(
				detail: $"No Issue found with this Id '{issueId}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		Priority priority = await prioirityServices.GetPriorityByIdAsync(assignPriorityDto.PriorityId);

		if (priority is null)
		{
			return Problem(
				detail: $"No Priority found with this Id '{assignPriorityDto.PriorityId}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		issue.Priority = priority;
		await issuesService.UpdateIssueByIdAsync(issue.Id, issue);


		IssueDto issueDto = mapper.Map<IssueDto>(issue);
		return Ok(issueDto);

	}


	// 12) Update Issue Tags
	[HttpPost("{issueId}/tags")]
	[Authorize(Roles = "manager")]
	public async Task<IActionResult> UpdateIssueTags(List<AssignTagDto> assignTagDtos, Guid issueId)
	{
		Issue issue = await issuesService.GetIssueByIdAsync(issueId);

		List<Tag> issueTags = await issuesService.GetIssueTagsAsync(issueId);


		if (issue is null)
		{
			return Problem(
				detail: $"No Issue found with this Id '{issueId}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		foreach (var assignTagDto in assignTagDtos)
		{
			Tag tag = await tagsServices.GetTagByIdAsync(assignTagDto.TagId);

			if (tag is null)
			{
				return Problem(
					detail: $"No Tag found with this Id '{assignTagDto.TagId}'.",
					statusCode: StatusCodes.Status404NotFound);
			}

			if (assignTagDto.IsSelected && !issueTags.Contains(tag))
			{
				await issuesService.AssignTag(tag, issue.Id);
			}
			else if (!assignTagDto.IsSelected && issueTags.Contains(tag))
			{
				await issuesService.UnAssignTag(tag, issue.Id);
			}

		}

		issueTags = await issuesService.GetIssueTagsAsync(issueId);

		List<TagDto> tagDtos = mapper.Map<List<TagDto>>(issueTags);

		return Ok(tagDtos);

	}



	// 13) close Issue
	[HttpPost("{id}/close")]
	public async Task<IActionResult> CloseIssue(Guid id)
	{
		Issue issue = await issuesService.GetIssueByIdAsync(id);

		if (issue is null)
		{
			return Problem(
				detail: $"No Issue found with this Id '{id}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		issue.Status = Status.Closed;

		await issuesService.UpdateIssueByIdAsync(id, issue);

		IssueDto issueDto = mapper.Map<IssueDto>(issue);

		return Ok(issueDto);
	}


}