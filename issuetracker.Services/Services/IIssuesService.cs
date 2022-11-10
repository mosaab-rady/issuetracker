using issuetracker.Entities;

namespace issuetracker.Services;


public interface IIssuesService
{
	Task<IEnumerable<Issue>> GetAllIssuesAsync();
	Task<IEnumerable<Issue>> GetIssuesInProjectWithUsersAsync(Guid projectId);
	Task<Issue> GetIssueByIdWithUsersAsync(Guid id);
	Task CreateIssueAsync(Issue issue);
	Task UpdateIssueByIdAsync(Guid id, Issue issue);
	Task DeleteIssueAsync(Guid id);
	Task AssignUser(AppUser user, Guid issueId);
	Task UnAssignUser(AppUser user, Guid issueId);
	Task AssignTag(Tag tag, Guid issueId);
	Task UnAssignTag(Tag tag, Guid issueId);
	Task<IEnumerable<Issue>> GetIssuesReportedByUser(string email);

	Task<Priority> GetIssuePriorityAsync(Guid id);
	Task<List<Tag>> GetIssueTagsAsync(Guid id);
	Task<Project> GetIssueProjectAsync(Guid id);
	Task<List<AppUser>> GetIssueUsersAsync(Guid id);
	Task<Issue> GetIssueByIdAsync(Guid id);
}