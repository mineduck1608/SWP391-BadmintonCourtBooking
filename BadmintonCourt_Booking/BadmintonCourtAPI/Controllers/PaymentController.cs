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
using System.Diagnostics;
using Newtonsoft.Json;
using BadmintonCourtDAOs;

namespace BadmintonCourtAPI.Controllers
{
	public class PaymentController : Controller
	{
		private readonly BadmintonCourtService _service = null;
		private const string PLAY_ONCE = "once";
		private const string FLEXIBLE = "flexible";
		private const string FIXED = "fixed";
		private const string BUY_TIME = "buyTime";
		private IConfiguration _configuration;
		public PaymentController(IConfiguration configuration)
		{
			_configuration = configuration;
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
				return Ok(await PayByMoMo(model, info));
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

		public async Task<MoMoResponse> PayByMoMo(TransactionDTO model, UserDetail info)
		{
			Debug.WriteLine(model.Date);
			string amount = model.Amount;
			string type = model.Type;
			string orderInfo = $"user: {info.FirstName} {info.LastName}, id: {model.UserId}, date: {DateTime.Now:yyyy-MM-dd}, type: {type}, ";
			switch (type)
			{
				case PLAY_ONCE:
					orderInfo += $"court: {model.CourtId}, on: {model.Date}, timeStart: {model.Start}, timeEnd: {model.End}";
					break;
				case FIXED:
					orderInfo += $"court: {model.CourtId}, number of months: {model.NumMonth}, on: {model.Date}, timeStart: {model.Start}, timeEnd: {model.End}";
					break;
				case FLEXIBLE:
				case BUY_TIME:
					//Not doing anything. The hour will be subtracted onto user's balance. This is here to check if the type is correct
					break;
				default: throw new NotImplementedException("Other booking types are not implemented yet");
			}
			var reqdata = MoMoServices.CreateRequestData(orderInfo, amount);
			var response = await MoMoServices.SendMoMoRequest(reqdata);
			return response;
		}
		[HttpGet]
		[Route("/Payment/MoMoCallback")]
		public async Task<ActionResult> MoMoCallback(MoMoRedirectResult result)
		{
			if (result.Message == "Success")
			{
				//Extract the common data
				string[] data = result.OrderInfo.Split(", ");
				string bookingId = "BK-" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7");
				string typeStr = result.OrderInfo.Split(", ")[3].Split(": ")[1];
				string paymentId = "P-" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7");
				ExtractBookingInfo(result, out float amount, out int bookingType, out string userId, out DateTime bookingDate, out int numOfMonth);
				var booking = new Booking()
				{
					BookingId = bookingId,
					Amount = amount,
					BookingType = bookingType,
					UserId = userId,
					BookingDate = bookingDate
				};
				var payment = new Payment()
				{
					PaymentId = paymentId,
					UserId = userId,
					Date = DateTime.Now,
					BookingId = bookingId,
					Method = 2,
					Amount = amount,
					TransactionId = result.TransId,
				};
				//Save to DB
				if (typeStr == FIXED || typeStr == PLAY_ONCE)
				{
					string slotId = "BS-" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7");
					string courtId = result.OrderInfo.Split(", ")[4].Split(": ")[1];
					ExtractSlotTime(result.OrderInfo, bookingType, out DateTime start, out DateTime end);
					_service.BookingService.AddBooking(booking);
					_service.PaymentService.AddPayment(payment);
					await ScheduleSlot(start, end, courtId, bookingId, typeStr, numOfMonth);
				}
				if(typeStr == FLEXIBLE)
				{
					_service.BookingService.AddBooking(booking);
					_service.PaymentService.AddPayment(payment);
				}
				if(typeStr == BUY_TIME)
				{
					payment.BookingId = null;
					_service.PaymentService.AddPayment(payment);
				}
				return Ok("Success");
			}
			return BadRequest();
		}
		[HttpGet]
		[Route("/Slot/Attempt")]
		/// <summary>
		/// Schedule slots for once or fixed booking
		/// </summary>
		/// <param name="start">Date and time the user starts playing for that block</param>
		/// <param name="end">Date and time it user stop playing for that block</param>
		/// <param name="courtId"></param>
		/// <param name="bookingId"></param>
		/// <param name="typeStr">Type of booking</param>
		/// <param name="numOfMonth"></param>
		/// <returns></returns>
		public async Task ScheduleSlot(DateTime start, DateTime end, string courtId, string bookingId, string typeStr, int numOfMonth)
		{
			SlotController controller = new(_configuration);
			bool success = false;
			if (typeStr == PLAY_ONCE)
			{
				//_service.SlotService.AddSlot(new()
				//{
				//	SlotId = "BS-" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"),
				//	BookingId = bookingId,
				//	CourtId = courtId,
				//	StartTime = start,
				//	EndTime = end,
				//});
				_service.SlotService.AttemptToAdd(new()
				{
					SlotId = "BS-" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"),
					BookingId = bookingId,
					CourtId = courtId,
					StartTime = start,
					EndTime = end,
				});
			}
			else
			{
				int count = _service.SlotService.GetAllSlots().Count + 1;
				//For every month...
				for (int i = 0; i < numOfMonth; i++)
				{
					//Schedule 4 slots by...
					for (int j = 0; j < 4; j++)
					{
						//Schedule it
						_service.SlotService.AddSlot(new()
						{
							SlotId = "BS-" + count.ToString("D7"),
							BookingId = bookingId,
							CourtId = courtId,
							StartTime = start,
							EndTime = end,
						});
						//Step by 7 days
						start = start.AddDays(7);
						end = end.AddDays(7);
						count++;
					}
				}
			}
		}

		private void ExtractSlotTime(string infoStr, int type, out DateTime start, out DateTime end)
		{
			var orderInfo = infoStr.Split(", ");
			//Day
			int dateIndex = (type == 1) ? 5 : 6;
			var dateStr = orderInfo[dateIndex].Split(": ")[1].Split("-");
			int year = int.Parse(dateStr[0]);
			int month = int.Parse(dateStr[1]);
			int day = int.Parse(dateStr[2]);
			//Start time
			int h1 = int.Parse(orderInfo[dateIndex + 1].Split(": ")[1]);
			start = new(year, month, day, h1, 0, 0);
			//End time
			int h2 = int.Parse(orderInfo[dateIndex + 2].Split(": ")[1]);
			end = new(year, month, day, h2, 0, 0);
		}

		private void ExtractBookingInfo(MoMoRedirectResult result, out float amount, out int bookingType, out string userId, out DateTime bookingDate, out int numMonth)
		{
			amount = float.Parse(result.Amount);
			var orderInfo = result.OrderInfo.Split(", ");
			switch (orderInfo[3].Split(": ")[1])
			{
				case PLAY_ONCE: bookingType = 1; break;
				case FIXED: bookingType = 2; break;
				case FLEXIBLE: bookingType = 3; break;
				default: bookingType = 0; break;
			}
			userId = orderInfo[1].Split(": ")[1];
			var dateArray = orderInfo[2].Split(": ")[1].Split("-");
			bookingDate = new DateTime(
				int.Parse(dateArray[0]),
				int.Parse(dateArray[1]),
				int.Parse(dateArray[2])
			);
			if (bookingType == 2)
			{
				//5
				string numOfMonth = orderInfo[5].Split(": ")[1];
				numMonth = int.Parse(numOfMonth);
			}
			else numMonth = 0;
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
