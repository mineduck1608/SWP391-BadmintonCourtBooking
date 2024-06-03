using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BadmintonCourtServices;
namespace BadmintonCourtAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OtherServicesController : ControllerBase
	{
		/// <summary>
		/// Send an email from <code>acceptpls10@gmail.com</code> to the desired Email
		/// </summary>
		/// <param name="email">The target email</param>
		/// <param name="subject">Subject of the mail</param>
		/// <param name="body">Content of the mail, allow HTML body</param>
		/// <returns>If the </returns>
		[HttpGet]
		[Route("/Email")]
		public async Task<bool> SendMail(string email, string subject, string body)
		{
			try
			{
				await BadmintonCourtServices.EmailSender.SendEmailAsync(email, subject, body);
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}
