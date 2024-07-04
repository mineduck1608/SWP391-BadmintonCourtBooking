using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.VnPay;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BadmintonCourtAPI.Utils;
using Microsoft.IdentityModel.Tokens;
using Microsoft.NET.StringTools;
using BadmintonCourtBusinessObjects.SupportEntities.Slot;
using BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.Momo;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Primitives;
namespace BadmintonCourtAPI.Controllers
{
	public class SlotController : Controller
	{
		private readonly BadmintonCourtService _service;
		private const string resultRedirectUrl = "http:/localhost:3000/paySuccess";
		private const string updateResultMomoCallBack = "https://localhost:7233/Slot/UpdateResultProcessMomo";
		private const string updateResultVnPayCallBack = "https://localhost:7233/Slot/UpdateResultProcessVnPay";
		public SlotController(IConfiguration config)
		{
			if (_service == null)
			{
				_service = new BadmintonCourtService(config);
			}
		}

		private void ExtractDataForUpdatingSlot(string slotId, out Payment? payment, out BookedSlot slot, out Court? court, out User? user, out Booking booking)
		{
			slot = _service.SlotService.GetSlotById(slotId);
			court = _service.CourtService.GetCourtByCourtId(slot.CourtId);
			payment = _service.PaymentService.GetPaymentByBookingId(slot.BookingId);
			booking = _service.BookingService.GetBookingByBookingId(slot.BookingId);
			user = _service.UserService.GetUserById(booking.UserId);
		}

		private void ExtractSlotAndTime(string date, int start, int end, string courtId, out DateTime startDate, out DateTime endDate, out List<BookedSlot> tmpStorage)
		{
			string[] dateComponents = date.Split('-');
			startDate = new DateTime(int.Parse(dateComponents[0]), int.Parse(dateComponents[1]), int.Parse(dateComponents[2]), start, 0, 0);
			endDate = new DateTime(int.Parse(dateComponents[0]), int.Parse(dateComponents[1]), int.Parse(dateComponents[2]), end, 0, 0);
			tmpStorage = _service.SlotService.GetA_CourtSlotsInTimeInterval(startDate, endDate, courtId);
		}
		private bool IsTimeIntervalValid(string? date, int? start, int? end, string? courtId)
		{
			BookedSlot primitive = _service.SlotService.GetSlotById("S1");
			if (date.IsNullOrEmpty() || start >= end || courtId.IsNullOrEmpty() || start < primitive.StartTime.Hour || end > primitive.EndTime.Hour || courtId.IsNullOrEmpty() || _service.CourtService.GetCourtByCourtId(courtId).CourtStatus == false)
				return false;
			return true;
		}

		private void SupportUpdateSlotEnoughBalnce(double tmpBalance, double newAmount, double refund, string courtId, DateTime startDate, DateTime endDate, Booking booking, BookedSlot slot, User user)
		{
			// Update số dư
			user.Balance = tmpBalance - newAmount; // Tru bot cho phan dat slot moi
			_service.UserService.UpdateUser(user, user.UserId);
			//----------------------------------------
			// Update booking
			if (booking.BookingType == 1) // 1 lần chơi -> Thay giá trị booking cũ thành mới
				booking.Amount = newAmount;
			else // Chơi tháng
			{
				booking.Amount -= refund; // Bỏ giá trị của 1 slot cũ
				booking.Amount += newAmount; // Thêm giá trị của slot mới vừa thay đổi
			}
			booking.ChangeLog += 1;
			_service.BookingService.UpdatBooking(booking, booking.BookingId);
			slot.StartTime = startDate;
			slot.EndTime = endDate;
			slot.CourtId = courtId;
			_service.SlotService.UpdateSlot(slot, slot.SlotId);
		}

