using BadmintonCourtServices.IService;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices
{
	public class MailService : IMailService
	{
		private const string senderMail = "externalauthdemo1234@gmail.com";
		private const string appPass = "iydb znrz nkgl ibdy";

		public void SendMail(string des, string content, string subject)
		{
			var emailToSend = new MimeMessage();
			emailToSend.From.Add(MailboxAddress.Parse(senderMail));
			emailToSend.To.Add(MailboxAddress.Parse(des));
			emailToSend.Subject = subject;
			emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = content };
			using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
			{
				emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
				emailClient.Authenticate(senderMail, appPass);
				emailClient.Send(emailToSend);
				emailClient.Disconnect(true);
			}
		}
	}
}
