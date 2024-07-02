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

		//private string GenerateVnPayUrl(TransactionDTO model)
		//{
		//	UserDetail info = _service.UserDetailService.GetUserDetailById(model.UserId);
		//	VnPayRequestDTO vnPayRequestDTO = new VnPayRequestDTO();
		//	string content = $"User: {info.FirstName} {info.LastName} | ID: {model.UserId} | Phone: {info.Phone} | Mail: {info.Email} |";
		//	//---------------------------------------------------------
		//	if (model.Type == "flexible") // Linh hoạt
		//	{
		//		content += $" Amount: {model.Amount}";
		//		vnPayRequestDTO = new VnPayRequestDTO { Amount = float.Parse(model.Amount.ToString()), Content = content, Date = DateTime.Now, UserId = model.UserId };
		//	}
		//	else
		//	{
		//		if (model.Start != null && model.End != null)
		//		{
		//			if (model.Start < model.End && model.Start >= StartHour && model.End <= EndHour)
		//			{
		//				if (model.Date != null)
		//				{
		//					if ((new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.Start.Value, 0, 0) - DateTime.Now).TotalMinutes >= 10)
		//					{
		//						if (!model.CourtId.IsNullOrEmpty())
		//						{
		//							Court court = _service.CourtService.GetCourtByCourtId(model.CourtId);
		//							List<BookedSlot> tmpStorage = _service.SlotService.GetA_CourtSlotsInTimeInterval(new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.Start.Value, 0, 0), new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.End.Value, 0, 0), model.CourtId);
		//							//---------------------------------------------------------
		//							if (tmpStorage.Count > 0) // Nếu sân có người đặt trước
		//								return "Slot";
		//							//---------------------------------------------------------
		//							if (model.Type == "playonce") // Đặt loại 1 lần chơi
		//							{
		//								content += $" Date: {model.Date.Value.ToString("yyyy-MM-dd")} {model.Start}h - {model.End}h | Court: {model.CourtId}";
		//								vnPayRequestDTO = new VnPayRequestDTO { Amount = (model.End - model.Start) * court.Price, Content = content, UserId = model.UserId };

		//							}
		//							//---------------------------------------------------------
		//							else  // Cố định. Vd: ngày 1/1/2001 15h-17h 2 tháng sân A
		//							{
		//								content += $" Start date on schedule: {model.Date.Value.ToString("yyyy-MM-dd")} {model.Start}h - {model.End}h | Court: {model.CourtId} | Number of months: {model.NumMonth}";
		//								vnPayRequestDTO = new VnPayRequestDTO { Amount = (model.End - model.Start) * court.Price * model.NumMonth * 4, Content = content, UserId = model.UserId };
		//							}
		//						}
		//						else return "Court";
		//					}
		//					else return "Time";
		//				}
		//				else return "Time";
		//			}
		//			else return "Time";
		//		}
		//		else return "Time";
		//	}
		//	//------------------------------------------------------------------------------------
		//	return _service.VnPayService.CreatePaymentUrl(HttpContext, vnPayRequestDTO, null);
		//}

		//private string GenerateMomoUrl(TransactionDTO model)
		//{
		//    UserDetail info = _service.UserDetailService.GetUserDetailById(model.UserId);
		//    string content = $"User: {info.FirstName} {info.LastName} | ID: {model.UserId} | Phone: {info.Phone} | Mail: {info.Email} |";
		//    string amount = "";
		//    //---------------------------------------------------------
		//    if (model.Type == "flexible")
		//    {
		//        amount = $"{model.Amount}";
		//        content += $" Amount: {amount}";
		//    }
		//    //---------------------------------------------------------
		//    else
		//    {
		//        if (model.Start != null && model.End != null)
		//        {
		//            if (model.Start < model.End && model.Start >= StartHour && model.End <= EndHour)
		//            {
		//                if (model.Date != null)
		//                {
		//                    if ((new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.Start.Value, 0, 0) - DateTime.Now).TotalMinutes >= 10)
		//                    {
		//                        if (!model.CourtId.IsNullOrEmpty())
		//                        {
		//                            Court court = _service.CourtService.GetCourtByCourtId(model.CourtId);
		//                            List<BookedSlot> tmpStorage = _service.SlotService.GetA_CourtSlotsInTimeInterval(new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.Start.Value, 0, 0), new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.End.Value, 0, 0), model.CourtId);
		//                            //---------------------------------------------------------
		//                            if (tmpStorage.Count > 0) // Nếu sân có người đặt trước
		//                                return "Slot";
		//                            //---------------------------------------------------------

		//                            if (model.Type == "playonce") // Đặt loại 1 lần chơi
		//                            {
		//                                amount = $"{(model.End - model.Start) * court.Price}";
		//                                content += $" Date: {model.Date.Value.ToString("yyyy-MM-dd")} {model.Start}h - {model.End}h | Court: {model.CourtId}";
		//                            }
		//                            //---------------------------------------------------------
		//                            else  // Cố định. Vd: ngày 1/1/2001 15h-17h 2 tháng sân A	
		//                            {
		//                                amount = $"{(model.End - model.Start) * court.Price * model.NumMonth}";
		//                                content += $" Start date on schedule: {model.Date.Value.ToString("yyyy-MM-dd")} {model.Start}h - {model.End}h | Court: {model.CourtId} | Number of months: {model.NumMonth}";
		//                            }
		//                        }
		//                        else return "Court";
		//                    }
		//                    else return "Time";
		//                }
		//                else return "Time";
		//            }
		//            else return "Time";
		//        }
		//        else return "Time";
		//    }
		//    //---------------------------------------------------------
		//    var reqdata = _service.MomoService.CreateRequestData(content, $"{amount}", "");
		//    var response = _service.MomoService.SendMoMoRequest(reqdata);
		//    return response.Result.PayUrl;
		//}

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

			sb.Append($"Type: {type} | ");
			sb.Append($"User: {userId} | ");
			sb.Append($"Email: {_service.UserDetailService.GetUserDetailById(userId).Email}");
			if (type != buyTime && type != flexibleBooking)
			{
				DateTime? date = transactionDTO.Date;
				int year = date.Value.Year; 
				int month = date.Value.Month; 
				int day = date.Value.Day;
				string dateStr = $"{year}-{month}-{day}";
				sb.Append($" | Court: {courtId}");
				sb.Append($" | Date: {dateStr}");
				sb.Append($" | Start hour: {start}");
				sb.Append($" | End hour: {end}");
				if (type == fixedBooking)
					sb.Append($" | Number of months: {numMonth}");
			}
			return sb.ToString();
		}

		private string ValidateOrder(TransactionDTO dto)
		{

			if (dto.UserId.IsNullOrEmpty() || _service.UserService.GetUserById(dto.UserId) == null)
				return "User not exist";

			if (dto.Type.IsNullOrEmpty())
				return "Type can't be empty";

			if (dto.Type == fixedBooking || dto.Type == playonceBooking)
			{
				BookedSlot primitive = _service.SlotService.GetSlotById("S1");
				//----------------------------------------------------------------
				if (dto.CourtId.IsNullOrEmpty())
					return "Court can't be empty";

				if (_service.CourtService.GetCourtByCourtId(dto.CourtId).CourtStatus == false)
					return "Court inactive";

				if (dto.Start == null || dto.End == null || dto.Start > dto.End || dto.Date == null || dto.Start < primitive.StartTime.Hour || dto.End > primitive.EndTime.Hour)
					return "Invalid time interval";
				
				DateTime startDate = new DateTime(dto.Date.Value.Year, dto.Date.Value.Month, dto.Date.Value.Day, dto.Start.Value, 0, 0);
				DateTime endDate = new DateTime(dto.Date.Value.Year, dto.Date.Value.Month, dto.Date.Value.Day, dto.End.Value, 0, 0);
				
				if ((startDate - DateTime.Now).TotalMinutes <= 10)
					return "Booking too close slot time";

				List<BookedSlot> tmpStorage = _service.SlotService.GetA_CourtSlotsInTimeInterval(startDate, endDate, dto.CourtId);
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

			float courtPrice = _service.CourtService.GetCourtByCourtId(model.CourtId).Price;
			var result = courtPrice * (model.End - model.Start) * (model.NumMonth == null ? 1 : (model.NumMonth * 4));
			return (float)result;
		}
		private string GenerateMomoUrl(string orderInfo, float amount)
		{
			MoMoRequestData reqData = _service.MomoService.CreateRequestData(orderInfo, amount.ToString(), null);
			var res = _service.MomoService.SendMoMoRequest(reqData);
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
			return _service.VnPayService.CreatePaymentUrl(HttpContext, requestDTO, null);
		}

		[HttpGet]
		[Route("Payment/Statistic")]
		[Authorize]
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
		[Authorize(Roles = "Admin,Staff")]
		[Route("Payment/GetAll")]
		public async Task<ActionResult<IEnumerable<Payment>>> GetAllPayments() => Ok(_service.PaymentService.GetAllPayments());

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
					return Ok(_service.PaymentService.GetPaymentsByUserId(userId).Where(x => x.Date >= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).OrderBy(x => x.Amount).ToList());

				else if (order == 2)
					return Ok(_service.PaymentService.GetPaymentsByUserId(userId).Where(x => x.Date >= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).OrderByDescending(x => x.Amount).ToList());

				return Ok(_service.PaymentService.GetPaymentsByUserId(userId).Where(x => x.Date >= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).ToList()); // Default
			}

			if (order == 1) // Sort tăng dần theo ngày
				return Ok(_service.PaymentService.GetAllPayments().Where(x => x.Date <= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).OrderBy(x => x.Amount).ToList());

			else if (order == 2)
				return Ok(_service.PaymentService.GetAllPayments().Where(x => x.Date >= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).OrderByDescending(x => x.Amount).ToList());

			return Ok(_service.PaymentService.GetAllPayments().Where(x => x.Date <= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).OrderByDescending(x => x.Amount).ToList());
		}


		[HttpGet]
		[Route("Payment/GetByUser")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<Payment>>> GetUserPayments(string id) => Ok(_service.PaymentService.GetPaymentsByUserId(id));


		[HttpGet]
		[Route("Payment/GetBySearch")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<Payment>>> GetPaymentsBySearch(string? id, string? search) => Ok(_service.PaymentService.GetPaymentsBySearch(id, search));

		[HttpPost]
		[Authorize]
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
			VnPayResponseDTO result = _service.VnPayService.PaymentExecute(Request.Query);
			string info = result.Description;
			DateTime date = result.Date;
			string transId = result.TransactionId;
			string amount = result.Amount.ToString();
			string expected = "00";
			string actual = result.VnPayResponseCode;
			bool success = SaveToDB(info, date, transId, amount, expected, actual);
			return Redirect(resultRedirectUrl + "?msg=" + (success ? "Success" : "Fail"));
		}

		[HttpGet]
		[Route("Payment/MomoResult")]
		public async Task<ActionResult> MomoCallback(MoMoRedirectResult result)
		{
			var timeStr = result.ResponseTime;
			var rawdate = timeStr.Split(' ')[0].Split('-');
			DateTime date = new(int.Parse(rawdate[0]), int.Parse(rawdate[1]), int.Parse(rawdate[2]));

			bool success = SaveToDB(result.OrderInfo, date, result.TransId, result.Amount, result.Message, "Success");
			return Redirect(resultRedirectUrl + "?msg=" + (success ? "Success" : "Fail"));
		}

		// Create info for both methods.It will have the following format:
		// Type: (type), User id: (userId), Email: (email), Court id: (courtId), Date: (bookingDate), Start hour: (start), End hour: (end), Number of month: (numMonth) (fixed only)
		// As for buying time:
		// Type: Buy Time, User id: (userId), Email: (email)

		public bool SaveToDB(string info, DateTime responseDate, string transId, string amountStr, string expected, string actual)
		{
			if (expected != actual) 
				return false;

			string mailBody = "Thanks for your purchasement. ";
			var map = TransformToDictionary(info, '|', ':');
			string userId = map["User"];
			User user = _service.UserService.GetUserById(userId);
			double amount = double.Parse(amountStr);
			Payment payment = new()
			{
				PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"),
				Date = responseDate,
				UserId = userId,
				TransactionId = transId,
				Amount = amount
				//BookingId missing
			};
			if (map["Type"] != buyTime && map["Type"] != flexibleBooking) //Capture buyTime
			{
				string courtId = map["Court"];
				string bookingId = "BK" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7");
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

				_service.BookingService.AddBooking(booking);
				if (map["Type"] == fixedBooking)
				{
					int numMonth = int.Parse(map["Number of months"]);
					List<BookedSlot> slotList = _service.SlotService.GetSlotsByFixedBooking(
						numMonth,
						new DateTime(date.Year, date.Month, date.Day, start, 0, 0),
						new DateTime(date.Year, date.Month, date.Day, end, 0, 0),
						courtId
						);
					foreach (var item in slotList)
					{
						item.BookingId = bookingId;
						item.SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7");
						_service.SlotService.AddSlot(item);
					}
				}
				else if (map["Type"] == playonceBooking)
				{
					_service.SlotService.AddSlot(new BookedSlot
					{
						SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"),
						BookingId = bookingId,
						CourtId = courtId,
						StartTime = new DateTime(date.Year, date.Month, date.Day, start, 0, 0),
						EndTime = new DateTime(date.Year, date.Month, date.Day, end, 0, 0)
					});
				}
				mailBody += $"You have successfully booked the slot {rawDate} {start}h - {end}h in Court{courtId} with type of {map["Type"]}. ";
			}
			else
			{
				user.Balance += amount;
				_service.UserService.UpdateUser(user, userId);
				mailBody += $"Your balance was successfully added with the amount of {amount}. ";
			}
			_service.PaymentService.AddPayment(payment);
			mailBody += "You now can check it in payment history.";
			// _service.MailService.SendMail(map["Email"], mailBody, "BMTC - Booking Notification");
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