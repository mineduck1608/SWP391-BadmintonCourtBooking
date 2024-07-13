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
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BadmintonCourtBusinessObjects.SupportEntities.Statistic;

namespace BadmintonCourtAPI.Controllers
{
	public class PaymentController : Controller
	{
		private readonly IPaymentService _service = null;
		private readonly IBookingService _bookingService = new BookingService();
		private readonly IDiscountService _discountService = new DiscountService();
		private readonly IUserDetailService _userDetailService = new UserDetailService();
		private readonly IUserService _userService = new UserService();
		private readonly ISlotService _slotService = new SlotService();
		private readonly ICourtService _courtService = new CourtService();
		private readonly MoMoService _moMoService = new MoMoService();
		private BadmintonCourtService bmtnService = null;
		private readonly IVnPayService _vnPayService = new VnPayService();
		private readonly IMailService _mailService = new MailService();

		public PaymentController(IPaymentService service)
		{
			_service = service;
			bmtnService = new BadmintonCourtService();
		}

		private const string resultRedirectUrl = "http://localhost:3000/paySuccess";
		private const string flexibleBooking = "flexible";
		private const string playonceBooking = "playOnce";
		private const string fixedBooking = "fixed";
		private const string buyTime = "buyTime";
		private const string style = "style='padding: 8px; border: 1px solid #ddd;'";



		private string GenerateSlotTable(string orderInfo)
		{
			int index = 3;
			int tail = orderInfo.Split(',').Length;
			List<string> detail = new List<string>();
			for (int i = index; i < tail; i++)
				detail.Add(orderInfo.Split(',')[i].Trim().Split(':')[1].Trim());

            string table = $@"<table {style}>
    <tr>
        <th {style}>Court</th>
        <th {style}>Date</th>
        <th {style}>Start period</th>
        <th {style}>End period</th>
";

			if (orderInfo.Contains("Number of months:"))
				table += $@"<th {style}>Number of month(s)</th>";
			table += @"</tr><tr>";
			foreach (var item in detail)
				table += $@"<td {style}>{item}</td>";
			   
            return table + " </tr></table>";
		}

		private void ExecuteSendMail (string orderInfo, string amount)
		{
			string userId = orderInfo.Split(',')[1].Trim().Split(':')[1].Trim();
			UserDetail info = _userDetailService.GetUserDetailById(userId);
			string type = orderInfo.Split(',')[0].Trim().Split(':')[1].Trim();
			//------------------------------------------------------------------
			string content = "";
			if (type == fixedBooking || type == playonceBooking)
				content = GenerateSlotTable(orderInfo);

			content += "<p>Dear " + ((info.FirstName.IsNullOrEmpty() && info.LastName.IsNullOrEmpty()) ? $"{info.Email}" : $"{info.FirstName} {info.LastName}") + ",</p>\r\n<p>Thank you for your purchase</p><p>";

			if (type == buyTime || type == flexibleBooking)
				content += $"We have added to your balance {amount}";
			else
				content += "We have added new booked slot to your schedule";
			//------------------------------------------------------------------
			content += "</p><p> You can now check it on your account.</p>\r\n<p>If you have any questions or need further assistance, please contact us.</p>\r\n<p>Best regards,<br>\r\nBadmintonCourtBooking BMTC</p>\r\n<p>Contact Information:<br>\r\nPhone: 0977300916<br>\r\nAddress: 123 Badminton St, Hanoi, Vietnam<br>\r\nEmail: externalauthdemo1234@gmail.com</p>";

			_mailService.SendMail(info.Email, content, "BMTC - Booking Notification");
		}
		// <p>You can now check your transaction information in the payment history section of your account.</p>\r\n<p>If you have any questions or need further assistance, please contact us.</p>\r\n<p>Best regards,<br>\r\nBadmintonCourtBooking BMTC</p>\r\n<p>Contact Information:<br>\r\nPhone: 0977300916<br>\r\nAddress: 123 Badminton St, Hanoi, Vietnam<br>\r\nEmail: externalauthdemo1234@gmail.com</p>

