using Azure;
using BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.Momo;
using BadmintonCourtServices.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices
{
	public class MoMoService : IMoMoService
	{
		private IConfiguration _config = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", true, true).Build();

		static readonly HttpClient client = new HttpClient();

		public string CreateRawData(MoMoRequestData request) => $"partnerCode={request.PartnerCode}" +
				$"&accessKey={request.AccessKey}" +
				$"&requestId={request.OrderID}" +
				$"&amount={request.Amount}" +
				$"&orderId={request.OrderID}" +
				$"&orderInfo={request.OrderInfo}" +
				$"&returnUrl={request.ReturnUrl}" +
				$"&notifyUrl={request.NotifyUrl}" +
				$"&extraData=";


		public string SignSHA256(string msg, string secretKey)
		{
			byte[] result;
			byte[] secretKeyByte = Encoding.UTF8.GetBytes(secretKey);
			byte[] msgByte = Encoding.UTF8.GetBytes(msg);
			using (var sha = new HMACSHA256(secretKeyByte))
			{
				result = sha.ComputeHash(msgByte);
				return BitConverter.ToString(result).ToLower().Replace("-", "");
			}
		}

		public string CreateRequestBody(MoMoRequestData request) => JsonConvert.SerializeObject(new
		{
			accessKey = request.AccessKey,
			partnerCode = request.PartnerCode,
			requestType = request.RequestType,
			notifyUrl = request.NotifyUrl,
			returnUrl = request.ReturnUrl,
			orderId = request.OrderID,
			amount = request.Amount,
			orderInfo = request.OrderInfo,
			requestId = request.OrderID,
			extraData = request.ExtraData,
			signature = SignSHA256(CreateRawData(request), request.SecretKey)
		});


		public async Task<MoMoResponse> SendMoMoRequest(MoMoRequestData request)
		{
			var httpRequestMessage = new HttpRequestMessage();
			httpRequestMessage.Method = HttpMethod.Post;
			httpRequestMessage.RequestUri = new Uri(request.Endpoint);
			var body = CreateRequestBody(request);
			HttpContent httpContent = new StringContent(body, Encoding.UTF8, "application/json");
			httpRequestMessage.Content = httpContent;
			try
			{
				Debug.WriteLine(httpRequestMessage.Headers.ToString());
				var t = await client.SendAsync(httpRequestMessage);
				string responseBody = "";
				using (var streamReader = new StreamReader(t.Content.ReadAsStream()))
				{
					string tmp = null;
					while ((tmp = streamReader.ReadLine()) != null)
					{
						responseBody += tmp;
					}
				}
				return JsonConvert.DeserializeObject<MoMoResponse>(responseBody);
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		public MoMoRequestData CreateRequestData(string orderInfo, string amount, string returnUrl) => new MoMoRequestData()
		{
			OrderInfo = orderInfo,
			Amount = amount,
			ReturnUrl = returnUrl.IsNullOrEmpty() ? _config["MoMoRequest:ReturnUrl"] : returnUrl,
			NotifyUrl = returnUrl.IsNullOrEmpty() ? _config["MoMoRequest:ReturnUrl"] : returnUrl,
			PartnerCode = _config["MoMoRequest:PartnerCode"],
			Endpoint = _config["MoMoRequest:Endpoint"],
			AccessKey = _config["MoMoRequest:AccessKey"],
			SecretKey = _config["MoMoRequest:SecretKey"],
			RequestType = _config["MoMoRequest:RequestType"],
			ExtraData = _config["MoMoRequest:ExtraData"],
			OrderID = DateTime.UtcNow.Ticks.ToString() + orderInfo.GetHashCode(),
		};

	}
}
