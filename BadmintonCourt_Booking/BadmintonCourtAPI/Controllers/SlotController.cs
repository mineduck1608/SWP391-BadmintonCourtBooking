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
namespace BadmintonCourtAPI.Controllers
{
	public class SlotController : Controller
	{
		private readonly BadmintonCourtService _service;
		public SlotController(IConfiguration config)
		{
			if (_service == null)
			{
				_service = new BadmintonCourtService(config);
			}
		}

		//[HttpPost]
		//[Route("Slot/AddDefault")]
		//public async Task<IActionResult> AddDefaultSlot(Slot slot)
		//      {
		//          service.slotService.AddSlot(slot);
		//          return Ok();
		//      }


		//[HttpPost]
		//[Route("Slot/AddBooked")]
		//      //[Authorize(Roles = "Admin,Staff")]
		//public async Task<IActionResult> AddBookedSLot(DateTime date, int start, int end, int bookingId, int courtId, int id)
		//      {
		//	service.slotService.AddSlot(new Slot(new DateTime(date.Year, date.Month, date.Day, start, 0, 0), new DateTime(date.Year, date.Month, date.Day, end, 0, 0), false, courtId, bookingId));
		//          return Ok();
		//      }



		[HttpGet]
		[Route("Slot/GetAll")]
		//[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<IEnumerable<BookedSlotSchema>>> GetAllSlots() => Ok(Util.FormatSlotList(_service.SlotService.GetAllSlots()));

		[HttpGet]
		[Route("Slot/GetByDemand")]
		//[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<IEnumerable<BookedSlotSchema>>> GetSlotsByDemand(string? branchId, string? courtId, DateOnly? startDate, DateOnly? endDate)
		{
			DateTime start = startDate == null ? new DateTime(1900, 1, 1, 0, 0, 1) : new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 1);
			DateTime end = endDate == null ? new DateTime(9000, 1, 1, 23, 59, 59) : new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);
			if (start > end)
				return BadRequest();

