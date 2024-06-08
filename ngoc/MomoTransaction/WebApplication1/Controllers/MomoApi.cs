using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoMo;
using Newtonsoft.Json;

namespace WebApplication1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MomoApi : ControllerBase
	{
		private static IConfiguration _config;
		public MomoApi(IConfiguration configuration) { _config = configuration; }
		[HttpGet]
		[Route("/Send")]
		public async Task<MomoResponse> SendResponse(string orderInfo, string amount)
		{
			MomoRequest request = new MomoRequest()
			{
				Amount = amount,
				OrderInfo = orderInfo
			};
			LoadData(request);
			request.OrderID = DateTime.UtcNow.Ticks.ToString();
			return await Services.SendRequest(request);
		}
		public static void LoadData(MomoRequest request)
		{
			request.ExtraData = "";
			request.Endpoint = _config.GetSection("MoMoRequest").GetValue<string>("Endpoint");
			request.PartnerCode = _config.GetSection("MoMoRequest").GetValue<string>("PartnerCode");
			request.AccessKey = _config.GetSection("MoMoRequest").GetValue<string>("AccessKey");
			request.SecretKey = _config.GetSection("MoMoRequest").GetValue<string>("SecretKey");
			request.ReturnUrl = _config.GetSection("MoMoRequest").GetValue<string>("ReturnUrl");
			request.NotifyUrl = _config.GetSection("MoMoRequest").GetValue<string>("NotifyUrl");
			request.RequestType = _config.GetSection("MoMoRequest").GetValue<string>("RequestType");
		}
	}
}
