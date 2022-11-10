using issuetracker.Database;
using issuetracker.Entities;
using Microsoft.EntityFrameworkCore;

namespace issuetracker.Services;

public class TagsService : ITagsServices
{
	private readonly PostgresContext context;

	public TagsService(PostgresContext context)
	{
		this.context = context;
	}

	public async Task<Tag> CreateTagAsync(Tag tag)
	{
		await context.Tags.AddAsync(tag);
		await context.SaveChangesAsync();
		return tag;
	}

	public async Task DeleteTagByIdAsync(Guid id)
	{
		var tag = await context.Tags.FindAsync(id);
		context.Tags.Remove(tag);
		await context.SaveChangesAsync();
	}

	public async Task<IEnumerable<Tag>> GetAllTagsAsync()
	{
		var tags = await context.Tags.ToListAsync();
		return tags;
	}

	public async Task<Tag> GetTagByIdAsync(Guid id)
	{
		var tag = await context.Tags.FindAsync(id);
		return tag;
	}

	public async Task UpdateTagByIdAsync(Guid id, Tag tag)
	{
		var ETag = await context.Tags.FindAsync(id);
		ETag = tag;
		await context.SaveChangesAsync();
	}
}