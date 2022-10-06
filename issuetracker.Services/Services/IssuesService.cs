using issuetracker.Database;
using issuetracker.Entities;
using Microsoft.EntityFrameworkCore;

namespace issuetracker.Services;

public class IssuesService : IIssuesService
{
	private readonly PostgresContext context;

	public IssuesService(PostgresContext context)
	{
		this.context = context;
	}

	public async Task AssignUser(AppUser user, Guid issueId)
	{
		var issue = await context.Issues.FindAsync(issueId);
		issue.AssignedTo.Add(user);
		await context.SaveChangesAsync();
	}
	public async Task UnAssignUser(AppUser user, Guid issueId)
	{
		var issue = await context.Issues.FindAsync(issueId);
		issue.AssignedTo.Remove(user);
		await context.SaveChangesAsync();
	}

	public async Task CreateIssueAsync(Issue issue)
	{
		await context.Issues.AddAsync(issue);
		await context.SaveChangesAsync();
	}

	public async Task DeleteIssueAsync(Guid id)
	{
		var issue = await context.Issues.FindAsync(id);
		context.Issues.Remove(issue);
		await context.SaveChangesAsync();
	}

	public async Task<IEnumerable<Issue>> GetAllIssuesAsync()
	{
		var issues = await context.Issues.Include(x => x.Priority).Include(x => x.Project).Include(x => x.AssignedTo).ToListAsync();
		return issues;
	}

	public async Task<Issue> GetIssueByIdAsync(Guid id)
	{
		var issue = await context.Issues.Include(x => x.AssignedTo)
			.Include(x => x.Project)
			.Include(x => x.Priority)
			.Include(x => x.Tags)
			.SingleOrDefaultAsync(x => x.Id == id);

		return issue;
	}


	public async Task UpdateIssueByIdAsync(Guid id, Issue issue)
	{
		var EIssue = await context.Issues.FindAsync(id);
		EIssue = issue;
		await context.SaveChangesAsync();
	}
}