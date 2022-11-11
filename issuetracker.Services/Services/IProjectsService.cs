using issuetracker.Entities;

namespace issuetracker.Services;

public interface IProjectsService
{
	Task<IEnumerable<Project>> GetAllProjectsAsync();
	Task<Project> GetProjectByIdAsync(Guid id);
	Task<Project> GetProjectByIdWithUsersAsync(Guid id);
	Task<Project> GetOneProjectAsync(string slug);
	Task<Project> GetProjectWithUsersAsync(string slug);
	Task CreateProjectAsync(Project project);
	Task UpdateProjectByIdAsync(Guid id, Project project);
	Task DeleteProjectByIdAsync(Guid id);

	Task AssignUser(AppUser user, Guid projectId);
	Task UnAssignUser(AppUser user, Guid projectId);

	Task<List<AppUser>> GetUsersInProjectAsync(Guid id);
	Task<List<Issue>> GetIssuesInProjectAsync(Guid id);
}