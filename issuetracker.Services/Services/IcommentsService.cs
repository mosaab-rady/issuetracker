using issuetracker.Entities;

namespace issuetracker.Services;

public interface IcommentsService
{
	Task<IEnumerable<Comment>> GetAllCommentsOnIssue(Guid issueID);
	Task CreateComment(Comment comment);
}