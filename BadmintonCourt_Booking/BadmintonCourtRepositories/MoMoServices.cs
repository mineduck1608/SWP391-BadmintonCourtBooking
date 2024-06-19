using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.MoMo;
using Microsoft.Extensions.Configuration;
using BadmintonCourtServices.IService;
using Microsoft.Extensions.Logging;

namespace BadmintonCourtServices
{
	public class MoMoServices : IMoMoServices
	{
		private static IConfiguration _config = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", true, true).Build();
		static readonly HttpClient client = new HttpClient();

		public static string CreateRawData(MoMoRequestData request)
		{
			return $"partnerCode={request.PartnerCode}" +
				$"&accessKey={request.AccessKey}" +
				$"&requestId={request.OrderID}" +
				$"&amount={request.Amount}" +
				$"&orderId={request.OrderID}" +
				$"&orderInfo={request.OrderInfo}" +
				$"&returnUrl={request.ReturnUrl}" +
				$"&notifyUrl={request.NotifyUrl}" +
				$"&extraData=";
		}

		public static string SignSHA256(string msg, string secretKey)
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

		public static string CreateRequestBody(MoMoRequestData request)
		{
			var requestData = new
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
				extraData = "",
				signature = SignSHA256(CreateRawData(request), request.SecretKey)
			};
			return JsonConvert.SerializeObject(requestData);
		}

		public static async Task<MoMoResponse> SendMoMoRequest(MoMoRequestData request)
		{
			var httpRequestMessage = new HttpRequestMessage();
			httpRequestMessage.Method = HttpMethod.Post;
			httpRequestMessage.RequestUri = new Uri(request.Endpoint);
			var body = CreateRequestBody(request);
			HttpContent httpContent = new StringContent(body, Encoding.UTF8, "application/json");
			httpRequestMessage.Content = httpContent;
			try
			{
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
		public static MoMoRequestData CreateRequestData(string orderInfo, string amount, string returnUrl)
		{
			return new MoMoRequestData()
			{
				OrderInfo = orderInfo,
				Amount = amount,
				ReturnUrl = returnUrl,
				NotifyUrl = returnUrl,
				PartnerCode = _config["MoMoRequest:PartnerCode"],
				Endpoint = _config["MoMoRequest:Endpoint"],
				AccessKey = _config["MoMoRequest:AccessKey"],
				SecretKey = _config["MoMoRequest:SecretKey"],
				RequestType = _config["MoMoRequest:RequestType"],
				ExtraData = _config["MoMoRequest:ExtraData"],
				OrderID = DateTime.UtcNow.Ticks.ToString() + orderInfo.GetHashCode(),
			};
		}
		public static MoMoRequestData CreateRequestData(string orderInfo, string amount)
		{
			return CreateRequestData(orderInfo, amount, _config["MoMoRequest:ReturnUrl"]);
		}
	}
}
