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

namespace BadmintonCourtAPI.Controllers
{
	public class PaymentController : Controller
	{
		private readonly BadmintonCourtService _service = null;

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
				content += $" Amount: {model.Amount * 1000}";
				vnPayRequestDTO = new VnPayRequestDTO { Amount = float.Parse(model.Amount.ToString()) * 1000, Content = content, Date = DateTime.Now, UserId = model.UserId };
			}
			else
			{
				if (model.Start != null && model.End != null)
				{
					if (model.Start > model.End)
					{
						if (model.Date != null)
						{
							if (model.Date > DateOnly.Parse(DateTime.Now.ToString()))
							{
								if (!model.CourtId.IsNullOrEmpty())
								{
									Court court = _service.CourtService.GetCourtByCourtId(model.CourtId);
									List<BookedSlot> tmpStorage = _service.SlotService.GetA_CourtSlotsInTimeInterval(new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.Start.Value, 0, 0), new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.End.Value, 0, 0), model.CourtId);
									//---------------------------------------------------------
									if (tmpStorage.Count > 0 || tmpStorage != null) // Nếu sân có người đặt trước
										return "Slot";
									//---------------------------------------------------------
									if (model.Type == "playonce") // Đặt loại 1 lần chơi
									{
										content += $" Date: {DateOnly.Parse(model.Date.ToString())} {model.Start}h - {model.End}h | Court: {model.CourtId}";
										vnPayRequestDTO = new VnPayRequestDTO { Amount = (model.End - model.Start) * court.Price * 1000, Content = content, UserId = model.UserId };

									}
									//---------------------------------------------------------
									else  // Cố định. Vd: ngày 1/1/2001 15h-17h 2 tháng sân A
									{
										content += $" Start date on schedule: {DateOnly.Parse(model.Date.ToString())} {model.Start}h - {model.End}h | Court: {model.CourtId} | Number of months: {model.NumMonth}";
										vnPayRequestDTO = new VnPayRequestDTO { Amount = (model.End - model.Start) * court.Price * model.NumMonth * 4 * 1000, Content = content, UserId = model.UserId };
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
			//---------------------------------------------------------
			if (model.Type == "flexible")
				content += $" Amount: {model.Amount * 1000}";
			//---------------------------------------------------------
			else
			{
				if (model.Start != null && model.End != null)
				{
					if (model.Start > model.End)
					{
						if (model.Date != null)
						{
							if (model.Date > DateOnly.Parse(DateTime.Now.ToString()))
							{
								if (!model.CourtId.IsNullOrEmpty())
								{
									Court court = _service.CourtService.GetCourtByCourtId(model.CourtId);
									List<BookedSlot> tmpStorage = _service.SlotService.GetA_CourtSlotsInTimeInterval(new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.Start.Value, 0, 0), new DateTime(model.Date.Value.Year, model.Date.Value.Month, model.Date.Value.Day, model.End.Value, 0, 0), model.CourtId);
									//---------------------------------------------------------
									if (tmpStorage.Count > 0 || tmpStorage != null) // Nếu sân có người đặt trước
										return "Slot";
									//---------------------------------------------------------
									if (model.Type == "playonce") // Đặt loại 1 lần chơi
										content += $" Date: {DateOnly.Parse(model.Date.ToString())} {model.Start}h - {model.End}h | Court: {model.CourtId}";
									//---------------------------------------------------------
									else  // Cố định. Vd: ngày 1/1/2001 15h-17h 2 tháng sân A	
										content += $" Start date on schedule: {DateOnly.Parse(model.Date.ToString())} {model.Start}h - {model.End}h | Court: {model.CourtId} | Number of months: {model.NumMonth}";

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
			var reqdata = _service.MomoService.CreateRequestData(content, (model.Amount * 1000).ToString(), "");
			var response = _service.MomoService.SendMoMoRequest(reqdata);
			return response.Result.PayUrl;
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
		public async Task<ActionResult<IEnumerable<Payment>>> GetUserPaymentsByDemand(int? order, DateOnly? start, DateOnly? end, string id)
		{
			if (start > end)
				return BadRequest(new { msg = "Invalid time interval" });
			start = start == null ? new DateOnly(2000, 1, 1) : start;
			end = end == null ? new DateOnly(2100, 1, 1) : end;


			if (order == 1) // Sort tăng dần theo ngày
				return Ok(_service.PaymentService.GetPaymentsByUserId(id).Where(x => x.Date >= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).OrderBy(x => x.Amount).ToList());

			else if (order == 2)
				return Ok(_service.PaymentService.GetPaymentsByUserId(id).Where(x => x.Date >= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).OrderByDescending(x => x.Amount).ToList());

			return Ok(_service.PaymentService.GetPaymentsByUserId(id).Where(x => x.Date >= new DateTime(start.Value.Year, start.Value.Month, start.Value.Day, 0, 0, 0) && x.Date <= new DateTime(end.Value.Year, end.Value.Month, end.Value.Day, 23, 59, 59)).ToList()); // Default
		}


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
				return BadRequest(new { msg = $"Court {model.CourtId} on {DateOnly.Parse(model.Date.ToString())} {model.Start}h - {model.End}h has been booked" });
			return Ok(new { url = url });
		}


		[HttpGet]
		//[Authorize]
		[Route("Payment/VnPayResult")]
		public async Task<ActionResult> VnPayPaymentCallBack()
		{
			VnPayResponseDTO result = _service.VnPayService.PaymentExecute(Request.Query);
			if (result == null || result.VnPayResponseCode != "00" && result.Status == false)
				return BadRequest(new { msg = "Fail" });

			string userId = result.Description.Split('|')[1].Trim().Split(':')[1].Trim();
			string courtId = result.Description.Split('|')[5].Trim().Split(':')[1].Trim();
			double amount = result.Amount;
			User user = _service.UserService.GetUserById(userId);
			// Loc bang keyword: date
			// Loc lop t2: loc bang startdate on schdeule
			// lop cuoi cung con lai la flexible
			//-----------------------------------------------------------------------------
			// Tạo payment
			Payment payment = new Payment { PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"), Date = result.Date, Method = 1, UserId = userId, TransactionId = result.TransactionId, Amount = amount };

			if (result.Description.ToLower().Contains("date")) // Loc dc loai flexible
			{
				string bookingId = "BK" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7");
				payment.BookingId = bookingId; // Nếu 1 lần chơi hoặc cố định thì payment sẽ có bookingId
											   //---------------------------------------------------------------------
				string rawDate = result.Description.Split('|')[4].Trim().Split(':')[1].Trim();
				DateOnly date = DateOnly.Parse(rawDate.Split(' ')[0].Trim());
				int start = int.Parse(new string(rawDate.Split(' ')[1].Where(char.IsDigit).ToArray()));
				int end = int.Parse(new string(rawDate.Split(' ')[3].Where(char.IsDigit).ToArray()));

				if (result.Description.Contains("Start date on schedule")) // Choi thang
				{
					_service.BookingService.AddBooking(new Booking { BookingId = bookingId, BookingType = 2, Amount = amount / 1000, UserId = userId }); // Tạo booking
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
				}
				//-------------------------------------------------------------
				else // Choi ngay`, choi 1 lan
				{
					_service.BookingService.AddBooking(new Booking { BookingId = bookingId, BookingType = 1, Amount = amount / 1000, UserId = userId });
					_service.SlotService.AddSlot(new BookedSlot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = bookingId, CourtId = courtId, StartTime = new DateTime(date.Year, date.Month, date.Day, start, 0, 0), EndTime = new DateTime(date.Year, date.Month, date.Day, end, 0, 0) });
				}
			}
			//-------------------------------------------------------------
			else // Mua so du
			{
				user.Balance += result.Amount / 1000;
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
					amount = amount / 1000;
					user.Balance += (amount * discount.Proportion) / 100;
					_service.UserService.UpdateUser(user, userId);
					break;
				}
			}
			return Ok(new { msg = "Success" });
		}

		[HttpGet]
		[Route("Payment/MomoResult")]
		//[Authorize]
		public async Task<ActionResult> MoMoCallback(MoMoRedirectResult result)
		{
			if (result.Message == "Fail")
				return BadRequest(new { msg = "Transaction fail" });
			//-------------------------------------------------------
			string userId = result.OrderInfo.Split('|')[1].Trim().Split(':')[1].Trim();
			string courtId = result.OrderInfo.Split('|')[5].Trim().Split(':')[1].Trim();
			double amount = double.Parse(result.Amount);
			User user = _service.UserService.GetUserById(userId);
			// Loc bang keyword: date
			// Loc lop t2: loc bang startdate on schdeule
			// lop cuoi cung con lai la flexible
			//-----------------------------------------------------------------------------
			// Tạo payment
			Payment payment = new Payment { PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"), Date = DateTime.Parse(result.ResponseTime), Method = 1, UserId = userId, TransactionId = result.TransId, Amount = amount };
			if (result.OrderInfo.ToLower().Contains("date")) // Loc dc loai flexible
			{
				string bookingId = "BK" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7");
				payment.BookingId = bookingId; // Nếu 1 lần chơi hoặc cố định thì payment sẽ có bookingId
											   //---------------------------------------------------------------------
				string rawDate = result.OrderInfo.Split('|')[4].Trim().Split(':')[1].Trim();
				DateOnly date = DateOnly.Parse(rawDate.Split(' ')[0].Trim());
				int start = int.Parse(new string(rawDate.Split(' ')[1].Where(char.IsDigit).ToArray()));
				int end = int.Parse(new string(rawDate.Split(' ')[3].Where(char.IsDigit).ToArray()));

				if (result.OrderInfo.Contains("Start date on schedule")) // Choi thang
				{
					_service.BookingService.AddBooking(new Booking { BookingId = bookingId, BookingType = 2, Amount = amount / 1000, UserId = userId }); // Tạo booking
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
					_service.BookingService.AddBooking(new Booking { BookingId = bookingId, BookingType = 1, Amount = amount / 1000, UserId = userId });
					_service.SlotService.AddSlot(new BookedSlot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = bookingId, CourtId = courtId, StartTime = new DateTime(date.Year, date.Month, date.Day, start, 0, 0), EndTime = new DateTime(date.Year, date.Month, date.Day, end, 0, 0) });
				}
			}
			//-------------------------------------------------------------
			else // Mua so du
			{
				user.Balance += amount / 1000;
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
