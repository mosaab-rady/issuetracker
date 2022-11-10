using AutoMapper;
using issuetracker.Dtos;
using issuetracker.Entities;
using issuetracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace issuetracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PriorityController : ControllerBase
{
	private readonly IPriorityService priorityService;
	private readonly IMapper mapper;

	public PriorityController(IPriorityService priorityService, IMapper mapper)
	{
		this.priorityService = priorityService;
		this.mapper = mapper;
	}

	// 1) get all Priorities
	[HttpGet]
	public async Task<IActionResult> GetAllPriorities()
	{
		IEnumerable<Priority> priorities = await priorityService.GetAllPrioritiesAsync();

		IEnumerable<PriorityDto> priorityDtos = mapper.Map<IEnumerable<PriorityDto>>(priorities);

		return Ok(priorityDtos);
	}


	// 2) get Priority by Id
	[HttpGet("{id}")]
	public async Task<IActionResult> GetPriorityById(Guid id)
	{
		Priority priority = await priorityService.GetPriorityByIdAsync(id);

		if (priority is null)
		{
			return Problem(
				detail: $"No Priority found with this Id '{id}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		PriorityDto priorityDto = mapper.Map<PriorityDto>(priority);

		return Ok(priorityDto);
	}



	// 3) Create Priority
	[HttpPost]
	public async Task<IActionResult> CreatePriority(CreatePriorityDto model)
	{
		Priority priority = mapper.Map<Priority>(model);

		await priorityService.CreatePriorityAsync(priority);

		PriorityDto priorityDto = mapper.Map<PriorityDto>(priority);

		return CreatedAtAction(
			nameof(CreatePriority),
			priorityDto);
	}



	// 4) Update Priority



	// 5) delete Priority
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeletePriorityById(Guid id)
	{
		Priority priority = await priorityService.GetPriorityByIdAsync(id);

		if (priority is null)
		{
			return Problem(
				detail: $"No Priority found with this Id '{id}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		await priorityService.DeletePriorityByIdAsync(priority.Id);

		return NoContent();
	}




	// 6) get issues In Priority
	[HttpGet("{priorityId}/Issues")]
	public async Task<IActionResult> GetIssuesInPriority(Guid priorityId)
	{
		Priority priority = await priorityService.GetPriorityByIdAsync(priorityId);

		if (priority is null)
		{
			return Problem(
				detail: $"No Priority found with this Id '{priorityId}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		List<Issue> issues = await priorityService.GetIssuesWhithPriority(priorityId);

		List<IssueDto> issueDtos = mapper.Map<List<IssueDto>>(issues);

		return Ok(issueDtos);
	}


}