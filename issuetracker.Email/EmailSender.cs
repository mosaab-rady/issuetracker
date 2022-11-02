using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace issuetracker.Email;

public class EmailSender : IEmailSender
{

	private readonly EmailConfiguration Options;
	private readonly IConfiguration configuration;

	public EmailSender(IConfiguration configuration)
	{
		this.configuration = configuration;
		Options = configuration.GetSection("EmailOptions").Get<EmailConfiguration>();
	}


	public Task SendIssueCreatedAsync(string to, string issueUrl, string projectName)
	{
		string subject = "New Issue";

		string body = $"<div> <h4>a new issue has been created in project {projectName} check it out</h4> <a href='{issueUrl}' >Detail</a> </div>";

		return Execute(to, subject, body, Options.SenderEmail, Options.SenderName);
	}



	public Task SendAssignToIssueAsync(string to, string issueUrl, string issueTitle, string projectName, string fromEmail, string fromName)
	{
		string subject = "you are assigned to new issue";

		string body = $"<div><h4>you have been assigned to issue '{issueTitle}' in {projectName} project</h4><a href='{issueUrl}'>click here for details</a></div>";

		return Execute(to, subject, body, fromEmail, fromName);
	}

	public async Task Execute(string to, string subject, string message, string? senderEmail, string? senderName)
	{
		// create message
		var email = new MimeMessage();

		email.Sender = MailboxAddress.Parse(senderEmail);

		if (!string.IsNullOrEmpty(senderName))
			email.Sender.Name = senderName;

		email.From.Add(email.Sender);

		email.To.Add(MailboxAddress.Parse(to));

		email.Subject = subject;

		email.Body = new TextPart(TextFormat.Html) { Text = message };

		// send email
		using var smtp = new SmtpClient();

		smtp.Connect(Options.HostAddress, Options.HostPort, Options.HostSecureSocketOptions);

		smtp.Authenticate(Options.HostUsername, Options.HostPassword);

		await smtp.SendAsync(email);

		smtp.Disconnect(true);

	}

	public Task SendConfirmationLink(string to, string link)
	{
		string subject = "Confirm your email.";

		string body = $"<div><h3>Thank you for signing up</h3><p>please click the link below to confirm your email</p> <a href='{link}'>Confirm</a></div>";

		return Execute(to, subject, body, Options.SenderEmail, Options.SenderName);
	}
}