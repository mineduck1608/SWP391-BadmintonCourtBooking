using BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.VnPay;
using BadmintonCourtDAOs;
using BadmintonCourtServices.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices
{
	public class VnPayService : IVnPayService
	{

		private readonly IConfiguration _config = null;
		private readonly VnPayLib _vnPayLib = null;

		public VnPayService(IConfiguration config)
		{
			if (_config == null)
				_config = config;
			if (_vnPayLib == null)
				_vnPayLib = new VnPayLib();
		}


		public string CreatePaymentUrl(HttpContext context, VnPayRequestDTO vnPayRequestDTO, string? returnUrl)
		{
			returnUrl = returnUrl.IsNullOrEmpty() ? _config["VnPay:ReturnUrl"] : returnUrl;
			_vnPayLib.AddRequestData("vnp_Version", VnPayLib.VERSION);
			_vnPayLib.AddRequestData("vnp_Command", "pay");
			_vnPayLib.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
			_vnPayLib.AddRequestData("vnp_Amount", (vnPayRequestDTO.Amount * 100).ToString());
			//Số tiền thanh toán. Số tiền không 
			//mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND
			//(một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần(khử phần thập phân), sau đó gửi sang VNPAY
			//là: 10000000
			_vnPayLib.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
			_vnPayLib.AddRequestData("vnp_CurrCode", "VND");
			_vnPayLib.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
			_vnPayLib.AddRequestData("vnp_Locale", "vn");
			_vnPayLib.AddRequestData("vnp_OrderInfo", vnPayRequestDTO.Content);
			_vnPayLib.AddRequestData("vnp_OrderType", "other"); //default value: other
			_vnPayLib.AddRequestData("vnp_ReturnUrl", returnUrl);
			_vnPayLib.AddRequestData("vnp_TxnRef", $"{DateTime.Now.Ticks.ToString()}"); // Mã tham chiếu của giao dịch tại
			string url = _vnPayLib.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);
			return url;
		}

		public VnPayResponseDTO PaymentExecute(IQueryCollection collection)
		{
			foreach (var (key, value) in collection)
				if (!key.IsNullOrEmpty() && key.StartsWith("vnp_"))
					_vnPayLib.AddResponseData(key, value.ToString());

			//string vnp_orderId = $"{_vnPayLib.GetResponseData("vnp_TxnRef")}";
			var vnp_transactionId = _vnPayLib.GetResponseData("vnp_TransactionNo");
			var vnp_SecureHash = collection.FirstOrDefault(x => x.Key == "vnp_SecureHash").Value;
			var vnp_ResponseCode = _vnPayLib.GetResponseData("vnp_ResponseCode");
			var vnp_OrderInfo = _vnPayLib.GetResponseData("vnp_OrderInfo");
			var amount = _vnPayLib.GetResponseData("vnp_Amount");

			if (!_vnPayLib.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]))
				return new VnPayResponseDTO { Status = false, VnPayResponseCode = vnp_ResponseCode };

			return new VnPayResponseDTO
			{
				Status = true,
				Description = vnp_OrderInfo,
				TransactionId = vnp_transactionId.ToString(),
				VnPayResponseCode = vnp_ResponseCode,
				Amount = double.Parse(amount),
				Date = DateTime.Now
			};

		}

	}
}
