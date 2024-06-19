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
using Microsoft.IdentityModel.Tokens;
using BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.MoMo;

namespace BadmintonCourtAPI.Controllers
{
	public class PaymentController : Controller
	{
		private readonly BadmintonCourtService _service = null;
		private const string PLAY_ONCE = "once";
		private const string FLEXIBLE = "flexible";
		private const string FIXED = "fixed";
		public PaymentController(IConfiguration configuration)
		{
			if (_service == null)
				_service = new BadmintonCourtService(configuration);
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
		public async Task<ActionResult<IEnumerable<Payment>>> GetUserPaymentsByDemand(int order, DateOnly start, DateOnly end, string id)
		{
			if (start > end)
				return BadRequest(new { msg = "Invalid time interval" });

			if (order == 1) // Sort tăng dần theo ngày
				return Ok(_service.PaymentService.GetPaymentsByUserId(id).Where(x => x.Date >= new DateTime(start.Year, start.Month, start.Day, 0, 0, 0) && x.Date <= new DateTime(end.Year, end.Month, end.Day, 23, 59, 59)).OrderBy(x => x.Amount).ToList());

			return Ok(_service.PaymentService.GetPaymentsByUserId(id).Where(x => x.Date >= new DateTime(start.Year, start.Month, start.Day, 0, 0, 0) && x.Date <= new DateTime(end.Year, end.Month, end.Day, 23, 59, 59)).OrderByDescending(x => x.Amount).ToList());
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

			if (model.Method == "vnpay") // VNPAY
			{
				var vnPay = await PayByVnPay(model, info, court);
			}
			else if (model.Method == "momo")
			{
				return Ok(await PayByMoMo(model, info, court));
			}
			return Ok();
		}

		public async Task<ActionResult> PayByVnPay(TransactionDTO model, UserDetail info, Court court)
		{
			string content = "";
			if (model.Type == PLAY_ONCE)  // Đặt loại 1 lần chơi
			{
				content = $"User: {info.FirstName} {info.LastName} | ID: {model.UserId} | Phone: {info.Phone} | Mail: {info.Email} | Date: {model.Date} {model.Start}h - {model.End}h | Court: {model.CourtId}";
				_service.VnPayService.CreatePaymentUrl(HttpContext, new VnPayRequestDTO { Amount = (int.Parse(model.End) - int.Parse(model.Start)) * court.Price * 1000, Content = content, Date = DateTime.Now, UserId = model.UserId, DaysList = model.DaysList, Interval = $"{model.Start}-{model.End}" });
			}

			else if (model.Type == FLEXIBLE) // Linh hoạt
			{
				double price = _service.CourtService.GetCourtByCourtId("C001").Price;
				content = $"User: {info.FirstName} {info.LastName} | ID: {model.UserId} | Phone: {info.Phone} | Mail: {info.Email} | Amount: {model.Amount}";
				_service.VnPayService.CreatePaymentUrl(HttpContext, new VnPayRequestDTO { Amount = double.Parse(model.Amount), Content = content, Date = DateTime.Now, UserId = model.UserId });
			}
			else // Cố định. Vd: ngày 1/1/2001 15h-17h 2 tháng sân A
			{
				content = $"User: {info.FirstName} {info.LastName} | ID: {model.UserId} | Phone: {info.Phone} | Mail: {info.Email} | Start date on schedule: {model.Date} {model.Start}h - {model.End}h | Court: {model.CourtId} | Number of months: {model.NumMonth}";
				_service.VnPayService.CreatePaymentUrl(HttpContext, new VnPayRequestDTO { Amount = (int.Parse(model.End) - int.Parse(model.Start)) * court.Price * model.NumMonth * 4 * 1000, Content = content, Date = DateTime.Now, UserId = model.UserId, DaysList = model.DaysList, Interval = $"{model.Start}-{model.End}" });
			}
			return Ok();
		}

		public async Task<MoMoResponse> PayByMoMo(TransactionDTO model, UserDetail info, Court court)
		{
			string amount = model.Amount;
			string orderInfo = $"User: {info.FirstName} {info.LastName}, date: {DateTime.Now.ToString("dd/MM/yyyy")}, type: {model.Type}";
			string type = model.Type;
			switch (type)
			{
				case FIXED:
					orderInfo += $"\nReserves for {model.NumMonth} month(s), from {model.Date} {model.Start}h - {model.End}h, at court: {model.CourtId}.";
					break;
				case PLAY_ONCE:
					orderInfo += $"\nPlay on {model.Date.ToString("dd/MM/yyyy")}";
					break;
				case FLEXIBLE:
					orderInfo += $"\n";
					break;
				default: throw new NotImplementedException("Other booking types are not implemented yet");
			}
			var reqdata = MoMoServices.CreateRequestData(orderInfo, amount);
			var response = await MoMoServices.SendMoMoRequest(reqdata);
			return response;
		}
		[HttpGet]
		[Route("/Payment/MoMoCallback")]
		public ActionResult MoMoCallback(MoMoResponse response)
		{
			return Ok();
		}
		[HttpPost]
		//[Authorize]
		[Route("Payment/Result")]
		public async Task<ActionResult> VnPayPaymentCallBack()
		{
			VnPayResponseDTO result = _service.VnPayService.PaymentExecute(Request.Query);
			if (result == null || result.VnPayResponseCode != "00" && result.Status == false)
				return BadRequest(new { msg = "Fail" });

			string userId = result.Description.Split('|')[1].Trim().Split(':')[1].Trim();
			string courtId = result.Description.Split('|')[5].Trim().Split(':')[1].Trim();
			string rawDayList = result.DayList;
			double amount = result.Amount;
			List<DateOnly> dayList = new List<DateOnly>();
			string[] tmpStorage = rawDayList.Split('|');
			for (int i = 0; i < tmpStorage.Length; i++)
				dayList.Add(DateOnly.Parse(tmpStorage[i].Trim()));
			User user = _service.UserService.GetUserById(userId);


			if (dayList.IsNullOrEmpty()) // Linh hoạt
			{
				_service.PaymentService.AddPayment(new Payment { PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"), Date = result.Date, Method = 1, UserId = userId, TransactionId = result.TransactionId, Amount = amount });
				user.Balance += result.Amount / 1000;
				_service.UserService.UpdateUser(user, userId);
			}

			else if (dayList.Count == 1) // 1 lan choi
			{
				string bookingId = "BK" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7");
				_service.BookingService.AddBooking(new Booking { BookingId = bookingId, BookingType = 1, Amount = amount / 1000, UserId = userId });
				_service.PaymentService.AddPayment(new Payment { PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"), BookingId = bookingId, Date = result.Date, Method = 1, UserId = userId, TransactionId = result.TransactionId, Amount = amount });
				_service.SlotService.AddSlot(new BookedSlot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = bookingId, CourtId = courtId, StartTime = new DateTime(dayList[0].Year, dayList[0].Month, dayList[0].Day, int.Parse(result.Interval.Split('-')[0]), 0, 0), EndTime = new DateTime(dayList[0].Year, dayList[0].Month, dayList[0].Day, int.Parse(result.Interval.Split('-')[1]), 0, 0) });
			}
			else if (dayList.Count > 1)// Co dinh - 1 thang, 2 thang, 3 thang, ...
			{
				string bookingId = "BK" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7");
				_service.BookingService.AddBooking(new Booking { BookingId = bookingId, BookingType = 2, Amount = amount / 1000, UserId = userId });
				_service.PaymentService.AddPayment(new Payment { PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"), BookingId = bookingId, Date = result.Date, Method = 1, UserId = userId, TransactionId = result.TransactionId, Amount = amount });
				for (int i = 0; i < dayList.Count; i++)
					_service.SlotService.AddSlot(new BookedSlot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = bookingId, CourtId = courtId, StartTime = new DateTime(dayList[i].Year, dayList[i].Month, dayList[0].Day, int.Parse(result.Interval.Split('-')[0]), 0, 0), EndTime = new DateTime(dayList[0].Year, dayList[0].Month, dayList[0].Day, int.Parse(result.Interval.Split('-')[1]), 0, 0) });
			}
			//----------------------------------------------
			List<Discount> discountList = _service.DiscountService.GetAllDiscounts().OrderByDescending(x => x.Amount).ToList();
			foreach (Discount discount in discountList)
			{
				if (amount >= discount.Amount)
				{
					amount = amount / 1000;
					user.Balance += (amount * discount.Proportion) / 100;
					_service.UserService.UpdateUser(user, userId);
					break;
				}
			}
			return Ok(new { msg = "Success" });
		}
	}
}
