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
		var issue = await context.Issues
		.Include(issue => issue.AssignedTo)
		.SingleOrDefaultAsync(issue => issue.Id == issueId);

		issue.AssignedTo.Add(user);
		await context.SaveChangesAsync();
	}
	public async Task UnAssignUser(AppUser user, Guid issueId)
	{
		var issue = await context.Issues
		.Include(issue => issue.AssignedTo)
		.SingleOrDefaultAsync(issue => issue.Id == issueId);

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
		var issues = await context.Issues
			.Include(x => x.Priority)
			.Include(x => x.Project)
			.ToListAsync();
		return issues;
	}

	public async Task<Issue> GetIssueByIdWithUsersAsync(Guid id)
	{
		var issue = await context.Issues
			.Include(issue => issue.AssignedTo)
			.Include(issue => issue.Project)
			.Include(issue => issue.Priority)
			.Include(issue => issue.Tags)
			.SingleOrDefaultAsync(issue => issue.Id == id);

		return issue;
	}

	public async Task<Issue> GetIssueByIdAsync(Guid id)
	{
		Issue issue = await context.Issues
		.Include(issue => issue.Priority)
		.Include(issue => issue.Project)
		.SingleOrDefaultAsync(issue => issue.Id == id);

		return issue;
	}

	public async Task<Priority> GetIssuePriorityAsync(Guid id)
	{
		List<Priority> priorities = await context.Issues
		.Where(issue => issue.Id == id)
		.Take(1)
		.Select(issue => issue.Priority)
		.ToListAsync();



		return priorities.FirstOrDefault();
	}


	public async Task<List<Tag>> GetIssueTagsAsync(Guid id)
	{
		List<List<Tag>> issuesTags = await context.Issues
					.Where(issue => issue.Id == id)
					.Take(1)
					.Select(issue => issue.Tags)
					.ToListAsync();

		return issuesTags.FirstOrDefault();
	}

	public async Task<List<AppUser>> GetIssueUsersAsync(Guid id)
	{
		List<List<AppUser>> issuesUsers = await context.Issues
		.Where(issue => issue.Id == id)
		.Take(1)
		.Select(issue => issue.AssignedTo)
		.ToListAsync();

		return issuesUsers.FirstOrDefault();
	}

	public async Task<Project> GetIssueProjectAsync(Guid id)
	{
		List<Project> issuesProjects = await context.Issues
		.Where(issue => issue.Id == id)
		.Take(1)
		.Select(issue => issue.Project)
		.ToListAsync();

		return issuesProjects.FirstOrDefault();
	}


	public async Task UpdateIssueByIdAsync(Guid id, Issue issue)
	{
		var EIssue = await context.Issues.FindAsync(id);
		EIssue = issue;
		await context.SaveChangesAsync();
	}

	public async Task<IEnumerable<Issue>> GetIssuesInProjectWithUsersAsync(Guid projectId)
	{
		var issues = await context.Issues
			.Include(issue => issue.AssignedTo)
			.Include(issue => issue.Priority)
			.Where(issue => issue.Project.Id == projectId)
			.ToListAsync();

		return issues;
	}

	public async Task<IEnumerable<Issue>> GetIssuesReportedByUser(string email)
	{
		var issues = await context.Issues
			.Include(issue => issue.Priority)
			.Include(issue => issue.Project)
			.Where(issue => issue.CreatedBy == email)
			.ToListAsync();

		return issues;
	}

	public async Task AssignTag(Tag tag, Guid issueId)
	{
		Issue issue = await context.Issues.Include(issue => issue.Tags).SingleOrDefaultAsync(issue => issue.Id == issueId);

		issue.Tags.Add(tag);
		await context.SaveChangesAsync();
	}

	public async Task UnAssignTag(Tag tag, Guid issueId)
	{
		Issue issue = await context.Issues.Include(issue => issue.Tags).SingleOrDefaultAsync(issue => issue.Id == issueId);

		issue.Tags.Remove(tag);
		await context.SaveChangesAsync();
	}
}