using issuetracker.Database;
using issuetracker.Entities;
using Microsoft.EntityFrameworkCore;

namespace issuetracker.Services;

public class CommentsService : IcommentsService
{
	private readonly PostgresContext context;

	public CommentsService(PostgresContext context)
	{
		this.context = context;
	}

	public async Task CreateComment(Comment comment)
	{
		await context.Comments.AddAsync(comment);
		await context.SaveChangesAsync();
	}

	public async Task<IEnumerable<Comment>> GetAllCommentsOnIssue(Guid issueID)
	{
		List<Comment> comments = new();

		comments = (await context.Issues
			.Include(issue => issue.Comments)
			.ThenInclude(comment => comment.User)
			.SingleOrDefaultAsync(issue => issue.Id == issueID)
		).Comments.OrderBy(issue => issue.CreatedOn).ToList();

		return comments;
	}
}