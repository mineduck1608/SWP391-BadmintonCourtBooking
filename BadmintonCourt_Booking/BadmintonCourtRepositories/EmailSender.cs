using BadmintonCourtServices.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices
{
	public class EmailSender : IEmailSender
	{
		private static readonly string _from = "acceptpls10@gmail.com";
		//APP PASSWORD
		private static readonly string _password = "xyzt brbk oxuy lirl";
		public static async Task SendEmailAsync(string email, string subject, string body)
		{
			MailMessage message = new MailMessage(_from, email, subject, body);
			message.BodyEncoding = Encoding.UTF8;
			message.SubjectEncoding = Encoding.UTF8;
			message.IsBodyHtml = true;
			//THIS IS CURRENTLY NOT WORKING
			message.Sender = new MailAddress(_from, "Badminton Court Services");

			using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
			{
				EnableSsl = true,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(_from, _password)
			};

			await smtpClient.SendMailAsync(message);
		}
	}
}
