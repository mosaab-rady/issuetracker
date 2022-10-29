using issuetracker.Entities;

namespace issuetracker.Services;


public interface IIssuesService
{
	Task<IEnumerable<Issue>> GetAllIssuesAsync();
	Task<IEnumerable<Issue>> GetIssuesInProjectWithUsersAsync(Guid projectId);
	Task<Issue> GetIssueByIdAsync(Guid id);
	Task CreateIssueAsync(Issue issue);
	Task UpdateIssueByIdAsync(Guid id, Issue issue);
	Task DeleteIssueAsync(Guid id);
	Task AssignUser(AppUser user, Guid issueId);
	Task UnAssignUser(AppUser user, Guid issueId);
	Task<IEnumerable<Issue>> GetIssuesReportedByUser(string email);
}