using issuetracker.Database;
using issuetracker.Entities;
using Microsoft.EntityFrameworkCore;

namespace issuetracker.Services;

public class ProjectsService : IProjectsService
{
	private readonly PostgresContext context;

	public ProjectsService(PostgresContext context)
	{
		this.context = context;
	}

	public async Task AssignUser(AppUser user, Guid projectId)
	{
		var project = await context.Projects
		.Include(project => project.AssignedTo)
		.SingleOrDefaultAsync(project => project.Id == projectId);

		project.AssignedTo.Add(user);
		await context.SaveChangesAsync();
	}

	public async Task UnAssignUser(AppUser user, Guid projectId)
	{
		var project = await context.Projects
			.Include(project => project.AssignedTo)
			.SingleOrDefaultAsync(project => project.Id == projectId);

		project.AssignedTo.Remove(user);
		await context.SaveChangesAsync();
	}

	public async Task CreateProjectAsync(Project project)
	{
		await context.Projects.AddAsync(project);
		await context.SaveChangesAsync();
	}

	public async Task DeleteProjectByIdAsync(Guid id)
	{
		var project = await context.Projects.FindAsync(id);
		context.Projects.Remove(project);
		await context.SaveChangesAsync();
	}

	public async Task<IEnumerable<Project>> GetAllProjectsAsync()
	{
		var projects = await context.Projects.ToListAsync();
		return projects;
	}

	public async Task<Project> GetOneProjectAsync(string slug)
	{
		var project = await context.Projects
		.SingleOrDefaultAsync(project => project.Slug == slug);
		return project;
	}

	public async Task<Project> GetProjectByIdAsync(Guid id)
	{
		var project = await context.Projects.SingleOrDefaultAsync(x => x.Id == id);
		return project;
	}

	public async Task UpdateProjectByIdAsync(Guid id, Project project)
	{
		var EProject = await context.Projects.FindAsync(id);
		EProject = project;
		await context.SaveChangesAsync();
	}

	public async Task<Project> GetProjectByIdWithUsersAsync(Guid id)
	{
		var project = await context.Projects
		.Include(project => project.AssignedTo)
		.SingleOrDefaultAsync(project => project.Id == id);

		return project;
	}

	public async Task<Project> GetProjectWithUsersAsync(string slug)
	{
		var project = await context.Projects
			.Include(project => project.AssignedTo)
			.SingleOrDefaultAsync(project => project.Slug == slug);

		return project;


	}

	public async Task<List<AppUser>> GetUsersInProjectAsync(Guid id)
	{
		List<List<AppUser>> users = await context.Projects
		.Where(project => project.Id == id)
		.Select(project => project.AssignedTo)
		.ToListAsync();

		return users.FirstOrDefault();
	}

	public async Task<List<Issue>> GetIssuesInProjectAsync(Guid id)
	{
		List<List<Issue>> issues = await context.Projects
		.Where(project => project.Id == id)
		.Include(project => project.Issues)
		.ThenInclude(issue => issue.Priority)
		.Select(project => project.Issues)
		.ToListAsync();

		return issues.FirstOrDefault();
	}
}