		/// <summary>
		/// Create info for both methods. It will have the following format:
		/// Type: (type), User id: (userId), Email: (email), Court id: (courtId), Date: (bookingDate as yyyy-MM-dd), Start hour: (start), End hour: (end), For: (numMonth) month(s) (fixed only)
		/// As for buying time:
		/// Type: Buy Time, User id: (userId)
		/// </summary>
		/// <param name="transactionDTO"></param>
		/// <returns></returns>
		private string GenerateOrderInfo(TransactionDTO transactionDTO)
		{
			StringBuilder sb = new StringBuilder();			
			string type = transactionDTO.Type;
			string userId = transactionDTO.UserId;
			string courtId = transactionDTO.CourtId;
		
			int? start = transactionDTO.Start;
			int? end = transactionDTO.End;
			int? numMonth = transactionDTO.NumMonth;
			float? amount = transactionDTO.Amount;

			sb.Append($"Type: {type} , ");
			sb.Append($"User: {userId} , ");
			sb.Append($"Email: {_userDetailService.GetUserDetailById(userId).Email}");
			if (type != buyTime && type != flexibleBooking)
			{
				DateTime? date = transactionDTO.Date;
				int year = date.Value.Year; 
				int month = date.Value.Month; 
				int day = date.Value.Day;	
				string dateStr = $"{year}-{month}-{day}";
				sb.Append($" , Court: {courtId}");
				sb.Append($" , Date: {dateStr}");
				sb.Append($" , Start hour: {start}");
				sb.Append($" , End hour: {end}");
				if (type == fixedBooking)
					sb.Append($" , Number of months: {numMonth}");
			}
			return sb.ToString();
		}

		private string ValidateOrder(TransactionDTO dto)
		{

			if (dto.UserId.IsNullOrEmpty() || _userService.GetUserById(dto.UserId) == null)
				return "User not exist";

			if (dto.Type.IsNullOrEmpty())
				return "Type can't be empty";

			if (dto.Type == fixedBooking || dto.Type == playonceBooking)
			{
				BookedSlot primitive = _slotService.GetSlotById("S1");
				//----------------------------------------------------------------
				if (dto.CourtId.IsNullOrEmpty())
					return "Court can't be empty";

				if (_courtService.GetCourtByCourtId(dto.CourtId).CourtStatus == false)
					return "Court inactive";

				if (dto.Start == null || dto.End == null || dto.Start > dto.End || dto.Date == null || dto.Start < primitive.StartTime.Hour || dto.End > primitive.EndTime.Hour)
					return "Invalid time interval";
				
				DateTime startDate = new DateTime(dto.Date.Value.Year, dto.Date.Value.Month, dto.Date.Value.Day, dto.Start.Value, 0, 0);
				DateTime endDate = new DateTime(dto.Date.Value.Year, dto.Date.Value.Month, dto.Date.Value.Day, dto.End.Value, 0, 0);
				
				if ((startDate - DateTime.Now).TotalMinutes <= 10)
					return "Booking too close slot time";

				List<BookedSlot> tmpStorage = _slotService.GetA_CourtSlotsInTimeInterval(startDate, endDate, dto.CourtId);
				if (tmpStorage.Count > 0)
					return $"Court {dto.CourtId} on {dto.Date} {dto.Start}h - {dto.End}h has been booked";

				if (dto.Type == fixedBooking)
					if (dto.NumMonth == null || dto.NumMonth > 3)
						return "Month's number can't be empty";
			}
			else
			{
				if (dto.Amount == null || dto.Amount < 10000)
					return "Invalid balance";
			}
			return "Valid";
		}