		private void ExtractAmountToValidateCondition(BookedSlot slot, Booking booking, DateTime startDate, DateTime endDate, string courtId, out double refund, out double tmpBalance, out double newAmount)
		{
			Court oCourt = _service.CourtService.GetCourtByCourtId(slot.CourtId);
			Court court = _service.CourtService.GetCourtByCourtId(courtId);
			User user = _service.UserService.GetUserById(booking.UserId);
			refund = oCourt.Price * (slot.EndTime.Hour - slot.StartTime.Hour);
			tmpBalance = user.Balance.Value + refund;
			newAmount = (endDate.Hour - startDate.Hour) * court.Price;
		}
		private Dictionary<string, string> ExtractPaymentInfo(string content)
		{
			string[] components = content.Split('|');
			Dictionary<string, string> result = new Dictionary<string, string>();
			int tail = components.Length - 2;
			for (int i = 0; i < components.Length - 1; i++)
            {	
				string key = "";
				string value = "";
				string tmp = components[i].Trim();
				if (i == tail)
				{
					key = "Status";
					value = tmp;
				}
				else
				{
					key = components[i].Split(':')[0].Trim();
					value = components[i].Split(':')[1].Trim();
				}
				result.Add(key, value);
			}
			return result;
		}

		[HttpGet]
		[Route("Slot/GetAll")]
		public async Task<ActionResult<IEnumerable<BookedSlotSchema>>> GetAllSlots() => Ok(Util.FormatSlotList(_service.SlotService.GetAllSlots()));

		[HttpGet]
		[Route("Slot/GetByDemand")]

		public async Task<ActionResult<IEnumerable<BookedSlotSchema>>> GetSlotsByDemand(string? branchId, string? courtId, DateOnly? startDate, DateOnly? endDate)
		{
			DateTime start = startDate == null ? new DateTime(2000, 1, 1, 0, 0, 1) : new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 1);
			DateTime end = endDate == null ? new DateTime(3000, 1, 1, 23, 59, 59) : new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);
			if (start > end)
				return BadRequest(new { msg = "Invalid time interval" });

