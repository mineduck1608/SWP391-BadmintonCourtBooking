using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.VnPay;
using BadmintonCourtBusinessObjects.SupportEntities.Payment;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Security.Cryptography;
using System.Text;
using BadmintonCourtAPI.Utils;

namespace BadmintonCourtAPI.Controllers
{
	public class PaymentController : Controller
	{
		private readonly BadmintonCourtService _service;

		public PaymentController(BadmintonCourtService service)
		{
			_service = service;
		}

		[HttpPost]
		[Authorize]
		[Route("Booking/TransactionProcess")]
		public async Task<ActionResult> Payment(TransactionDTO model)
		{
			// Method: phương thức thanh toán: momo, vnpay, ...
			// id: user id
			// court id: mã sân
			// date: ngày bắt đầu chơi - ngày hẹn trên lịch
			// type: loại đặt: linh động, cố định, 1 lần chơi
			// start - end: khoảng thời gian chơi. VD: 3h - 5h ngày x/x/xxxx
			// numMonth: số tháng chơi (giành cho đặt cố định)
			UserDetail info = _service.userDetailService.GetUserDetailById(model.UserId);
			Court court = _service.courtService.GetCourtByCourtId(model.CourtId);
			string content = "";  // Nội dung giao dịch đc add tự động vào app bank lúc giao dịch
								  //------------------------------------------------------------

			if (model.Method == "vnpay") // VNPAY
			{
				if (model.Type == "1 lần chơi")  // Đặt loại 1 lần chơi
				{
					content = $"User: {info.FirstName} {info.LastName} | ID: {model.UserId} | Phone: {info.Phone} | Mail: {info.Email} | Date: {model.Date} {model.Start}h - {model.End}h | Court: {model.CourtId}";
					_service.VnPayService.CreatePaymentUrl(HttpContext, new VnPayRequestDTO { Amount = (int.Parse(model.End) - int.Parse(model.Start)) * court.Price, Content = content, Date = DateTime.Now, OrderId = "VNP" + new Random().Next(1000, 100000), UserId = int.Parse(model.UserId), DaysList = model.DaysList, Interval = $"{model.Start}-{model.End}" });
				}

				else if (model.Type == "linh hoạt") // Linh hoạt
				{
					double price = _service.courtService.GetCourtByCourtId("C001").Price;
					content = $"User: {info.FirstName} {info.LastName} | ID: {model.UserId} | Phone: {info.Phone} | Mail: {info.Email} | Hours: {model.Hours}";
					_service.VnPayService.CreatePaymentUrl(HttpContext, new VnPayRequestDTO { Amount = int.Parse(model.Hours) * price, Content = content, Date = DateTime.Now, OrderId = "VNP" + new Random().Next(1000, 100000), UserId = int.Parse(model.UserId) });
				}
				else // Cố định. Vd: ngày 1/1/2001 15h-17h 2 tháng sân A
				{
					content = $"User: {info.FirstName} {info.LastName} | ID: {model.UserId} | Phone: {info.Phone} | Mail: {info.Email} | Start date on schedule: {model.Date} {model.Start}h - {model.End}h | Court: {model.CourtId} | Number of months: {model.NumMonth}";
					_service.VnPayService.CreatePaymentUrl(HttpContext, new VnPayRequestDTO { Amount = (int.Parse(model.End) - int.Parse(model.Start)) * court.Price * model.NumMonth * 4, Content = content, Date = DateTime.Now, OrderId = "VNP" + new Random().Next(1000, 100000), UserId = int.Parse(model.UserId), DaysList = model.DaysList, Interval = $"{model.Start}-{model.End}" });
				}
				return Ok();
			}
			else if (model.Method == "momo")
			{
				return Ok();
			}
			return Ok();
		}


		[HttpPost]
		[Authorize]
		[Route("Payment/Result")]
		public async Task<ActionResult> PaymentCallBack()
		{
			// Demo VnPay
			VnPayResponseDTO result = _service.VnPayService.PaymentExecute(Request.Query);
			if (result == null || result.VnPayResponseCode != "00" && result.Status == false)
				return BadRequest(new { VnPayResponseCode = $"{result.VnPayResponseCode}" });

			string userId = result.Description.Split('|')[1].Trim().Split(':')[1].Trim();
			string courtId = result.Description.Split('|')[5].Trim().Split(':')[1].Trim();
			string rawDayList = result.DayList;
			double amount = result.Amount;
			List<DateOnly> dayList = new List<DateOnly>();
			string[] tmpStorage = rawDayList.Split('|');
			for (int i = 0; i < tmpStorage.Length; i++)
				dayList.Add(DateOnly.Parse(tmpStorage[i].Trim()));

			if (dayList.Count == 0) // Linh hoạt
			{
				_service.paymentService.AddPayment(new Payment { PaymentId = Util.GeneratPaymentId(_service), BookingId = null, Date = result.Date, Method = "Flexible", UserId = userId, TransactionId = result.TransactionId, Amount = amount });
				User user = _service.userService.GetUserById(userId);
				user.Balance += result.Amount / _service.courtService.GetCourtByCourtId("S1").Price;
				_service.userService.UpdateUser(user, userId);
			}

			else if (dayList.Count == 1) // 1 lan choi
			{
				_service.bookingService.AddBooking(new Booking { BookingId = Util.GenerateBookingId(_service), BookingType = 1, Amount = amount, UserId = userId });
				_service.paymentService.AddPayment(new Payment { PaymentId = Util.GeneratPaymentId(_service), BookingId = _service.bookingService.GetRecentAddedBooking().BookingId, Date = result.Date, Method = "1 lan choi", UserId = userId, TransactionId = result.TransactionId });
				_service.slotService.AddSlot(new Slot { SlotId = Util.GenerateSlotId(_service), BookingId = _service.bookingService.GetRecentAddedBooking().BookingId, CourtId = courtId, Status = false, StartTime = new DateTime(dayList[0].Year, dayList[0].Month, dayList[0].Day, int.Parse(result.Interval.Split('-')[0]), 0, 0), EndTime = new DateTime(dayList[0].Year, dayList[0].Month, dayList[0].Day, int.Parse(result.Interval.Split('-')[1]), 0, 0) });
			}
			else // Co dinh - 1 thang, 2 thang, 3 thang, ...
			{
				_service.bookingService.AddBooking(new Booking { BookingId = Util.GenerateBookingId(_service), BookingType = 2, Amount = amount, UserId = userId });
				_service.paymentService.AddPayment(new Payment { PaymentId = Util.GeneratPaymentId(_service), BookingId = _service.bookingService.GetRecentAddedBooking().BookingId, Date = result.Date, Method = "Fixed", UserId = userId, TransactionId = result.TransactionId });
				for (int i = 0; i < dayList.Count; i++)
					_service.slotService.AddSlot(new Slot { SlotId = Util.GenerateSlotId(_service), BookingId = _service.bookingService.GetRecentAddedBooking().BookingId, CourtId = courtId, Status = false, StartTime = new DateTime(dayList[i].Year, dayList[i].Month, dayList[0].Day, int.Parse(result.Interval.Split('-')[0]), 0, 0), EndTime = new DateTime(dayList[0].Year, dayList[0].Month, dayList[0].Day, int.Parse(result.Interval.Split('-')[1]), 0, 0) });
			}
			// Demo Momo: on going
			return Ok();
		}
	}
}