		private string GeneratePayUrl(TransactionDTO model)
		{
			string content = GenerateOrderInfo(model);
			float amount = CalculateAmount(model);
			return model.Method == 1 ? GenerateVnPayUrl(content, amount) : GenerateMomoUrl(content, amount);
		}
		private float CalculateAmount(TransactionDTO model)
		{
			if (model.Type == buyTime || model.Type == flexibleBooking) 
				return (float)model.Amount; //Already caught if null

			float courtPrice = _courtService.GetCourtByCourtId(model.CourtId).Price;
			var result = courtPrice * (model.End - model.Start) * (model.NumMonth == null ? 1 : (model.NumMonth * 4));
			return (float)result;
		}
		private string GenerateMomoUrl(string orderInfo, float amount)
		{
			MoMoRequestData reqData = _moMoService.CreateRequestData(orderInfo, amount.ToString(), null);
			var res = _moMoService.SendMoMoRequest(reqData);
			res.Wait();
			return res.Result.PayUrl;
		}

		private string GenerateVnPayUrl(string orderInfo, float amount)
		{
			VnPayRequestDTO requestDTO = new()
			{
				Amount = amount,
				Content = orderInfo
			};
			return _vnPayService.CreatePaymentUrl(HttpContext, requestDTO, null);
		}

