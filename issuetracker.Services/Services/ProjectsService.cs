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
		var project = await context.Projects.Include(x => x.Issues)
			.Include(x => x.AssignedTo)
			.SingleOrDefaultAsync(e => e.Slug == slug);
		return project;
	}

	public async Task<Project> GetProjectByIdAsync(Guid id)
	{
		var project = await context.Projects.FindAsync(id);
		return project;
	}

	public async Task UpdateProjectByIdAsync(Guid id, Project project)
	{
		var EProject = await context.Projects.FindAsync(id);
		EProject = project;
		await context.SaveChangesAsync();
	}
}