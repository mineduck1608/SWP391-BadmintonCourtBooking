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


		public string CreatePaymentUrl(HttpContext context, VnPayRequestDTO vnPayRequestDTO)
		{
			_vnPayLib.AddRequestData("vnp_Version", VnPayLib.VERSION);
			_vnPayLib.AddRequestData("vnp_Command", "pay");
			_vnPayLib.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
			_vnPayLib.AddRequestData("vnp_Amount", (vnPayRequestDTO.Amount * 100).ToString());
			//Số tiền thanh toán. Số tiền không 
			//mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND
			//(một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần(khử phần thập phân), sau đó gửi sang VNPAY
			//là: 10000000
			_vnPayLib.AddRequestData("vnp_CreateDate", vnPayRequestDTO.Date.ToString("yyyyMMddHHmmss"));
			_vnPayLib.AddRequestData("vnp_CurrCode", "VND");
			_vnPayLib.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
			_vnPayLib.AddRequestData("vnp_Locale", "vn");
			_vnPayLib.AddRequestData("vnp_OrderInfo", vnPayRequestDTO.Content);
			_vnPayLib.AddRequestData("vnp_OrderType", "other"); //default value: other
			_vnPayLib.AddRequestData("vnp_ReturnUrl", _config["VnPay:ReturnUrl"]);
			_vnPayLib.AddRequestData("vnp_TxnRef", vnPayRequestDTO.UserId + DateTime.Now.Ticks.ToString()); // Mã tham chiếu của giao dịch tại hệ
			_vnPayLib.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString());
			if (vnPayRequestDTO.Type == "Cố định" || vnPayRequestDTO.Type == "1 lần chơi")
			{
				//_vnPayDAO.AddRequestData("Day List", ToHashString(vnPayRequestDTO.DaysList));
				_vnPayLib.AddRequestData("Day_List", vnPayRequestDTO.DaysList);
				_vnPayLib.AddRequestData("Interval", vnPayRequestDTO.Interval);
			}
			return _vnPayLib.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);
		}

		public VnPayResponseDTO PaymentExecute(IQueryCollection collection)
		{
			foreach (var (key, value) in collection)
				if (!key.IsNullOrEmpty() && key.StartsWith("vnp_") || key.StartsWith("Day_List") || key.StartsWith("Interval"))
					_vnPayLib.AddRequestData(key, value.ToString());

			var vnp_orderId = Convert.ToInt64(_vnPayLib.GetResponseData("vnp_TxnRef"));
			var vnp_transactionId = _vnPayLib.GetResponseData("vnp_TransactionNo");
			var vnp_SecureHash = collection.FirstOrDefault(x => x.Key == "vnp_SecureHash").Value;
			var vnp_ResponseCode = _vnPayLib.GetResponseData("vnp_ResponseCode");
			var vnp_OrderInfo = _vnPayLib.GetResponseData("vnp_OrderInfo");
			var amount = _vnPayLib.GetResponseData("vnp_Amount");

			if (!_vnPayLib.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]))
				return new VnPayResponseDTO { Status = false, VnPayResponseCode = vnp_ResponseCode };
			else
			{
				var dayList = _vnPayLib.GetResponseData("Day_List");
				var interval = _vnPayLib.GetResponseData("Interval");
				return new VnPayResponseDTO // Cố định | 1 lần chơi
				{
					Status = true,
					PaymentMethod = "1",
					Description = vnp_OrderInfo,
					BookingId = vnp_orderId.ToString(),
					TransactionId = vnp_transactionId.ToString(),
					Token = vnp_SecureHash,
					VnPayResponseCode = vnp_ResponseCode,
					DayList = dayList,
					Interval = interval,
					Amount = double.Parse(amount),
					Date = DateTime.Now
				};
			}
		}

	}
}