		[HttpGet]
		[Route("Payment/Statistic")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<IEnumerable<DashboardResponseDTO>>> GetPaymentsForStatistic(DashboardRequestDTO model)
		{

			model.Year = model.Year == null ? DateTime.Now.Year : model.Year; // Năm ko nhập mặc định lấy năm hiện tại lúc gửi request
																			  //-----------------------------------------------------------
			model.Type = model.Type == null ? 1 : model.Type; // Type ko nhập mặc định sẽ là loại dashboard năm
															  //-----------------------------------------------------------
			DateTime end = new DateTime(model.Year.Value, 12, 31, 23, 59, 59);
			DateTime start = new DateTime(model.Year.Value, 1, 1, 0, 0, 0);
			List<Payment> pList = _service.GetAllPayments().Where(x => x.Date >= start && x.Date <= end).ToList();
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
					if (model.StartMonth == 11) // Tháng 11 thì chỉ đc xem nhiều nhất 11 12 hoặc chỉ 1
						model.MonthNum = model.MonthNum == 3 ? 2 : model.MonthNum; // Nếu chọn 3 mặc định sẽ đưa về 2 
																				   //-----------------------------------------------------
					else if (model.StartMonth == 12)
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
		[Authorize(Roles = "Admin,Staff")]
		[Route("Payment/GetAll")]
		public async Task<ActionResult<IEnumerable<Payment>>> GetAllPayments() => Ok(_service.GetAllPayments());

		[HttpGet]
		[Route("Payment/GetByOrder")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<Payment>>> GetAllPaymentsByOrder(int? order, DateTime? start, DateTime? end, string userId)
		{
			start = start == null ? new DateTime(2000, 1, 1, 0, 0, 0) : start;
			end = end == null ? new DateTime(3000, 1, 1, 23, 59, 59) : end;
			if (start > end)
				return BadRequest(new { msg = "Invalid time interval" });

			//-----------------------------------------------------------
			if (!userId.IsNullOrEmpty())
			{
				if (order == 1) // Sort tăng dần theo ngày
					return Ok(_service.GetPaymentsByUserId(userId).Where(x => x.Date >= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).OrderBy(x => x.Amount).ToList());

				else if (order == 2)
					return Ok(_service.GetPaymentsByUserId(userId).Where(x => x.Date >= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).OrderByDescending(x => x.Amount).ToList());

				return Ok(_service.GetPaymentsByUserId(userId).Where(x => x.Date >= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).ToList()); // Default
			}

			if (order == 1) // Sort tăng dần theo ngày
				return Ok(_service.GetAllPayments().Where(x => x.Date <= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).OrderBy(x => x.Amount).ToList());

			else if (order == 2)
				return Ok(_service.GetAllPayments().Where(x => x.Date >= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).OrderByDescending(x => x.Amount).ToList());

			return Ok(_service.GetAllPayments().Where(x => x.Date <= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).OrderByDescending(x => x.Amount).ToList());
		}


		[HttpGet]
		[Route("Payment/GetByUser")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<Payment>>> GetUserPayments(string id) => Ok(_service.GetPaymentsByUserId(id));


		[HttpGet]
		[Route("Payment/GetBySearch")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<Payment>>> GetPaymentsBySearch(string? id, string? search) => Ok(_service.GetPaymentsBySearch(id, search));

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
			//string url = model.Method == 1 ? GenerateVnPayUrl(model) : GenerateMomoUrl(model);
			model.Method = model.Method == null ? 1 : model.Method;
			string msg = ValidateOrder(model);
			if (msg != "Valid")
				return BadRequest(new { msg = msg });
			return Ok(new { url = GeneratePayUrl(model)});
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
		//			_service.SlotService.AddSlot(new BookedSlot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = bookingId, CourtId = courtId, StartHour = new DateTime(date.Year, date.Month, date.Day, start, 0, 0), EndHour = new DateTime(date.Year, date.Month, date.Day, end, 0, 0) });
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
		//			_service.SlotService.AddSlot(new BookedSlot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = bookingId, CourtId = courtId, StartHour = new DateTime(date.Year, date.Month, date.Day, start, 0, 0), EndHour = new DateTime(date.Year, date.Month, date.Day, end, 0, 0) });
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



		//[HttpGet]
		////[Authorize]
		//[Route("Payment/VnPayResult")]
		//public async Task<IActionResult> VnPayPaymentCallBack()
		//{
		//	VnPayResponseDTO result = _service.VnPayService.PaymentExecute(Request.Query);
		//	if (result.VnPayResponseCode != "00")
		//		return Redirect(resultRedirectUrl + "?msg=fail");

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
		//			_service.BookingService.AddBooking(new Booking { BookingId = bookingId, BookingType = 2, Amount = amount, UserId = userId, BookingDate = DateTime.Now }); // Tạo booking
		//																																									  //-------------------------------------------------------------
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
		//			_service.BookingService.AddBooking(new Booking { BookingId = bookingId, BookingType = 1, Amount = amount, UserId = userId, BookingDate = DateTime.Now });
		//			_service.SlotService.AddSlot(new BookedSlot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = bookingId, CourtId = courtId, StartTime = new DateTime(date.Year, date.Month, date.Day, start, 0, 0), EndTime = new DateTime(date.Year, date.Month, date.Day, end, 0, 0) });
		//		}
		//	}
		//	//-------------------------------------------------------------
		//	else // Mua so du
		//	{
		//		user.Balance += result.Amount;
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
		//	return Redirect(resultRedirectUrl + "?msg=success");
		//}

		[HttpGet]
		[Route("Payment/VnPayResult")]
		public async Task<ActionResult> VnPayPaymentCallBack()
		{
			VnPayResponseDTO result = _vnPayService.PaymentExecute(Request.Query);
			string content = result.Description;
			DateTime date = result.Date;
			string transId = result.TransactionId;
			string amount = result.Amount.ToString();
			string expected = "00";
			string actual = result.VnPayResponseCode;
			bool success = SaveToDB(content, date, transId, amount, expected, actual, 1);
			if (success)
				ExecuteSendMail(content, amount);
            return Redirect(resultRedirectUrl + "?msg=" + (success ? "Success" : "Fail"));
		}

		[HttpGet]
		[Route("Payment/MomoResult")]
		public async Task<ActionResult> MomoCallback(MoMoRedirectResult result)
		{
			var timeStr = result.ResponseTime;
			var rawdate = timeStr.Split(' ')[0].Split('-');
			DateTime date = new(int.Parse(rawdate[0]), int.Parse(rawdate[1]), int.Parse(rawdate[2]));

			bool success = SaveToDB(result.OrderInfo, date, result.TransId, result.Amount, result.Message, "Success", 2);
			if (success)
				ExecuteSendMail(result.OrderInfo, result.Amount);
            return Redirect(resultRedirectUrl + "?msg=" + (success ? "Success" : "Fail"));
		}

		// Create info for both methods.It will have the following format:
		// Type: (type), User id: (userId), Email: (email), Court id: (courtId), Date: (bookingDate), Start hour: (start), End hour: (end), Number of month: (numMonth) (fixed only)
		// As for buying time:
		// Type: Buy Time, User id: (userId), Email: (email)

		public bool SaveToDB(string info, DateTime responseDate, string transId, string amountStr, string expected, string actual, int method)
		{
			if (expected != actual) 
				return false;

			var map = TransformToDictionary(info, ',', ':');
			string userId = map["User"];
			User user = _userService.GetUserById(userId);
			double amount = double.Parse(amountStr);
			Payment payment = new()
			{
				PaymentId = "P" + (_service.GetAllPayments().Count + 1).ToString("D7"),
				Date = responseDate,
				UserId = userId,
				TransactionId = transId,
				Amount = amount,
				Method = method
				//BookingId missing
			};
			if (map["Type"] != buyTime && map["Type"] != flexibleBooking) //Capture buyTime
			{
				string courtId = map["Court"];
				string bookingId = "BK" + (_bookingService.GetAllBookings().Count + 1).ToString("D7");
				string rawDate = map["Date"];
				DateTime date = Convert.ToDateTime(rawDate);
				int start = int.Parse(map["Start hour"]);
				int end = int.Parse(map["End hour"]);
				payment.BookingId = bookingId;

				Booking booking = new()
				{
					BookingId = bookingId,
					BookingDate = DateTime.Now,
					UserId = userId,
					Amount = amount,
					BookingType = map["Type"] == fixedBooking ? 2 : 1,
					ChangeLog = 0
				};

				_bookingService.AddBooking(booking);
				if (map["Type"] == fixedBooking)
				{
					int numMonth = int.Parse(map["Number of months"]);
					List<BookedSlot> slotList = _slotService.GetSlotsByFixedBooking(
						numMonth,
						new DateTime(date.Year, date.Month, date.Day, start, 0, 0),
						new DateTime(date.Year, date.Month, date.Day, end, 0, 0),
						courtId
						);
					foreach (var item in slotList)
					{
						item.BookingId = bookingId;
						item.SlotId = "S" + (_slotService.GetAllSlots().Count + 1).ToString("D7");
						_slotService.AddSlot(item);
					}
				}
				else if (map["Type"] == playonceBooking)
				{
					_slotService.AddSlot(new BookedSlot
					{
						SlotId = "S" + (_slotService.GetAllSlots().Count + 1).ToString("D7"),
						BookingId = bookingId,
						CourtId = courtId,
						StartTime = new DateTime(date.Year, date.Month, date.Day, start, 0, 0),
						EndTime = new DateTime(date.Year, date.Month, date.Day, end, 0, 0)
					});
				}
			}
			else
			{
				user.Balance += amount;
			}
			_service.AddPayment(payment);

			List<Discount> discountList = _discountService.GetAllDiscounts().Where(x => x.IsDelete == null).OrderByDescending(x => x.Amount).ToList();
			if (discountList.Count == 0)
			{
				_userService.UpdateUser(user, userId);
				return true;
			}
			foreach (Discount discount in discountList)
			{
				if (amount >= discount.Amount)
				{
					user.Balance += (amount * discount.Proportion) / 100;
					_userService.UpdateUser(user, userId);
					break;
				}
			}
			return true;
		}

		private Dictionary<string, string> TransformToDictionary(string src, char paramSeparator, char kvSeparator)
		{
			string[] strings = src.Split(paramSeparator, kvSeparator);
			Dictionary<string, string> map = new();
			for (int i = 0; i < strings.Length; i += 2)
			{
				map.Add(strings[i].Trim(), strings[i + 1].Trim());
			}
			return map;
		}
	}
}