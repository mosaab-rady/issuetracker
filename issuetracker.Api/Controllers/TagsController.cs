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
public class TagsController : ControllerBase
{
	private readonly ITagsServices tagsServices;

	private readonly IMapper mapper;

	public TagsController(ITagsServices tagsServices, IMapper mapper)
	{
		this.tagsServices = tagsServices;
		this.mapper = mapper;
	}

	// 1) get All tags
	[HttpGet]
	public async Task<IActionResult> GetAllTAgs()
	{
		IEnumerable<Tag> tags = await tagsServices.GetAllTagsAsync();

		IEnumerable<TagDto> tagDtos = mapper.Map<IEnumerable<TagDto>>(tags);


		return Ok(tagDtos);
	}

	// 2) get Tag By Id
	[HttpGet("{id}")]
	public async Task<IActionResult> GetTagById(Guid id)
	{
		Tag tag = await tagsServices.GetTagByIdAsync(id);

		if (tag is null)
		{
			return Problem(
				detail: $"No Tag found with this Id '{id}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		TagDto tagDto = mapper.Map<TagDto>(tag);

		return Ok(tagDto);
	}


	// 3) create Tag
	[HttpPost]
	public async Task<IActionResult> CreateTag(CreateTagDto model)
	{
		Tag tag = mapper.Map<Tag>(model);

		await tagsServices.CreateTagAsync(tag);

		TagDto tagDto = mapper.Map<TagDto>(tag);

		return CreatedAtAction(nameof(CreateTag), tagDto);
	}


	// 4) delete Tag
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteTagById(Guid id)
	{
		Tag tag = await tagsServices.GetTagByIdAsync(id);

		if (tag is null)
		{
			return Problem(
				detail: $"No Tag found with this Id '{id}'.",
				statusCode: StatusCodes.Status404NotFound);
		}

		await tagsServices.DeleteTagByIdAsync(tag.Id);
		return NoContent();
	}


	// 5) Update Tag


	// 6) Get All issues in a tag


}