			// -----------------------------------------------------------
			if (branchId.IsNullOrEmpty() && courtId.IsNullOrEmpty()) // Full
				return Ok(_service.SlotService.GetAllSlots().Where(x => x.StartTime >= start && x.EndTime <= end).ToList());
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
		[Route("Slot/BookingByBalence")]
		//[Authorize]
		public async Task<IActionResult> AddBookedSLot(DateOnly date, int start, int end, string courtId, string userId)
		{
			User user = _service.UserService.GetUserById(userId);
			DateTime startDate = new DateTime(date.Year, date.Month, date.Day, start, 0, 0);
			DateTime endDate = new DateTime(date.Year, date.Month, date.Day, end, 0, 0);
			List<BookedSlot> tmpStorage = _service.SlotService.GetA_CourtSlotsInTimeInterval(startDate, endDate, courtId);
			if (tmpStorage == null || tmpStorage.Count == 0)
			{
				Court court = _service.CourtService.GetCourtByCourtId(courtId);
				double amount = (end - start) * court.Price;
				if (user.Balance >= amount)
				{
					_service.BookingService.AddBooking(new Booking { BookingId = "BK" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7"), Amount = amount, BookingType = 1, UserId = userId, BookingDate = DateTime.Now });
					_service.SlotService.AddSlot(new BookedSlot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = _service.BookingService.GetRecentAddedBooking().BookingId, CourtId = courtId, StartTime = startDate, EndTime = endDate });
					user.Balance -= (end - start);
					_service.UserService.UpdateUser(user, userId);
					return Ok(new { msg = "Success" });
				}
				return BadRequest(new { msg = "Balance not enough" });
			}
			return BadRequest(new { msg = "This slot has been booked" });
		}

		[HttpPost]
		[Route("Slot/GetSLotCourtInDay")]
		public async Task<ActionResult<IEnumerable<BookedSlot>>> GetA_CourtSlotsInDay(DateTime date, string id) => _service.SlotService.GetA_CourtSlotsInTimeInterval(new DateTime(date.Year, date.Month, date.Day, 0, 0, 1), new DateTime(date.Year, date.Month, date.Day, 23, 59, 59), id).ToList();


		[HttpGet]
		[Route("Slot/GetBeforeConfirm")]
		public async Task<ActionResult<IEnumerable<BookedSlotSchema>>> GetSlotsByFixedBookingBeforeConfirm(int monthNum, DateTime date, int start, int end, string id) => Ok(Util.FormatSlotList(_service.SlotService.GetSlotsByFixedBooking(monthNum, new DateTime(date.Year, date.Month, date.Day, start, 0, 0), new DateTime(date.Year, date.Month, date.Day, end, 0, 0), id).ToList()));


		[HttpPut]
		[Route("Slot/UpdateOfficeHours")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateOfficeHours(int start, int end)
		{
			if (start >= end)
				return BadRequest(new { msg = "End must be bigger than start" });
			BookedSlot slot = _service.SlotService.GetSlotById("S1");
			slot.StartTime = Util.CustomizeDate(start);
			slot.EndTime = Util.CustomizeDate(end);
			_service.SlotService.UpdateSlot(slot, "S1");
			return Ok(new { msg = "Success" });
		}

		[HttpPut]
		[Route("Slot/UpdateByStaff")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateSlotByStaff(DateOnly date, int start, int end, string slotId, string courtId)
		{
			BookedSlot primitiveSlot = _service.SlotService.GetSlotById("S1");

			if (start >= end || start < primitiveSlot.StartTime.Hour || end > primitiveSlot.EndTime.Hour)
				return BadRequest(new { msg = "Invalid time" });


			// Check trường hợp sân đấy giờ đấy đang tạm bảo trì đổi thời gian + sân khác cho khách
			BookedSlot slot = _service.SlotService.GetSlotById(slotId);
			Booking booking = _service.BookingService.GetBookingByBookingId(slot.BookingId);

			DateTime startDate = new DateTime(date.Year, date.Month, date.Day, start, 0, 0);
			DateTime endDate = new DateTime(date.Year, date.Month, date.Day, end, 0, 0);
			//Court c1 = _service.CourtService.GetCourtByCourtId("C1");


			List<BookedSlot> tmpList = _service.SlotService.GetA_CourtSlotsInTimeInterval(startDate, endDate, courtId);
			if (courtId != slot.CourtId) // Thay đổi sân khác với sân gốc đã đặt
			{
				if (tmpList != null) // Khung giờ đấy của sân mới đã kẹt
					return BadRequest(new { msg = $"Court {courtId} between {startDate} to {endDate} has been booked" });
			}
			else // Cũng là sân cũ
			{
				tmpList.Remove(slot); // Bỏ slot gốc để tránh quét phải
				if (tmpList.Count > 0) // Kết quả sau khi bỏ slot gốc nếu danh sách vẫn còn tức là thời gian muốn thay đổi trong đấy vẫn bị cấn các slot đc đặt khác
					return BadRequest(new { msg = $"Court {courtId} between {startDate} to {endDate} has been booked" });
			}

			slot.StartTime = new DateTime(date.Year, date.Month, date.Day, start, 0, 0);
			slot.EndTime = new DateTime(date.Year, date.Month, date.Day, end, 0, 0);
			slot.CourtId = courtId;
			_service.SlotService.UpdateSlot(slot, slotId);

			//if (bookingId.IsNullOrEmpty() && status == true) // Tương đương hủy slot -> hoàn số dư
			//{
			//	Booking booking = _service.BookingService.GetBookingByBookingId(tmpBookingId);
			//	Payment payment = _service.PaymentService.GetPaymentByBookingId(tmpBookingId);
			//	if (payment == null) // Book = số dư
			//	{
			//		if (booking.BookingType == 1) // 1 lần chơi
			//			_service.BookingService.DeleteBooking(tmpBookingId);
			//	}
			//	else // Book = bank
			//	{
			//		if (booking.BookingType == 1) // 1 lần chơi
			//		{
			//			// Chỉnh bookingId của payment thành null rồi mới xóa booking 
			//			payment.BookingId = null;
			//			_service.PaymentService.UpdatePayment(payment, payment.PaymentId);
			//			// Xóa booking
			//			_service.BookingService.DeleteBooking(tmpBookingId);
			//		}
			//		// Nếu chơi cố định chơi tháng thì chỉ đơn thuần xóa slot và hoàn tiền. Ko xóa luôn book do những slot còn lại của chuỗi tháng đấy sẽ bị ảnh hưởng theo
			//	}
			//	// Hoàn số dư
			//	User user = _service.UserService.GetUserById(booking.UserId);
			//	user.Balance += refund;
			//	_service.UserService.UpdateUser(user, user.UserId);
			//}
			return Ok(new { msg = "Success" });
		}

		[HttpDelete]
		[Route("Slot/CancelByStaff")]
		//[Authorize]
		public async Task<IActionResult> CancelSlotByStaff(string slotId, string bookingId) // Nhân viên hủy slot
		{
			Booking booking = _service.BookingService.GetBookingByBookingId(bookingId);
			BookedSlot slot = _service.SlotService.GetSlotById(slotId);

			User user = _service.UserService.GetUserById(booking.UserId);
			Payment payment = _service.PaymentService.GetPaymentByBookingId(bookingId);
			int refund = slot.EndTime.Hour - slot.StartTime.Hour;
			_service.SlotService.DeleteSlot(slotId);
			if (payment == null) // Đặt sân = số dư
			{
				if (booking.BookingType == 1) // 1 lần chơi 
					_service.BookingService.DeleteBooking(bookingId);
			}
			// -----------------------------------------
			// Bank để đặt
			else
			{
				if (booking.BookingType == 1) // 1 lần chơi
				{
					// Chỉnh bookingId của payment thành null rồi mới xóa booking 
					payment.BookingId = null;
					_service.PaymentService.UpdatePayment(payment, payment.PaymentId);
					// Xóa booking
					_service.BookingService.DeleteBooking(bookingId);
				}
				// Nếu chơi cố định chơi tháng thì chỉ đơn thuần xóa slot và hoàn tiền. Ko xóa luôn book do những slot còn lại của chuỗi tháng đấy sẽ bị ảnh hưởng theo
			}
			user.Balance += refund;
			_service.UserService.UpdateUser(user, user.UserId);
			return Ok(new { msg = "Success" });
		}


		[HttpDelete]
		[Route("Slot/Cancel")]
		//[Authorize]
		public async Task<IActionResult> CancelSlot(string slotId, string bookingId) // Khách hàng chủ động hủy slot
		{
			Booking booking = _service.BookingService.GetBookingByBookingId(bookingId);

			BookedSlot slot = _service.SlotService.GetSlotById(slotId);
			Court court = _service.CourtService.GetCourtByCourtId(slot.CourtId);
			User user = _service.UserService.GetUserById(booking.UserId);
			Payment payment = _service.PaymentService.GetPaymentByBookingId(bookingId);
			double refund = (slot.EndTime.Hour - slot.StartTime.Hour) * court.Price;
			_service.SlotService.DeleteSlot(slotId);

			if (payment == null) // Đặt sân = số dư
			{
				if (booking.BookingType == 1) // 1 lần chơi 
					_service.BookingService.DeleteBooking(bookingId);
			}
			// -----------------------------------------
			// Bank để đặt
			else
			{
				if (booking.BookingType == 1) // 1 lần chơi
				{
					// Chỉnh bookingId của payment thành null rồi mới xóa booking 
					payment.BookingId = null;
					_service.PaymentService.UpdatePayment(payment, payment.PaymentId);
					// Xóa booking
					_service.BookingService.DeleteBooking(bookingId);
				}
				// Nếu chơi cố định chơi tháng thì chỉ đơn thuần xóa slot và hoàn tiền. Ko xóa luôn book do những slot còn lại của chuỗi tháng đấy sẽ bị ảnh hưởng theo
			}
			user.Balance += refund;
			_service.UserService.UpdateUser(user, user.UserId);
			return Ok(new { msg = "Success" });
		}


		// Status: on going
		[HttpPut]
		[Route("Slot/UpdateByUser")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateSlotByUser(int? start, int? end, DateOnly? date, string userId, string courtId, string slotId, int? paymentMethod) // Khách đổi ý muốn thay đổi sân / thời gian / ....
		{
			if (start > end || start == null || end == null || date == null)
				return BadRequest(new { msg = "Invalid time" });

			BookedSlot slot = _service.SlotService.GetSlotById(slotId);
			Booking booking = _service.BookingService.GetBookingByBookingId(slot.BookingId);
			//--------------------------------------------------------------------------------------
			if ((DateTime.Now - booking.BookingDate).TotalHours <= 1)
			{
				DateTime startDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, start.Value, 0, 0);
				DateTime endDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, end.Value, 0, 0);
				if (slot.CourtId == courtId && slot.StartTime == startDate && slot.EndTime == endDate) // Chọn nhầm lại y chang cũ ko khác gì
					return Ok(new { msg = "Success" });

			
				Court oCourt = _service.CourtService.GetCourtByCourtId(slot.CourtId);
				Court court = _service.CourtService.GetCourtByCourtId(courtId);
				List<BookedSlot> tmpList = _service.SlotService.GetA_CourtSlotsInTimeInterval(startDate, endDate, courtId);
				//--------------------------------------------------------------------------------------
				if (courtId != slot.CourtId) // Thay đổi sân khác với sân gốc đã đặt
				{
					if (tmpList != null) // Khung giờ đấy của sân mới đã kẹt
						return BadRequest(new { msg = $"Court {courtId} between {startDate} to {endDate} has been booked" });
				}
				else // Cũng là sân cũ
				{
					tmpList.Remove(slot); // Bỏ slot gốc để tránh quét phải
					if (tmpList.Count > 0) // Kết quả sau khi bỏ slot gốc nếu danh sách vẫn còn tức là thời gian muốn thay đổi trong đấy vẫn bị cấn các slot đc đặt khác
						return BadRequest(new { msg = $"Court {courtId} between {startDate} to {endDate} has been booked" });
				}

				User user = _service.UserService.GetUserById(userId);
				UserDetail info = _service.UserDetailService.GetUserDetailById(userId);
				int oInterval = slot.EndTime.Hour - slot.StartTime.Hour; // Thời lượng cũ trc khi sửa
				int nInterval = end.Value - start.Value; // Thời lượng mới sau khi sửa
				paymentMethod = paymentMethod == null ? 1 : paymentMethod; // Bắt buộc chọn phương thức thanh toán khi thay đổi giờ chơi
																		   // Ko chọn sẽ mặc định vnpay

				double tmpBalance = user.Balance.Value + booking.Amount; // Giả sử tổng số dư của khách sau khi đc hoàn tiền
				double newAmount = nInterval * court.Price;
				// ---------------------------- 
				user.Balance = tmpBalance;  // Chỉnh lại số dư thành số dư giả định  
				if (tmpBalance >= newAmount) // Check số dư sau khi hoàn đã đủ tiền thanh toán
				{
					// Update số dư
					user.Balance -= newAmount; // Tru bot cho phan dat slot moi
					_service.UserService.UpdateUser(user, userId);
					//----------------------------------------
					// Update booking
					if (booking.BookingType == 1) // 1 lần chơi -> Thay giá trị booking cũ thành mới
						booking.Amount = newAmount;
					else // Chơi tháng
					{
						booking.Amount -= oInterval * oCourt.Price; // Bỏ giá trị của 1 slot cũ
						booking.Amount += newAmount; // Thêm giá trị của slot mới vừa thay đổi
					}
					_service.BookingService.UpdatBooking(booking, booking.BookingId); // Update
					//-------------------------------------------------
					// Update slot
					slot.StartTime = startDate;
					slot.EndTime = endDate;
					slot.CourtId = courtId;
					_service.SlotService.UpdateSlot(slot, slotId);
				}
				//----------------------------------------------------
				// Sau khi đã giả định số dư tổng sẽ có sau khi đc hoàn mà vẫn ko đủ thì tiến hành cho khách thực hiện giao dịch qua app bank / momo
				else
				{
					string content = $"User: {info.FirstName} {info.LastName} | ID: {userId} | Phone: {info.Phone} | Mail: {info.Email} | Date: {date} {start}h - {end}h | Court: {courtId} | Booking: {booking.BookingId} | Slot: {slotId} |";
					double transactionAmount = (newAmount - tmpBalance) < 10 ? float.Parse(newAmount.ToString()) : (newAmount - tmpBalance);
					//-----------------------------------------------------


					// Thực hiện giao dịch thanh toán
					if (transactionAmount < 10) // GIao dịch VNPay chưa tới 10k -> thanh toán full ko cấn số dư
					{
						content += " Balance not enough | Change Slot";
					}
					else // Sau khi cấn số dư thì số tiền cần phải thanh toán trên 10k -> tiến hành cook giao dịch app bank
						 // Thực hiện giao dịch thanh toán
						content += " Balance enough | Change Slot";
					//------------------------------------------------------------------------
					//------------------------------------------------------------------------
					if (paymentMethod == 1)
						return Ok(new
						{
							url = _service.VnPayService.CreatePaymentUrl(HttpContext, new VnPayRequestDTO
							{
								Amount = float.Parse(transactionAmount.ToString()) * 1000,
								UserId = userId,
								Content = content
							},
							"https://localhost:7233/Slot/UpdateResultProcessVnPay")
						});
					else
					{
						var reqdata = _service.MomoService.CreateRequestData(content, (transactionAmount * 1000).ToString(), "https://localhost:7233/Slot/UpdateResultProcessMomo");
						var response = _service.MomoService.SendMoMoRequest(reqdata);
						return Ok(new { url = response.Result.PayUrl });
					}
				}
				return Ok(new { msg = "Success" });
			}
			return BadRequest(new { msg = "Can't cancel as out of time to change decision" });
		}


		[HttpGet]
		[Route("Slot/UpdateResultProcessVnPay")]
		public async Task<ActionResult> ProcessResultVnpay()
		{
			VnPayResponseDTO result = _service.VnPayService.PaymentExecute(Request.Query);
			if (result == null || result.VnPayResponseCode != "00" || result.Status == false)
				return BadRequest(new { msg = "Transaction failed! Can't update data" });

			//---------------------------------------------------------------------
			string userId = result.Description.Split('|')[1].Trim().Split(',')[1].Trim();
			User user = _service.UserService.GetUserById(userId);
			//------------------------------------------------------------------
			string rawDate = result.Description.Split('|')[4].Trim().Split(':')[1].Trim();
			DateOnly date = DateOnly.Parse(rawDate.Split(' ')[0]);
			int start = int.Parse(new string(rawDate.Split(' ')[1].Where(char.IsDigit).ToArray()));
			int end = int.Parse(new string(rawDate.Split(' ')[3].Where(char.IsDigit).ToArray()));
			//------------------------------------------------------------------
			string courtId = result.Description.Split('|')[6].Trim().Split(',')[1].Trim();
			Court court = _service.CourtService.GetCourtByCourtId(courtId);
			//------------------------------------------------------------------
			string bookingId = result.Description.Split('|')[6].Trim().Split(',')[1].Trim();
			Booking booking = _service.BookingService.GetBookingByBookingId(bookingId);
			//------------------------------------------------------------------
			string slotId = result.Description.Split('|')[7].Trim().Split(',')[1].Trim();
			BookedSlot slot = _service.SlotService.GetSlotById(slotId);
			Court oCourt = _service.CourtService.GetCourtByCourtId(slot.CourtId);
			//------------------------------------------------------------------
			// Giao dịch thành công
			_service.PaymentService.AddPayment(new Payment { PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"), BookingId = booking.BookingId, Date = result.Date, Method = 1, UserId = userId, TransactionId = result.TransactionId });
			//------------------------------------------------------------------

			if (result.Description.Split('|')[8].Trim().Equals("Balance enough")) // Giao dịch update số dư đã cấn ra và còn ít nhất từ 10k để thỏa giao dịch qua vnpay - tương đương giao dịch bên trang vnpay sẽ >= 10k
			{
				user.Balance = 0;  // Do cấn hết số dư vào đơn đặt này nên trả về lại 0
				booking.Amount += result.Amount / 1000; // Vì cấn toàn bộ số dư sau khi hoàn rồi mới thanh toán nên chỉ cần cộng thêm vào giá trị của giao dịch đã thanh toán từ payment ko quan tâm loại chơi 1 lần hay cố định
			}
			//------------------------------------------------------------------
			// Giao dịch update đã cấn ra và còn chưa tới 10k để giao dịch trên vnpay - Ko thể giao dịch với số tiền nhỏ hơn 10k -> giao dịch full tiền của slot mới mà ko đc cấn từ balance giả định đã hoàn
			else
			{
				user.Balance += (slot.EndTime.Hour - slot.StartTime.Hour) * oCourt.Price; // Hoàn lại số dư slot cũ đã đc hủy
				if (booking.BookingType == 1) // 1 lần chơi -> Thay luôn giá trị đơn cũ thành đơn mới
					booking.Amount = result.Amount / 1000;
				//-----------------------------------------------------			
				else if (booking.BookingType == 2) // Chơi tháng
				{
					booking.Amount -= (slot.EndTime.Hour - slot.StartTime.Hour) * oCourt.Price; // Trừ bớt giá trị đơn đặt cũ đi bằng tiền của 1 trong những ngày đã đặt
					booking.Amount += result.Amount / 1000; // Thêm giá trị của slot mới vào
				}
			}
			// -------------------------------------
			// Update lại dữ liệu số dư của khách trong db
			_service.UserService.UpdateUser(user, userId);
			//----------------------------------------
			// Update Booking trước slot vì để lấy lại giá trị đơn tiền cũ đã đặt của slot cũ
			_service.BookingService.UpdatBooking(booking, booking.BookingId);
			//-----------------------------------------
			// Update lại slot sau khi update booking hoàn tất
			slot.CourtId = courtId;
			slot.StartTime = new DateTime(date.Year, date.Month, date.Day, start, 0, 0);
			slot.EndTime = new DateTime(date.Year, date.Month, date.Day, end, 0, 0);
			_service.SlotService.UpdateSlot(slot, slotId);
			return Ok(new { msg = "Success" });
		}

		
		[HttpGet]
		[Route("Slot/UpdateResultProcessMomo")]
		public async Task<ActionResult> ProcessResultMomo(MoMoRedirectResult result)
		{
			if (result.Message == "Fail")
				return BadRequest(new { msg = "Transaction fail" });

			//---------------------------------------------------------------------
			string userId = result.OrderInfo.Split('|')[1].Trim().Split(',')[1].Trim();
			User user = _service.UserService.GetUserById(userId);
			//------------------------------------------------------------------
			string rawDate = result.OrderInfo.Split('|')[4].Trim().Split(':')[1].Trim();
			DateOnly date = DateOnly.Parse(rawDate.Split(' ')[0]);
			int start = int.Parse(new string(rawDate.Split(' ')[1].Where(char.IsDigit).ToArray()));
			int end = int.Parse(new string(rawDate.Split(' ')[3].Where(char.IsDigit).ToArray()));
			//------------------------------------------------------------------
			string courtId = result.OrderInfo.Split('|')[6].Trim().Split(',')[1].Trim();
			Court court = _service.CourtService.GetCourtByCourtId(courtId);
			//------------------------------------------------------------------
			string bookingId = result.OrderInfo.Split('|')[6].Trim().Split(',')[1].Trim();
			Booking booking = _service.BookingService.GetBookingByBookingId(bookingId);
			//------------------------------------------------------------------
			string slotId = result.OrderInfo.Split('|')[7].Trim().Split(',')[1].Trim();
			BookedSlot slot = _service.SlotService.GetSlotById(slotId);
			Court oCourt = _service.CourtService.GetCourtByCourtId(slot.CourtId);
			//------------------------------------------------------------------
			_service.PaymentService.AddPayment(new Payment { PaymentId = "P" + (_service.PaymentService.GetAllPayments().Count + 1).ToString("D7"), BookingId = booking.BookingId, Date = DateTime.Parse(result.ResponseTime), Method = 1, UserId = userId, TransactionId = result.TransId });
			//------------------------------------------------------------------

			if (result.OrderInfo.Split('|')[8].Trim().Equals("Balance enough")) // Giao dịch update số dư đã cấn ra và còn ít nhất từ 10k 
			{
				user.Balance = 0;  // Do cấn hết số dư vào đơn đặt này nên trả về lại 0
				booking.Amount += double.Parse(result.Amount) / 1000; // Vì cấn toàn bộ số dư sau khi hoàn rồi mới thanh toán nên chỉ cần cộng thêm vào giá trị của giao dịch đã thanh toán từ payment ko quan tâm loại chơi 1 lần hay cố định
			}
			//------------------------------------------------------------------
			// Ko  giao dịch với số tiền nhỏ hơn 10k -> giao dịch full tiền của slot mới mà ko đc cấn từ balance giả định đã hoàn
			else
			{
				user.Balance += (slot.EndTime.Hour - slot.StartTime.Hour) * oCourt.Price; // Hoàn lại số dư slot cũ đã đc hủy
				if (booking.BookingType == 1) // 1 lần chơi -> Thay luôn giá trị đơn cũ thành đơn mới
					booking.Amount = double.Parse(result.Amount) / 1000;
				//-----------------------------------------------------			
				else if (booking.BookingType == 2) // Chơi tháng
				{
					booking.Amount -= (slot.EndTime.Hour - slot.StartTime.Hour) * oCourt.Price; // Trừ bớt giá trị đơn đặt cũ đi bằng tiền của 1 trong những ngày đã đặt
					booking.Amount += double.Parse(result.Amount) / 1000; // Thêm giá trị của slot mới vào
				}
			}
			// -------------------------------------
			// Update lại dữ liệu số dư của khách trong db
			_service.UserService.UpdateUser(user, userId);
			//----------------------------------------
			// Update Booking trước slot vì để lấy lại giá trị đơn tiền cũ đã đặt của slot cũ
			_service.BookingService.UpdatBooking(booking, booking.BookingId);
			//-----------------------------------------
			// Update lại slot sau khi update booking hoàn tất
			slot.CourtId = courtId;
			slot.StartTime = new DateTime(date.Year, date.Month, date.Day, start, 0, 0);
			slot.EndTime = new DateTime(date.Year, date.Month, date.Day, end, 0, 0);
			_service.SlotService.UpdateSlot(slot, slotId);
			return Ok(new { msg = "Success" });
		}


	}
}