			// -----------------------------------------------------------
			if (branchId.IsNullOrEmpty() && courtId.IsNullOrEmpty()) // Full
				return Ok(Util.FormatSlotList(_service.SlotService.GetAllSlots().Where(x => x.StartTime >= start && x.EndTime <= end).ToList()));
			// -----------------------------------------------------------
			else if (!branchId.IsNullOrEmpty() && courtId.IsNullOrEmpty()) // Theo chi nhánh
			{
				List<Court> courtList = _service.CourtService.GetCourtsByBranchId(branchId);
				List<BookedSlot> result = new List<BookedSlot>();
				foreach (var court in courtList)
				{
					List<BookedSlot> tmpStorage = _service.SlotService.GetA_CourtSlotsInTimeInterval(start, end, court.CourtId);
					foreach (var slot in tmpStorage)
						result.Add(slot);
				}
				return Ok(result);
			}
			// -----------------------------------------------------------
			// Sân cụ thể
			return Ok(Util.FormatSlotList(_service.SlotService.GetA_CourtSlotsInTimeInterval(start, end, courtId)));
		}


		[HttpPost]
		[Route("Slot/BookingByBalance")]
		[Authorize]
		public async Task<IActionResult> AddBookedSLot(string? date, int? start, int? end, string? courtId, string userId)
		{
			if (!IsTimeIntervalValid(date, start, end, courtId))
				return BadRequest(new { msg = "Invalid time" });



			DateTime startDate = new();
			DateTime endDate = new();
			List<BookedSlot> tmpStorage = new();
			ExtractSlotAndTime(date, start.Value, end.Value, courtId, out startDate, out endDate, out tmpStorage);
			if (tmpStorage.Count == 0)
			{
				User user = _service.UserService.GetUserById(userId);
				Court court = _service.CourtService.GetCourtByCourtId(courtId);
				double amount = (end.Value - start.Value) * court.Price;
				if (user.Balance >= amount)
				{
					string bookingId = "BK" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7");
					_service.BookingService.AddBooking(new Booking { BookingId = bookingId, Amount = amount, BookingType = 1, UserId = userId, BookingDate = DateTime.Now, ChangeLog = 0 });
					_service.SlotService.AddSlot(new BookedSlot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = bookingId, CourtId = courtId, StartTime = startDate, EndTime = endDate,  });
					user.Balance -= amount;
					_service.UserService.UpdateUser(user, userId);
					return Ok(new { msg = "Success" });
				}
				return BadRequest(new { msg = "Balance not enough" });
			}
			return BadRequest(new { msg = "This slot has been booked" });
		}

		[HttpPost]
		[Route("Slot/GetSlotCourtInDay")]
		public async Task<ActionResult<IEnumerable<BookedSlotSchema>>> GetA_CourtSlotsInDay(DateTime? date, string id) => date == null ? BadRequest(new { msg = "Date can't be empty" }) : Ok(Util.FormatSlotList(_service.SlotService.GetA_CourtSlotsInTimeInterval(new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, 0, 0, 1), new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, 23, 59, 59), id).ToList()));

		[HttpPost]
		[Route("Slot/GetSlotCourtInInterval")]
		public async Task<ActionResult<IEnumerable<BookedSlotSchema>>> GetA_CourtSlotsInInterval(DateTime startTime, DateTime endTime, string id) => (startTime == null || endTime == null) ? BadRequest(new { msg = "Date can't be empty" }) : Ok(_service.SlotService.GetA_CourtSlotsInTimeInterval(startTime, endTime, id));

		[HttpGet]
		[Route("Slot/GetBeforeConfirm")]
		public async Task<ActionResult<IEnumerable<BookedSlotSchema>>> GetSlotsByFixedBookingBeforeConfirm(int monthNum, DateTime date, int start, int end, string id) => Ok(Util.FormatSlotList(_service.SlotService.GetSlotsByFixedBooking(monthNum, new DateTime(date.Year, date.Month, date.Day, start, 0, 0), new DateTime(date.Year, date.Month, date.Day, end, 0, 0), id).ToList()));


		[HttpPut]
		[Route("Slot/UpdateOfficeHours")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateOfficeHours(int start, int end)
		{
			if (start >= end)
				return BadRequest(new { msg = "End must be bigger than start" });
			BookedSlot slot = _service.SlotService.GetSlotById("S1");
			slot.StartTime = new DateTime(1900, 1, 1, start, 0, 0);
			slot.EndTime = new DateTime(1900, 1, 1, end, 0, 0);
			_service.SlotService.UpdateSlot(slot, "S1");
			return Ok(new { msg = "Success" });
		}

		[HttpPut]
		[Route("Slot/UpdateByStaff")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<IActionResult> UpdateSlotByStaff(string? date, int? start, int? end, string slotId, string? courtId, string bookingId)
		{
			if (IsTimeIntervalValid(date, start, end, courtId))
				return BadRequest(new { msg = "Invalid time" });
		

			Booking booking = _service.BookingService.GetBookingByBookingId(bookingId);
			if (booking.ChangeLog < 2)
			{
				BookedSlot slot = _service.SlotService.GetSlotById(slotId);
				DateTime startDate = new();
				DateTime endDate = new();
				List<BookedSlot> tmpStorage = new();
				ExtractSlotAndTime(date, start.Value, end.Value, courtId, out startDate, out endDate, out tmpStorage);
				if (courtId == slot.CourtId) // Cũng là sân cũ
				{
					tmpStorage.Remove(slot); // Bỏ slot gốc để tránh quét phải
				}
				if (tmpStorage.Count > 0) // Khung giờ đấy của sân mới đã kẹt
					return BadRequest(new { msg = $"Court {courtId} between {startDate} to {endDate} has been booked" });

				double refund;
				double tmpBalance;
				double newAmount;
				ExtractAmountToValidateCondition(slot, booking, startDate, endDate, courtId, out refund, out tmpBalance, out newAmount);
				User user = _service.UserService.GetUserById(booking.UserId);
				if (tmpBalance >= newAmount)
				{
					SupportUpdateSlotEnoughBalnce(tmpBalance, newAmount, refund, courtId, startDate, endDate, booking, slot, user);
					return Ok(new { msg = "Success" });
				}
				return BadRequest(new { msg = "Balance not enough" });

			}
			return BadRequest(new { msg = "Can't change" });
		}

		// Status: on going
		[HttpPut]
		[Route("Slot/UpdateByUser")]
		[Authorize]
		public async Task<IActionResult> UpdateSlotByUser(int? start, int? end, string? date, string userId, string courtId, string slotId, int? paymentMethod, string bookingId) // Khách đổi ý muốn thay đổi sân / thời gian / ....
		{
			if (!IsTimeIntervalValid(date, start, end, courtId))
				return BadRequest(new { msg = "Invalid time" });

			Booking booking = _service.BookingService.GetBookingByBookingId(bookingId);
			//--------------------------------------------------------------------------------------
			if ((DateTime.Now - booking.BookingDate).TotalHours <= 1 && booking.ChangeLog < 2)
			{
				BookedSlot slot = _service.SlotService.GetSlotById(slotId);
				DateTime startDate = new();
				DateTime endDate = new();
				List<BookedSlot> tmpStorage = new();
				ExtractSlotAndTime(date, start.Value, end.Value, courtId, out startDate, out endDate, out tmpStorage);
				if (courtId == slot.CourtId) // Cũng là sân cũ
				{
					tmpStorage.Remove(slot); // Bỏ slot gốc để tránh quét phải
				}
				if (tmpStorage.Count > 0) // Khung giờ đấy của sân mới đã kẹt
					return BadRequest(new { msg = $"Court {courtId} between {startDate} to {endDate} has been booked" });

				double refund;
				double tmpBalance;
				double newAmount;
				ExtractAmountToValidateCondition(slot, booking, startDate, endDate, courtId, out refund, out tmpBalance, out newAmount);
				User user = _service.UserService.GetUserById(userId);
				// ---------------------------- 
				if (tmpBalance >= newAmount) // Check số dư sau khi hoàn đã đủ tiền thanh toán
				{
					SupportUpdateSlotEnoughBalnce(tmpBalance, newAmount, refund, courtId, startDate, endDate, booking, slot, user);
				}
				//----------------------------------------------------
				// Sau khi đã giả định số dư tổng sẽ có sau khi đc hoàn mà vẫn ko đủ thì tiến hành cho khách thực hiện giao dịch qua app bank / momo
				else
				{
					UserDetail info = _service.UserDetailService.GetUserDetailById(userId);
					string content = $"User: {info.FirstName} {info.LastName} | ID: {userId} | Phone: {info.Phone} | Mail: {info.Email} | Date: {date} {start}h - {end}h | Court: {courtId} | Booking: {booking.BookingId} | Slot: {slotId} |";
					double transactionAmount = (newAmount - tmpBalance) < 10000 ? float.Parse(newAmount.ToString()) : (newAmount - tmpBalance);
					paymentMethod = paymentMethod == null ? 1 : paymentMethod;
					// Thực hiện giao dịch thanh toán
					if (transactionAmount < 10000) // GIao dịch VNPay chưa tới 10k -> thanh toán full ko cấn số dư
						content += " Balance not enough | Change Slot";
					else // Sau khi cấn số dư thì số tiền cần phải thanh toán trên 10k -> tiến hành cook giao dịch app bank
						 // Thực hiện giao dịch thanh toán
						content += " Balance enough | Change Slot";
					//------------------------------------------------------------------------
					string paymentUrl = "";
					if (paymentMethod == 1)
					{
						paymentUrl = _service.VnPayService.CreatePaymentUrl(HttpContext, new VnPayRequestDTO
						{
							Content = content,
							Amount = float.Parse(transactionAmount.ToString())
						},
						updateResultVnPayCallBack
						);
					}
					else
					{
						var reqdata = _service.MomoService.CreateRequestData(content, (transactionAmount * 1000).ToString(), updateResultMomoCallBack);
						var response = _service.MomoService.SendMoMoRequest(reqdata);
						paymentUrl = response.Result.PayUrl;
					}
					return Ok(new { url = paymentUrl });
				}
				return Ok(new { msg = "Success" });
			}
			return BadRequest(new { msg = "Can't change as out of time to change decision" });
		}

		[HttpDelete]
		[Route("Slot/CancelByStaff")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<IActionResult> CancelSlotByStaff(string slotId) // Nhân viên hủy slot
		{
			Booking booking = new();
			BookedSlot slot = new();
			Court court = new();
			User user = new();
			Payment payment = new();
			ExtractDataForUpdatingSlot(slotId, out payment, out slot, out court, out user, out booking);
			float refund = court.Price * (slot.EndTime.Hour - slot.StartTime.Hour);

			slot.IsDeleted = true;
			_service.SlotService.UpdateSlot(slot, slotId);
			if (booking.BookingType == 1) // 1 lan choi
			{
				booking.IsDeleted = true;
				if (payment != null) // 1 lan choi dat = tien
				{
					payment.BookingId = null;
					_service.PaymentService.UpdatePayment(payment, payment.PaymentId);
				}
			}
			else booking.Amount -= refund; // Choi thang' / choi co dinh
			_service.BookingService.UpdatBooking(booking, booking.BookingId);
			user.Balance += refund;
			_service.UserService.UpdateUser(user, user.UserId);
			return Ok(new { msg = "Success" });
		}


		[HttpDelete]
		[Route("Slot/Cancel")]
		[Authorize]
		public async Task<IActionResult> CancelSlot(string slotId, string bookingId) // Khách hàng chủ động hủy slot
		{
			Booking booking = _service.BookingService.GetBookingByBookingId(bookingId);
			if ((DateTime.Now - booking.BookingDate).TotalHours <= 1 && booking.ChangeLog < 2)
			{
				BookedSlot slot = new();
				Court court = new();
				User user = new();
				Payment payment = new();
				ExtractDataForUpdatingSlot(slotId, out payment, out slot, out court, out user, out booking);
				float refund = court.Price * (slot.EndTime.Hour - slot.StartTime.Hour);

				slot.IsDeleted = true;
				_service.SlotService.UpdateSlot(slot, slotId);
				if (booking.BookingType == 1) // 1 lan choi
				{
					booking.IsDeleted = true;
					if (payment != null) // 1 lan choi dat = tien
					{
						payment.BookingId = null;
						_service.PaymentService.UpdatePayment(payment, payment.PaymentId);
					}
				}
				else
				{
					booking.Amount -= refund; // Choi thang' / choi co dinh
					booking.ChangeLog += 1;
				}
				_service.BookingService.UpdatBooking(booking, booking.BookingId);
				user.Balance += refund / 2;
				_service.UserService.UpdateUser(user, user.UserId);
				return Ok(new { msg = "Success" });
			}
			return BadRequest(new { msg = "Can't change" });
		}

		[HttpGet]
		[Route("Slot/UpdateResultProcessVnPay")]
		public async Task<IActionResult> ProcessResultVnpay()
		{
			VnPayResponseDTO result = _service.VnPayService.PaymentExecute(Request.Query);
			bool status = UpdateNewSlotToDB("00", result.VnPayResponseCode, result.Description, result.TransactionId, result.Amount, result.Date, 1);
			if (status)
			{
				string userId = result.Description.Split('|')[1].Trim().Split(':')[1].Trim();
                _service.MailService.SendMail(_service.UserDetailService.GetUserDetailById(userId).Email, $@"
<p>Dear {_service.UserDetailService.GetUserDetailById(userId).FirstName + " " + _service.UserDetailService.GetUserDetailById(userId).LastName},</p>
<p>Thank you for your purchase. Your slot has been updated. You can now check it on your account.</p>
<p>If you have any questions or need further assistance, please contact us.</p>
<p>Best regards,<br>
BadmintonCourtBooking BMTC</p>
<p>Contact Information:<br>
Phone: 0977300916<br>
Address: 123 Badminton St, Hanoi, Vietnam<br>
Email: externalauthdemo1234@gmail.com</p>",
"BMTC - Slot Update Notification");

            }
            return Redirect(resultRedirectUrl + "?msg=" + (status ? "Success" : "Fail"));
		}

		private bool UpdateNewSlotToDB(string expected, string actual, string content, string transactionId, double amount, DateTime transactionPeriod, int method)
		{
			if (expected != actual) 
				return false;

			Dictionary<string, string> infoStorage = ExtractPaymentInfo(content);
			//---------------------------------------------------------------------
			string userId = infoStorage["ID"];
			User user = _service.UserService.GetUserById(userId);
			//---------------------------------------------------------------------
			string rawDate = infoStorage["Date"];
			DateOnly date = DateOnly.Parse(rawDate.Split(' ')[0]);
			int start = int.Parse(new string(rawDate.Split(' ')[1].Where(char.IsDigit).ToArray()));
			int end = int.Parse(new string(rawDate.Split(' ')[3].Where(char.IsDigit).ToArray()));
			//---------------------------------------------------------------------
			string courtId = infoStorage["Court"];
			//---------------------------------------------------------------------
			string bookingId = infoStorage["Booking"];
			Booking booking = _service.BookingService.GetBookingByBookingId(bookingId);
			//---------------------------------------------------------------------
			string slotId = infoStorage["Slot"];
			BookedSlot slot = _service.SlotService.GetSlotById(slotId);
			Court oCourt = _service.CourtService.GetCourtByCourtId(slot.CourtId);
			//---------------------------------------------------------------------
			string paymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7");
			_service.PaymentService.AddPayment(new Payment
			{
				PaymentId = paymentId,
				Amount = amount,
				BookingId = bookingId,
				Method = method,
				TransactionId = transactionId,
				Date = transactionPeriod,
				UserId = userId
			});

			if (infoStorage["Status"] == "Balance enough")
			{
				user.Balance = 0;
				booking.Amount += amount;
			}
			else
			{
				double oldPrice = oCourt.Price * (slot.EndTime.Hour - slot.StartTime.Hour);
				user.Balance += oldPrice;
				if (booking.BookingType == 2)
				{
					booking.Amount -= oldPrice;
					booking.Amount += amount;
				}
				else booking.Amount = amount;
			}
			//---------------------------------------------------------------------
			_service.UserService.UpdateUser(user, userId);
			//---------------------------------------------------------------------
			_service.BookingService.UpdatBooking(booking, bookingId);
			//---------------------------------------------------------------------
			slot.CourtId = courtId;
			slot.StartTime = new DateTime(date.Year, date.Month, date.Day, start, 0, 0);
			slot.EndTime = new DateTime(date.Year, date.Month, date.Day, end, 0, 0);
			_service.SlotService.UpdateSlot(slot, slotId);
			return true;
		}

		[HttpGet]
		[Route("Slot/UpdateResultProcessMomo")]
		public async Task<IActionResult> ProcessResultMomo(MoMoRedirectResult result)
		{
			bool status = UpdateNewSlotToDB("Success", result.Message, result.OrderInfo, result.TransId, double.Parse(result.Amount), DateTime.Parse(result.ResponseTime.Split(' ')[0]), 2);
			if (status)
			{
				string userId = result.OrderInfo.Split('|')[0].Trim().Split(':')[1].Trim();
                _service.MailService.SendMail(_service.UserDetailService.GetUserDetailById(userId).Email, $@"
<p>Dear {_service.UserDetailService.GetUserDetailById(userId).FirstName + " " + _service.UserDetailService.GetUserDetailById(userId).LastName},</p>
<p>Thank you for your purchase. Your slot has been updated. You can now check it on your account.</p>
<p>If you have any questions or need further assistance, please contact us.</p>
<p>Best regards,<br>
BadmintonCourtBooking BMTC</p>
<p>Contact Information:<br>
Phone: 0977300916<br>
Address: 123 Badminton St, Hanoi, Vietnam<br>
Email: externalauthdemo1234@gmail.com</p>",
"BMTC - Slot Update Notification");

            }
            return Redirect(resultRedirectUrl + "?msg=" + (status ? "Success" : "Fail"));
		}


	}
}