using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoMo
{
	public class Services
	{
		static readonly HttpClient client = new HttpClient();
		public static string CreateRawData(MomoRequest request)
		{
			return $"partnerCode={request.PartnerCode}&accessKey={request.AccessKey}&requestId={request.OrderID}&amount={request.Amount}&orderId={request.OrderID}&orderInfo={request.OrderInfo}&returnUrl={request.ReturnUrl}&notifyUrl={request.NotifyUrl}&extraData=";
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

		public static string CreateRequestBody(MomoRequest request)
		{
			var requestData = new
			{
				accessKey = request.AccessKey,//
				partnerCode = request.PartnerCode,//
				requestType = request.RequestType,//
				notifyUrl = request.NotifyUrl,//
				returnUrl = request.ReturnUrl,//
				orderId = request.OrderID,//
				amount = request.Amount,//
				orderInfo = request.OrderInfo,//
				requestId = request.OrderID,//
				extraData = "",//
				signature = SignSHA256(CreateRawData(request), request.SecretKey)
			};
			return JsonConvert.SerializeObject(requestData);
		}

		public static async Task<MomoResponse> SendRequest(MomoRequest request)
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
				return JsonConvert.DeserializeObject<MomoResponse>(responseBody);
			}
			catch (Exception ex)
			{
				return null;
			}
        }
	}
}
