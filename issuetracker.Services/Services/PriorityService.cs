using issuetracker.Database;
using issuetracker.Entities;
using Microsoft.EntityFrameworkCore;

namespace issuetracker.Services;


public class PriorityService : IPriorityService
{
	private readonly PostgresContext context;

	public PriorityService(PostgresContext context)
	{
		this.context = context;
	}

	public async Task<Priority> CreatePriorityAsync(Priority priority)
	{
		await context.Priority.AddAsync(priority);
		await context.SaveChangesAsync();
		return priority;
	}

	public async Task DeletePriorityByIdAsync(Guid id)
	{
		var EPriority = await context.Priority.FindAsync(id);
		context.Priority.Remove(EPriority);
		await context.SaveChangesAsync();
	}

	public async Task<IEnumerable<Priority>> GetAllPrioritiesAsync()
	{
		var Priorities = await context.Priority.ToListAsync();
		return Priorities;
	}

	public async Task<List<Issue>> GetIssuesWhithPriority(Guid priorityId)
	{
		List<List<Issue>> issues = await context.Priority.Where(priority => priority.Id == priorityId)
		.Select(priority => priority.Issues)
		.ToListAsync();

		return issues.FirstOrDefault();
	}

	public async Task<Priority> GetPriorityByIdAsync(Guid id)
	{
		var priority = await context.Priority.FindAsync(id);
		return priority;
	}

	public async Task UpdatePriorityByIdAsync(Guid id, Priority priority)
	{
		var EPriority = await context.Priority.FindAsync(id);
		EPriority = priority;
		await context.SaveChangesAsync();
	}
}