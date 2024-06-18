using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata.Ecma335;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Net.Sockets;

namespace BadmintonCourtDAOs
{
	public class VnPayLib
	{
		public const string VERSION = "2.1.0";
		private SortedList<String, String> _requestData = new SortedList<String, String>(new VnPayCompare());
		private SortedList<String, String> _responseData = new SortedList<String, String>(new VnPayCompare());


		public void AddRequestData(string key, string value)
		{
			if (!value.IsNullOrEmpty())
				_requestData.Add(key, value);
		}

		public void AddResponseData(string key, string value)
		{
			if (!value.IsNullOrEmpty())
				_responseData.Add(key, value);
		}


		public string? GetResponseData(string key) => _responseData.TryGetValue(key, out string retValue) ? retValue : String.Empty;

		#region Request

		public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
		{
			StringBuilder data = new StringBuilder();
			foreach (KeyValuePair<string, string> kv in _requestData)
			{
				if (!String.IsNullOrEmpty(kv.Value))
				{
					data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
				}
			}
			string queryString = data.ToString();

			baseUrl += "?" + queryString;
			String signData = queryString;
			if (signData.Length > 0)
			{

				signData = signData.Remove(data.Length - 1, 1);
			}
			string vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);
			baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

			return baseUrl;
		}



		#endregion

		#region Response process

		public bool ValidateSignature(string inputHash, string secretKey) => Utils.HmacSHA512(secretKey, GetResponseData()).Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);

		private string GetResponseData()
		{
			StringBuilder data = new StringBuilder();
			if (_responseData.ContainsKey("vnp_SecureHashType"))
				_responseData.Remove("vnp_SecureHashType");
			if (_responseData.ContainsKey("vnp_SecureHash"))
				_responseData.Remove("vnp_SecureHash");
			foreach (KeyValuePair<string, string> kv in _responseData)
			{
				if (!String.IsNullOrEmpty(kv.Value))
				{
					data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
				}
			}
			//remove last '&'
			if (data.Length > 0)
				data.Remove(data.Length - 1, 1);
			return data.ToString();
		}

		#endregion
	}

	public class Utils
	{

		public static String HmacSHA512(string key, String inputData)
		{
			var hash = new StringBuilder();
			byte[] keyBytes = Encoding.UTF8.GetBytes(key);
			byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
			using (var hmac = new HMACSHA512(keyBytes))
			{
				byte[] hashValue = hmac.ComputeHash(inputBytes);
				foreach (var theByte in hashValue)
					hash.Append(theByte.ToString("x2"));
			}
			return hash.ToString();
		}


		public static string GetIpAddress(HttpContext context)
		{
			var ipAddress = string.Empty;
			try
			{
				var remoteIpAddress = context.Connection.RemoteIpAddress;

				if (remoteIpAddress != null)
				{
					if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
					{
						remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
							.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
					}

					if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();

					return ipAddress;
				}
			}
			catch (Exception ex)
			{
				return "Invalid IP:" + ex.Message;
			}

			return "127.0.0.1";
		}


		public static string GenerateId() => new Guid().ToString("N").Substring(0, 10);

		public static string GenerateTxnRefId() => new Guid().ToString().Substring(0, 10);
	}

	public class VnPayCompare : IComparer<string>
	{
		public int Compare(string x, string y)
		{
			if (x == y) return 0;
			if (x == null) return -1;
			if (y == null) return 1;
			var vnpCompare = CompareInfo.GetCompareInfo("en-US");
			return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
		}
	}
}
