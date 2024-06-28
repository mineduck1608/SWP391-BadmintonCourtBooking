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
using BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.Momo;
using BadmintonCourtServices.IService;
using Azure;
using BadmintonCourtBusinessObjects.SupportEntities.Statistic;

namespace BadmintonCourtAPI.Controllers
{
	public class PaymentController : Controller
	{
		private readonly BadmintonCourtService _service = null;
		private const string resultRedirectUrl = "http:/localhost:3000/paySuccess";
		private const string flexibleBooking = "Flexible";
		private const string playonceBooking = "One-time";
		private const string fixedBooking = "Fixed";
		private const string buyTime = "Buy Time";

		public PaymentController(IConfiguration config)
		{
			if (_service == null)
				_service = new BadmintonCourtService(config);
		}

		private string GenerateVnPayUrl(TransactionDTO model)
		{
			UserDetail info = _service.UserDetailService.GetUserDetailById(model.UserId);
			VnPayRequestDTO vnPayRequestDTO = new VnPayRequestDTO();
			string content = $"User: {info.FirstName} {info.LastName} | ID: {model.UserId} | Phone: {info.Phone} | Mail: {info.Email} |";
			//---------------------------------------------------------
			if (model.Type == "flexible") // Linh hoạt
			{
				content += $" Amount: {model.Amount}";
				vnPayRequestDTO = new VnPayRequestDTO { Amount = float.Parse(model.Amount.ToString()), Content = content, Date = DateTime.Now, UserId = model.UserId };
			}
			else
			{
				if (model.Start != null && model.End != null)
				{
					BookedSlot primitive = _service.SlotService.GetSlotById("S1");
					if (model.Start < model.End && model.Start >= primitive.StartTime.Hour && model.End <= primitive.EndTime.Hour)
					{
						if (model.Date != null)
						{
							if ((new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.Start.Value, 0, 0) - DateTime.Now).TotalMinutes >= 10)
							{
								if (!model.CourtId.IsNullOrEmpty())
								{
									Court court = _service.CourtService.GetCourtByCourtId(model.CourtId);
									List<BookedSlot> tmpStorage = _service.SlotService.GetA_CourtSlotsInTimeInterval(new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.Start.Value, 0, 0), new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.End.Value, 0, 0), model.CourtId);
									//---------------------------------------------------------
									if (tmpStorage.Count > 0) // Nếu sân có người đặt trước
										return "Slot";
									//---------------------------------------------------------
									if (model.Type == "playonce") // Đặt loại 1 lần chơi
									{
										content += $" Date: {model.Date.Value.ToString("yyyy-MM-dd")} {model.Start}h - {model.End}h | Court: {model.CourtId}";
										vnPayRequestDTO = new VnPayRequestDTO { Amount = (model.End - model.Start) * court.Price, Content = content, UserId = model.UserId };

									}
									//---------------------------------------------------------
									else  // Cố định. Vd: ngày 1/1/2001 15h-17h 2 tháng sân A
									{
										content += $" Start date on schedule: {model.Date.Value.ToString("yyyy-MM-dd")} {model.Start}h - {model.End}h | Court: {model.CourtId} | Number of months: {model.NumMonth}";
										vnPayRequestDTO = new VnPayRequestDTO { Amount = (model.End - model.Start) * court.Price * model.NumMonth * 4, Content = content, UserId = model.UserId };
									}
								}
								else return "Court";
							}
							else return "Time";
						}
						else return "Time";
					}
					else return "Time";
				}
				else return "Time";
			}
			//------------------------------------------------------------------------------------
			return _service.VnPayService.CreatePaymentUrl(HttpContext, vnPayRequestDTO, null);
		}

		private string GenerateMomoUrl(TransactionDTO model)
		{
			UserDetail info = _service.UserDetailService.GetUserDetailById(model.UserId);
			string content = $"User: {info.FirstName} {info.LastName} | ID: {model.UserId} | Phone: {info.Phone} | Mail: {info.Email} |";
			string amount = "";
			//---------------------------------------------------------
			if (model.Type == "flexible")
			{
				amount = $"{model.Amount}";
				content += $" Amount: {amount}";
			}
			//---------------------------------------------------------
			else
			{
				if (model.Start != null && model.End != null)
				{
					BookedSlot primitive = _service.SlotService.GetSlotById("S1");
					if (model.Start < model.End && model.Start >= primitive.StartTime.Hour && model.End <= primitive.EndTime.Hour)
					{
						if (model.Date != null)
						{
							if ((new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.Start.Value, 0, 0) - DateTime.Now).TotalMinutes >= 10)
							{
								if (!model.CourtId.IsNullOrEmpty())
								{
									Court court = _service.CourtService.GetCourtByCourtId(model.CourtId);
									List<BookedSlot> tmpStorage = _service.SlotService.GetA_CourtSlotsInTimeInterval(new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.Start.Value, 0, 0), new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.End.Value, 0, 0), model.CourtId);
									//---------------------------------------------------------
									if (tmpStorage.Count > 0) // Nếu sân có người đặt trước
										return "Slot";
									//---------------------------------------------------------

									if (model.Type == "playonce") // Đặt loại 1 lần chơi
									{
										amount = $"{(model.End - model.Start) * court.Price}";
										content += $" Date: {model.Date.Value.ToString("yyyy-MM-dd")} {model.Start}h - {model.End}h | Court: {model.CourtId}";
									}
									//---------------------------------------------------------
									else  // Cố định. Vd: ngày 1/1/2001 15h-17h 2 tháng sân A	
									{
										amount = $"{(model.End - model.Start) * court.Price * model.NumMonth}";
										content += $" Start date on schedule: {model.Date.Value.ToString("yyyy-MM-dd")} {model.Start}h - {model.End}h | Court: {model.CourtId} | Number of months: {model.NumMonth}";
									}
								}
								else return "Court";
							}
							else return "Time";
						}
						else return "Time";
					}
					else return "Time";
				}
				else return "Time";
			}
			//---------------------------------------------------------
			var reqdata = _service.MomoService.CreateRequestData(content, $"{amount}", "");
			var response = _service.MomoService.SendMoMoRequest(reqdata);
			return response.Result.PayUrl;
		}



		[HttpGet]
		//[Authorize]
		[Route("Payment/Statistic")]
		public async Task<ActionResult<IEnumerable<DashboardResponseDTO>>> GetPaymentsForStatistic(DashboardRequestDTO model)
		{
			
			model.Year = model.Year == null ? DateTime.Now.Year : model.Year; // Năm ko nhập mặc định lấy năm hiện tại lúc gửi request
			//-----------------------------------------------------------
			model.Type = model.Type == null ? 1 : model.Type; // Type ko nhập mặc định sẽ là loại dashboard năm
			//-----------------------------------------------------------
			DateTime start = new DateTime(model.Year.Value, 12, 31, 23, 59, 59);
			DateTime end = new DateTime(model.Year.Value, 1, 1, 0, 0, 0);
			List<Payment> pList = _service.PaymentService.GetAllPayments().Where(x => x.Date >= start && x.Date <= end).ToList();
			//-----------------------------------------------------------
			if (model.Type == 1) // Dashboard năm
				return Ok(Util.GenerateYearDashboard(model.Year.Value, pList));
			//-----------------------------------------------------------
			else
			{	
				if (model.Type == 2) // Dashboard Tháng
				{
					if (model.StartMonth == null || model.MonthNum == null)
						return BadRequest(new { msg = "Invalid month" });
					//-----------------------------------------------------
					if (model.MonthNum == 11) // Tháng 11 thì chỉ đc xem nhiều nhất 11 12 hoặc chỉ 11	
						model.MonthNum = model.MonthNum == 3 ? 2 : model.MonthNum; // Nếu chọn 3 mặc định sẽ đưa về 2 
					//-----------------------------------------------------
					else if (model.MonthNum == 12)
						model.MonthNum = model.MonthNum == 3 || model.MonthNum == 2 ? 1 : model.MonthNum; // Nhập 3 hay 2 đều đưa về 1
					//-----------------------------------------------------
					return Ok(Util.GenerateMonthDashboard(model.Year.Value, model.StartMonth.Value, model.MonthNum.Value, pList));
				}
				//-----------------------------------------------------
				// Dashboard tuần
				if (model.StartMonth == null)
					return BadRequest(new { msg = "Month can't be empty" });
				model.Week = model.Week == null ? 1 : model.Week.Value; // Nếu ko nhập tuần nào thì mặc định tuần đầu
				return Ok(Util.GenerateWeekDashboard(model.Year.Value, model.StartMonth.Value, model.Week.Value, pList));
			}
		}

		[HttpGet]
		//[Authorize(Roles = "Admin,Staff")]
		[Route("Payment/GetAll")]
		public async Task<ActionResult<IEnumerable<Payment>>> GetAllPayments() => Ok(_service.PaymentService.GetAllPayments());

		[HttpGet]
		[Route("Payment/GetByOrder")]
		//[Authorize]
		public async Task<ActionResult<IEnumerable<Payment>>> GetAllPaymentsByOrder(int? order, string? rawStart, string? rawEnd, string userId)
		{

			DateTime startDate = rawStart.IsNullOrEmpty() ? new DateTime(2000, 1, 1, 0, 0, 0) : new DateTime(int.Parse(rawStart.Split('-')[0]), int.Parse(rawStart.Split('-')[1]), int.Parse(rawStart.Split('-')[2]), 0, 0, 0);
			DateTime endDate = rawEnd.IsNullOrEmpty() ? new DateTime(3000, 1, 1, 23, 59, 59) : new DateTime(int.Parse(rawEnd.Split('-')[0]), int.Parse(rawEnd.Split('-')[1]), int.Parse(rawEnd.Split('-')[2]), 23, 59, 59);
			if (endDate > startDate)
				return BadRequest(new { msg = "Invalid time interval" });

			//-----------------------------------------------------------
			if (!userId.IsNullOrEmpty())
			{
				if (order == 1) // Sort tăng dần theo ngày
					return Ok(_service.PaymentService.GetPaymentsByUserId(userId).Where(x => x.Date >=startDate && x.Date <=endDate).OrderBy(x => x.Amount).ToList());

				else if (order == 2)
					return Ok(_service.PaymentService.GetPaymentsByUserId(userId).Where(x => x.Date >= startDate && x.Date <= endDate).OrderByDescending(x => x.Amount).ToList());

				return Ok(_service.PaymentService.GetPaymentsByUserId(userId).Where(x => x.Date >= startDate && x.Date <= endDate).ToList()); // Default
			}

			if (order == 1) // Sort tăng dần theo ngày
				return Ok(_service.PaymentService.GetAllPayments().Where(x => x.Date >= startDate && x.Date <= endDate).OrderBy(x => x.Amount).ToList());

			else if (order == 2)
				return Ok(_service.PaymentService.GetAllPayments().Where(x => x.Date >= startDate && x.Date <= endDate).OrderByDescending(x => x.Amount).ToList());

			return Ok(_service.PaymentService.GetAllPayments().Where(x => x.Date <= startDate && x.Date <= endDate).OrderByDescending(x => x.Amount).ToList());
		}


		[HttpGet]
		[Route("Payment/GetByUser")]
		//[Authorize]
		public async Task<ActionResult<IEnumerable<Payment>>> GetUserPayments(string id) => Ok(_service.PaymentService.GetPaymentsByUserId(id));


		[HttpGet]
		[Route("Payment/GetBySearch")]
		//[Authorize]
		public async Task<ActionResult<IEnumerable<Payment>>> GetPaymentsBySearch(string? id, string? search) => Ok(_service.PaymentService.GetPaymentsBySearch(id, search));

		[HttpPost]
		//[Authorize]
		[Route("Booking/TransactionProcess")]
		public async Task<IActionResult> Payment(TransactionDTO model)
		{
			// Method: phương thức thanh toán: momo, vnpay, ...
			// id: user id
			// court id: mã sân
			// date: ngày bắt đầu chơi - ngày hẹn trên lịch
			// type: loại đặt: linh động, cố định, 1 lần chơi
			// start - end: khoảng thời gian chơi. VD: 3h - 5h ngày x/x/xxxx
			// numMonth: số tháng chơi (giành cho đặt cố định)
			string url = model.Method == 1 ? GenerateVnPayUrl(model) : GenerateMomoUrl(model);
			if (url == "Time")
				return BadRequest(new { msg = "Invalid time" });
			else if (url == "Court")
				return BadRequest(new { msg = "Court can't be empty" });
			else if (url == "Slot")
				return BadRequest(new { msg = $"Court {model.CourtId} on {model.Date.Value.ToString("yyyy-MM-dd")} {model.Start}h - {model.End}h has been booked" });
			return Ok(new { url = url });
		}


		//[HttpGet]
		////[Authorize]
		//[Route("Payment/VnPayResult")]
		//public async Task<ActionResult> VnPayPaymentCallBack()
		//{
		//	VnPayResponseDTO result = _service.VnPayService.PaymentExecute(Request.Query);
		//	if (result.VnPayResponseCode != "00")
		//		return BadRequest(new { msg = "Fail" });

		//	string des = result.Description;
		//	string userId = result.Description.Split('|')[1].Trim().Split(':')[1].Trim();
		//	double amount = result.Amount;
		//	User user = _service.UserService.GetUserById(userId);
		//	// Loc bang keyword: date
		//	// Loc lop t2: loc bang startdate on schdeule
		//	// lop cuoi cung con lai la flexible
		//	//-----------------------------------------------------------------------------
		//	// Tạo payment
		//	Payment payment = new Payment { PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"), Date = result.Date, Method = 1, UserId = userId, TransactionId = result.TransactionId, Amount = amount };

		//	if (result.Description.ToLower().Contains("date")) // Loc dc loai flexible
		//	{
		//		string courtId = result.Description.Split('|')[5].Trim().Split(':')[1].Trim();
		//		string bookingId = "BK" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7");
		//		payment.BookingId = bookingId; // Nếu 1 lần chơi hoặc cố định thì payment sẽ có bookingId
		//									   //---------------------------------------------------------------------
		//		string rawDate = result.Description.Split('|')[4].Trim().Split(':')[1].Trim();
		//		DateOnly date = DateOnly.Parse(rawDate.Split(' ')[0].Trim());
		//		int start = int.Parse(new string(rawDate.Split(' ')[1].Where(char.IsDigit).ToArray()));
		//		int end = int.Parse(new string(rawDate.Split(' ')[3].Where(char.IsDigit).ToArray()));

		//		if (result.Description.Contains("Start date on schedule")) // Choi thang
		//		{
		//			_service.BookingService.AddBooking(new Booking { BookingId = bookingId, BookingType = 2, Amount = amount / 1000, UserId = userId, BookingDate = DateTime.Now }); // Tạo booking
		//																																				 //-------------------------------------------------------------
		//			string[] tmpArr = result.Description.Split('|');
		//			int numMonth = int.Parse(tmpArr[tmpArr.Length - 1].Trim().Split(':')[1].Trim());
		//			List<BookedSlot> slotList = _service.SlotService.GetSlotsByFixedBooking(numMonth, new DateTime(date.Year, date.Month, date.Day, start, 0, 0), new DateTime(date.Year, date.Month, date.Day, end, 0, 0), courtId);
		//			foreach (var item in slotList)
		//			{
		//				item.BookingId = bookingId;
		//				item.SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7");
		//				_service.SlotService.AddSlot(item);
		//			}
		//		}
		//		//-------------------------------------------------------------
		//		else // Choi ngay`, choi 1 lan
		//		{
		//			_service.BookingService.AddBooking(new Booking { BookingId = bookingId, BookingType = 1, Amount = amount / 1000, UserId = userId, BookingDate = DateTime.Now });
		//			_service.SlotService.AddSlot(new BookedSlot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = bookingId, CourtId = courtId, StartTime = new DateTime(date.Year, date.Month, date.Day, start, 0, 0), EndTime = new DateTime(date.Year, date.Month, date.Day, end, 0, 0) });
		//		}
		//	}
		//	//-------------------------------------------------------------
		//	else // Mua so du
		//	{
		//		user.Balance += result.Amount / 1000;
		//		_service.UserService.UpdateUser(user, userId);
		//	}
		//	// ----------------
		//	// Tạo payment sau khi có booking
		//	_service.PaymentService.AddPayment(payment);
		//	//----------------------------------------------
		//	List<Discount> discountList = _service.DiscountService.GetAllDiscounts().OrderByDescending(x => x.Amount).ToList();
		//	foreach (Discount discount in discountList)
		//	{
		//		if (amount >= discount.Amount)
		//		{
		//			amount = amount;
		//			user.Balance += (amount * discount.Proportion) / 100;
		//			_service.UserService.UpdateUser(user, userId);
		//			break;
		//		}
		//	}
		//	return Ok(new { msg = "Success" });
		//}

		//[HttpGet]
		//[Route("Payment/MomoResult")]
		////[Authorize]
		//public async Task<ActionResult> MoMoCallback(MoMoRedirectResult result)
		//{
		//	if (result.Message == "Fail")
		//		return BadRequest(new { msg = "Transaction fail" });
		//	//-------------------------------------------------------
		//	string userId = result.OrderInfo.Split('|')[1].Trim().Split(':')[1].Trim();
		//	double amount = double.Parse(result.Amount);
		//	User user = _service.UserService.GetUserById(userId);
		//	// Loc bang keyword: date
		//	// Loc lop t2: loc bang startdate on schdeule
		//	// lop cuoi cung con lai la flexible
		//	//-----------------------------------------------------------------------------
		//	// Tạo payment
		//	Payment payment = new Payment { PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"), Date = DateTime.Parse(result.ResponseTime), Method = 1, UserId = userId, TransactionId = result.TransId, Amount = amount };
		//	if (result.OrderInfo.ToLower().Contains("date")) // Loc dc loai flexible
		//	{
		//		string courtId = result.OrderInfo.Split('|')[5].Trim().Split(':')[1].Trim();
		//		string bookingId = "BK" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7");
		//		payment.BookingId = bookingId; // Nếu 1 lần chơi hoặc cố định thì payment sẽ có bookingId
		//									   //---------------------------------------------------------------------
		//		string rawDate = result.OrderInfo.Split('|')[4].Trim().Split(':')[1].Trim();
		//		DateOnly date = DateOnly.Parse(rawDate.Split(' ')[0].Trim());
		//		int start = int.Parse(new string(rawDate.Split(' ')[1].Where(char.IsDigit).ToArray()));
		//		int end = int.Parse(new string(rawDate.Split(' ')[3].Where(char.IsDigit).ToArray()));

		//		if (result.OrderInfo.Contains("Start date on schedule")) // Choi thang
		//		{
		//			_service.BookingService.AddBooking(new Booking { BookingId = bookingId, BookingType = 2, Amount = amount / 1000, UserId = userId }); // Tạo booking
		//																																				 //-------------------------------------------------------------
		//			string[] tmpArr = result.OrderInfo.Split('|');
		//			int numMonth = int.Parse(tmpArr[tmpArr.Length - 1].Trim().Split(':')[1].Trim());
		//			List<BookedSlot> slotList = _service.SlotService.GetSlotsByFixedBooking(numMonth, new DateTime(date.Year, date.Month, date.Day, start, 0, 0), new DateTime(date.Year, date.Month, date.Day, end, 0, 0), courtId);
		//			foreach (var item in slotList)
		//			{
		//				item.BookingId = bookingId;
		//				item.SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7");
		//				_service.SlotService.AddSlot(item);
		//			}
		//		}
		//		//-------------------------------------------------------------
		//		else // Choi ngay`, choi 1 lan
		//		{
		//			_service.BookingService.AddBooking(new Booking { BookingId = bookingId, BookingType = 1, Amount = amount / 1000, UserId = userId });
		//			_service.SlotService.AddSlot(new BookedSlot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = bookingId, CourtId = courtId, StartTime = new DateTime(date.Year, date.Month, date.Day, start, 0, 0), EndTime = new DateTime(date.Year, date.Month, date.Day, end, 0, 0) });
		//		}
		//	}
		//	//-------------------------------------------------------------
		//	else // Mua so du
		//	{
		//		user.Balance += amount / 1000;
		//		_service.UserService.UpdateUser(user, userId);
		//	}
		//	// ----------------
		//	// Tạo payment sau khi có booking
		//	_service.PaymentService.AddPayment(payment);
		//	//----------------------------------------------
		//	List<Discount> discountList = _service.DiscountService.GetAllDiscounts().OrderByDescending(x => x.Amount).ToList();
		//	foreach (Discount discount in discountList)
		//	{
		//		if (amount >= discount.Amount)
		//		{
		//			amount = amount / 1000;
		//			user.Balance += (amount * discount.Proportion);
		//			_service.UserService.UpdateUser(user, userId);
		//			break;
		//		}
		//	}
		//	return Ok(new { msg = "Success" });
		//}



		[HttpGet]
		//[Authorize]
		[Route("Payment/VnPayResult")]
		public async Task<IActionResult> VnPayPaymentCallBack()
		{
			VnPayResponseDTO result = _service.VnPayService.PaymentExecute(Request.Query);
			if (result.VnPayResponseCode != "00")
				return Redirect(resultRedirectUrl + "?msg=fail");

			string des = result.Description;
			string userId = result.Description.Split('|')[1].Trim().Split(':')[1].Trim();
			double amount = result.Amount;
			User user = _service.UserService.GetUserById(userId);
			UserDetail info = _service.UserDetailService.GetUserDetailById(userId);
			string desMail = info.Email;
			string mailBody = "Thank you for your purchasement! ";
			// Loc bang keyword: date
			// Loc lop t2: loc bang startdate on schdeule
			// lop cuoi cung con lai la flexible
			//-----------------------------------------------------------------------------
			// Tạo payment
			Payment payment = new Payment { PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"), Date = result.Date, Method = 1, UserId = userId, TransactionId = result.TransactionId, Amount = amount };

			if (result.Description.ToLower().Contains("date")) // Loc dc loai flexible
			{
				string courtId = result.Description.Split('|')[5].Trim().Split(':')[1].Trim();
				string bookingId = "BK" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7");
				payment.BookingId = bookingId; // Nếu 1 lần chơi hoặc cố định thì payment sẽ có bookingId
											   //---------------------------------------------------------------------
				string rawDate = result.Description.Split('|')[4].Trim().Split(':')[1].Trim();
				DateOnly date = DateOnly.Parse(rawDate.Split(' ')[0].Trim());
				int start = int.Parse(new string(rawDate.Split(' ')[1].Where(char.IsDigit).ToArray()));
				int end = int.Parse(new string(rawDate.Split(' ')[3].Where(char.IsDigit).ToArray()));

				Booking booking = new Booking()
				{
					BookingId = bookingId,
					Amount = amount,
					UserId = userId,
					BookingDate = DateTime.Now,
				};

				if (result.Description.Contains("Start date on schedule")) // Choi thang
				{

					booking.BookingType = 2;	
					_service.BookingService.AddBooking(booking);
					//-------------------------------------------------------------
					string[] tmpArr = result.Description.Split('|');
					int numMonth = int.Parse(tmpArr[tmpArr.Length - 1].Trim().Split(':')[1].Trim());
					List<BookedSlot> slotList = _service.SlotService.GetSlotsByFixedBooking(numMonth, new DateTime(date.Year, date.Month, date.Day, start, 0, 0), new DateTime(date.Year, date.Month, date.Day, end, 0, 0), courtId);
					foreach (var item in slotList)
					{
						item.BookingId = bookingId;
						item.SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7");
						_service.SlotService.AddSlot(item);
					}
					mailBody += $"You have successfully finished the payment {payment.PaymentId} as {amount} with the booking {bookingId}. Now you can check your booking for upcoming played slot.";
					
				}
				//-------------------------------------------------------------
				else // Choi ngay`, choi 1 lan
				{
					booking.BookingType = 1;
					_service.BookingService.AddBooking(booking);
					_service.SlotService.AddSlot(new BookedSlot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = bookingId, CourtId = courtId, StartTime = new DateTime(date.Year, date.Month, date.Day, start, 0, 0), EndTime = new DateTime(date.Year, date.Month, date.Day, end, 0, 0) });
				}
			}
			//-------------------------------------------------------------
			else // Mua so du
			{
				user.Balance += result.Amount;
				_service.UserService.UpdateUser(user, userId);
				mailBody += $"Your balance now is {user.Balance} after the transaction of {amount}. You now can double check it {DateTime.Now}";
			}
			// ----------------
			// Tạo payment sau khi có booking
			_service.PaymentService.AddPayment(payment);
			_service.MailService.SendMail("taikhoanhoconl123@gmail.com", mailBody, "Payment");
			//----------------------------------------------
			List<Discount> discountList = _service.DiscountService.GetAllDiscounts().OrderByDescending(x => x.Amount).ToList();
			foreach (Discount discount in discountList)
			{
				if (amount >= discount.Amount)
				{
					user.Balance += (amount * discount.Proportion) / 100;
					_service.UserService.UpdateUser(user, userId);
					break;
				}
			}
			return Redirect(resultRedirectUrl + "?msg=success");
		}

		[HttpGet]
		[Route("Payment/MomoResult")]
		//[Authorize]
		public async Task<ActionResult> MoMoCallback(MoMoRedirectResult result)
		{
			if (result.Message == "Fail")
				return Redirect(resultRedirectUrl + "?msg=fail");
			//-------------------------------------------------------
			string userId = result.OrderInfo.Split('|')[1].Trim().Split(':')[1].Trim();
			double amount = double.Parse(result.Amount);
			User user = _service.UserService.GetUserById(userId);
			// Loc bang keyword: date
			// Loc lop t2: loc bang startdate on schdeule
			// lop cuoi cung con lai la flexible
			//-----------------------------------------------------------------------------
			// Tạo payment
			Payment payment = new Payment { PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"), Date = DateTime.Parse(result.ResponseTime), Method = 2, UserId = userId, TransactionId = result.TransId, Amount = amount };
			if (result.OrderInfo.ToLower().Contains("date")) // Loc dc loai flexible
			{
				string courtId = result.OrderInfo.Split('|')[5].Trim().Split(':')[1].Trim();
				string bookingId = "BK" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7");
				payment.BookingId = bookingId; // Nếu 1 lần chơi hoặc cố định thì payment sẽ có bookingId
											   //---------------------------------------------------------------------
				string rawDate = result.OrderInfo.Split('|')[4].Trim().Split(':')[1].Trim();
				DateOnly date = DateOnly.Parse(rawDate.Split(' ')[0].Trim());
				int start = int.Parse(new string(rawDate.Split(' ')[1].Where(char.IsDigit).ToArray()));
				int end = int.Parse(new string(rawDate.Split(' ')[3].Where(char.IsDigit).ToArray()));

				if (result.OrderInfo.Contains("Start date on schedule")) // Choi thang
				{
					_service.BookingService.AddBooking(new Booking { BookingId = bookingId, BookingType = 2, Amount = amount, UserId = userId }); // Tạo booking
																																						 //-------------------------------------------------------------
					string[] tmpArr = result.OrderInfo.Split('|');
					int numMonth = int.Parse(tmpArr[tmpArr.Length - 1].Trim().Split(':')[1].Trim());
					List<BookedSlot> slotList = _service.SlotService.GetSlotsByFixedBooking(numMonth, new DateTime(date.Year, date.Month, date.Day, start, 0, 0), new DateTime(date.Year, date.Month, date.Day, end, 0, 0), courtId);
					foreach (var item in slotList)
					{
						item.BookingId = bookingId;
						item.SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7");
						_service.SlotService.AddSlot(item);
					}
				}
				//-------------------------------------------------------------
				else // Choi ngay`, choi 1 lan
				{
					_service.BookingService.AddBooking(new Booking { BookingId = bookingId, BookingType = 1, Amount = amount, UserId = userId });
					_service.SlotService.AddSlot(new BookedSlot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = bookingId, CourtId = courtId, StartTime = new DateTime(date.Year, date.Month, date.Day, start, 0, 0), EndTime = new DateTime(date.Year, date.Month, date.Day, end, 0, 0) });
				}
			}
			//-------------------------------------------------------------
			else // Mua so du
			{
				user.Balance += amount;
				_service.UserService.UpdateUser(user, userId);
			}
			// ----------------
			// Tạo payment sau khi có booking
			_service.PaymentService.AddPayment(payment);
			//----------------------------------------------
			List<Discount> discountList = _service.DiscountService.GetAllDiscounts().OrderByDescending(x => x.Amount).ToList();
			foreach (Discount discount in discountList)
			{
				if (amount >= discount.Amount)
				{
					user.Balance += (amount * discount.Proportion) / 100;
					_service.UserService.UpdateUser(user, userId);
					break;
				}
			}
			return Redirect(resultRedirectUrl + "?msg=success");
		}

	}
}
