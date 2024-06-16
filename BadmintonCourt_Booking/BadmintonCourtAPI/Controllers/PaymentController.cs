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

		[HttpGet]
		//[Authorize(Roles = "Admin,Staff")]
		[Route("Payment/GetAll")]
		public async Task<ActionResult<IEnumerable<Payment>>> GetAllPayments() => Ok(_service.PaymentService.GetAllPayments());

		[HttpGet]
		[Route("Payment/GetByOrder")]
		//[Authorize]
		public async Task<ActionResult<IEnumerable<Payment>>> GetAllPaymentsByOrder(int order, DateOnly start, DateOnly end)
		{
			if (start > end)
				return BadRequest(new { msg = "Invalid time interval" });

			if (order == 1) // Sort tăng dần theo ngày
				return Ok(_service.PaymentService.GetAllPayments().Where(x => x.Date <= new DateTime(start.Year, start.Month, start.Day, 0, 0, 0) && x.Date <= new DateTime(end.Year, end.Month, end.Day, 23, 59, 59)).OrderBy(x => x.Amount).ToList());

			return Ok(_service.PaymentService.GetAllPayments().Where(x => x.Date <= new DateTime(start.Year, start.Month, start.Day, 0, 0, 0) && x.Date <= new DateTime(end.Year, end.Month, end.Day, 23, 59, 59)).OrderByDescending(x => x.Amount).ToList());
		}


		[HttpGet]
		[Route("Payment/GetByUser")]
		//[Authorize]
		public async Task<ActionResult<IEnumerable<Payment>>> GetUserPayments(string id) => Ok(_service.PaymentService.GetPaymentsByUserId(id));

		[HttpGet]
		[Route("Payment/GetByUserOrder")]
		//[Authorize]
		public async Task<ActionResult<IEnumerable<Payment>>> GetUserPaymentsByOrder(int order, DateOnly start, DateOnly end, string id)
		{
			if (start > end)
				return BadRequest(new { msg = "Invalid time interval" });

			if (order == 1) // Sort tăng dần theo ngày
				return Ok(_service.PaymentService.GetPaymentsByUserId(id).Where(x => x.Date <= new DateTime(start.Year, start.Month, start.Day, 0, 0, 0) && x.Date <= new DateTime(end.Year, end.Month, end.Day, 23, 59, 59)).OrderBy(x => x.Amount).ToList());

			return Ok(_service.PaymentService.GetPaymentsByUserId(id).Where(x => x.Date <= new DateTime(start.Year,  start.Month, start.Day, 0, 0, 0) && x.Date <= new DateTime(end.Year, end.Month, end.Day, 23, 59, 59)).OrderByDescending(x => x.Amount).ToList());
		}

		[HttpGet]
		[Route("Payment/GetBySearch")]
		//[Authorize]
		public async Task<ActionResult<IEnumerable<Payment>>> GetPaymentsBySearch(string? id, string? search) => Ok(_service.PaymentService.GetPaymentsBySearch(id, search));

		[HttpPost]
		//[Authorize]
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
			UserDetail info = _service.UserDetailService.GetUserDetailById(model.UserId);
			Court court = _service.CourtService.GetCourtByCourtId(model.CourtId);
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
					double price = _service.CourtService.GetCourtByCourtId("C001").Price;
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
		//[Authorize]
		[Route("Payment/Result")]
		public async Task<ActionResult> VnPayPaymentCallBack()
		{
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
				_service.PaymentService.AddPayment(new Payment { PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"), Date = result.Date, Method = "VnPay", UserId = userId, TransactionId = result.TransactionId, Amount = amount });
				User user = _service.UserService.GetUserById(userId);
				user.Balance += result.Amount / _service.CourtService.GetCourtByCourtId("S1").Price;
				_service.UserService.UpdateUser(user, userId);
			}

			else if (dayList.Count == 1) // 1 lan choi
			{
				_service.BookingService.AddBooking(new Booking { BookingId = "BK" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7"), BookingType = 1, Amount = amount, UserId = userId });
				_service.PaymentService.AddPayment(new Payment { PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"), BookingId = _service.BookingService.GetRecentAddedBooking().BookingId, Date = result.Date, Method = "1 lan choi", UserId = userId, TransactionId = result.TransactionId });
				_service.SlotService.AddSlot(new Slot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = _service.BookingService.GetRecentAddedBooking().BookingId, CourtId = courtId, Status = false, StartTime = new DateTime(dayList[0].Year, dayList[0].Month, dayList[0].Day, int.Parse(result.Interval.Split('-')[0]), 0, 0), EndTime = new DateTime(dayList[0].Year, dayList[0].Month, dayList[0].Day, int.Parse(result.Interval.Split('-')[1]), 0, 0) });
			}
			else // Co dinh - 1 thang, 2 thang, 3 thang, ...
			{
				_service.BookingService.AddBooking(new Booking { BookingId = "BK" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7"), BookingType = 2, Amount = amount, UserId = userId });
				_service.PaymentService.AddPayment(new Payment { PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"), BookingId = _service.BookingService.GetRecentAddedBooking().BookingId, Date = result.Date, Method = "VnPay", UserId = userId, TransactionId = result.TransactionId });
				for (int i = 0; i < dayList.Count; i++)
					_service.SlotService.AddSlot(new Slot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = _service.BookingService.GetRecentAddedBooking().BookingId, CourtId = courtId, Status = false, StartTime = new DateTime(dayList[i].Year, dayList[i].Month, dayList[0].Day, int.Parse(result.Interval.Split('-')[0]), 0, 0), EndTime = new DateTime(dayList[0].Year, dayList[0].Month, dayList[0].Day, int.Parse(result.Interval.Split('-')[1]), 0, 0) });
			}
			return Ok();
		}
	}
}
