using issuetracker.Entities;

namespace issuetracker.Services;

public interface IPriorityService
{
	Task<IEnumerable<Priority>> GetAllPrioritiesAsync();

	Task<Priority> GetPriorityByIdAsync(Guid id);

	Task<Priority> CreatePriorityAsync(Priority priority);

	Task UpdatePriorityByIdAsync(Guid id, Priority priority);

	Task DeletePriorityByIdAsync(Guid id);
	Task<List<Issue>> GetIssuesWhithPriority(Guid priorityId);
}