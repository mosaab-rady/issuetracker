namespace issuetracker.Email;

public interface IEmailSender
{
	Task SendIssueCreatedAsync(string to, string issueUrl, string projectName);
	Task SendAssignToIssueAsync(string to,
														 string issueUrl,
														 string issueTitle,
														 string projectName,
														 string fromEmail,
														 string fromName);

	Task SendConfirmationLink(string to, string link);
}