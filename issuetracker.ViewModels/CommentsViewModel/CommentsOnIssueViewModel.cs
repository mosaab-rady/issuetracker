namespace issuetracker.ViewModels;

public class CommentsOnIssueViewModel
{
	public CommentsOnIssueViewModel()
	{
		Comments = new List<CommentViewModel>();
	}
	public CreateCommentViewModel CreateComment { get; set; }

	public string IssueTitle { get; set; }
	public string IssueDescription { get; set; }
	public List<CommentViewModel> Comments { get; set; }
}