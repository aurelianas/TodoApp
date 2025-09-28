using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using ToDoApp.Shared;

namespace ToDoApp.Server.Controllers;

[ApiController]
public class EmailController : Controller
{
	[HttpPost]
	[Route(ApiEndpoints.EmailEndpoints.Send)]
	public IActionResult Send()
	{
		try
		{
			var body = "<p>This is the email body just for testing purpose!!!</p>";
			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse("aurelianas87@gmail.com"));
			email.To.Add(MailboxAddress.Parse("aurelian.stancu@diginesis.com"));
			//email.To.Add(MailboxAddress.Parse("ianosmadalina@gmail.com"));
			email.Subject = "Test email Subject";
			email.Body = new TextPart(TextFormat.Html) { Text = body };

			using var smtp = new SmtpClient();
			smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
			var pass = "geksmjcdkmrtbljk"; //password of the gmail generated using https://myaccount.google.com/apppasswords
			smtp.Authenticate("aurelianas87@gmail.com", pass);
			smtp.Send(email);
			smtp.Disconnect(true);
			return Ok(true);
		}
		catch (Exception ex)
		{
			return Ok(false);
		}
	}
}