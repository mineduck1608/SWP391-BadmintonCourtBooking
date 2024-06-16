using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BadmintonCourtAPI.Utils;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using Microsoft.EntityFrameworkCore;
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

 
        [HttpPost]
		[Route("Slot/BookingByBalence")]
		//[Authorize]
		public async Task<IActionResult> AddBookedSLot(DateOnly date, int start, int end, string courtId, string userId)
		{
			User user = _service.UserService.GetUserById(userId);
			DateTime startDate = new DateTime(date.Year, date.Month, date.Day, start, 0, 0);
			DateTime endDate = new DateTime(date.Year, date.Month, date.Day, end, 0, 0);
			List<Slot> tmpStorage = _service.SlotService.GetA_CourtSlotsInDay(startDate, endDate, courtId);
			if (tmpStorage == null || tmpStorage.Count == 0)
			{
				if (user.Balance >= (end - start))
				{
					_service.BookingService.AddBooking(new Booking { BookingId = "BK" + (_service.BookingService.GetAllBookings().Count + 1).ToString("D7"), Amount = (end - start) * _service.CourtService.GetCourtByCourtId("S1").Price, BookingType = 1, UserId = userId });
					_service.SlotService.AddSlot(new Slot { SlotId = "S" + (_service.SlotService.GetAllSlots().Count + 1).ToString("D7"), BookingId = _service.BookingService.GetRecentAddedBooking().BookingId, CourtId = courtId, Status = false, StartTime = startDate, EndTime = endDate });
					user.Balance -= (end - start);
					_service.UserService.UpdateUser(user, userId);
					return Ok("Success");
				}
				return Ok("Balance not enough");
			}
			return Ok("This slot has been booked");
		
		}

        [HttpPost]
        [Route("GetSLotCourtInDay")]
        public async Task<IEnumerable<Slot>> GetSLotCourtInDay([FromBody] SlotRequest request)
        {
            DateTime startDate = new DateTime(request.Date.Year, request.Date.Month, request.Date.Day, 0, 0, 0);
            DateTime endDate = new DateTime(request.Date.Year, request.Date.Month, request.Date.Day, 23, 59, 0);

            return await _service.SlotService.GetA_CourtSlotsInDay(startDate, endDate, request.Id);
        }


        [HttpGet]
		[Route("Slot/GetBeforeConfirm")]
		public async Task<ActionResult<IEnumerable<Slot>>> GetSlotsByFixedBookingBeforeConfirm(int monthNum, DateTime date, int start, int end, string id) => Ok(_service.SlotService.GetSlotsByFixedBooking(monthNum, new DateTime(date.Year, date.Month, date.Day, start, 0, 0), new DateTime(date.Year, date.Month, date.Day, end, 0, 0), id).ToList());


		// Update thời gian hoạt động của sân đồng thời update toàn bộ các sân
		// Bắt đầu từ Slot gốc - slot đầu tiên có id là 1

		
        [HttpPut]
		[Route("Slot/UpdateOfficeHours")]
        //[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateOfficeHours(int start, int end)
        {
			if (start >= end)
				return BadRequest(new { msg = "End must be bigger than start" });
			Slot slot = _service.SlotService.GetSlotById("S1");
			slot.StartTime = Util.CustomizeDate(start);
			slot.EndTime = Util.CustomizeDate(end);
			_service.SlotService.UpdateSlot(slot, "S1");
			return Ok(new { msg = "Success" });
        }

		[HttpPut]
		[Route("Slot/Update")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateSlot(DateOnly date, int start, int end, string slotId, string? bookingId, string courtId, bool status)
		{
			Slot primitiveSlot = _service.SlotService.GetSlotById("S1");
			if (!bookingId.IsNullOrEmpty()) // Nếu slot có khách đặt
				if (status) // Tình trạng sân lại trống
					return BadRequest(new { msg = "If court is booked, the status can't be false" });
			if (start >= end || start < primitiveSlot.StartTime.Hour || end > primitiveSlot.EndTime.Hour)
				return BadRequest(new { msg = "Invalid time"});
			

			// Check trường hợp sân đấy giờ đấy đang tạm bảo trì đổi thời gian + sân khác cho khách
			Slot tmp = _service.SlotService.GetAllSlots().FirstOrDefault(x => x.CourtId == courtId && x.StartTime == new DateTime(date.Year, date.Month, date.Day, start, 0, 0) && x.EndTime == new DateTime(date.Year, date.Month, date.Day, end, 0, 0) && x.Status == false && x.BookingId != null);
			if (tmp != null)  // Trường hợp sân đấy giờ đấy đã có khách khác đặt
				return BadRequest(new { msg = $"Court {courtId} from {start}h - {end}h {date} has been booked"});

			Slot slot = _service.SlotService.GetSlotById(slotId);
			slot.StartTime = new DateTime(date.Year, date.Month, date.Day, start, 0, 0);
			slot.EndTime = new DateTime(date.Year, date.Month, date.Day, end, 0, 0);
			slot.Status = status;
			slot.BookingId = bookingId;
			slot.CourtId = courtId;
			_service.SlotService.UpdateSlot(slot, slotId);
			return Ok(new { msg = "Success" });
		}

		//[HttpDelete]
		//[Route("Slot/Delete")]
  //      [Authorize(Roles = "Admin")]
		//public async Task<IActionResult> DeleteSlot(string id)
  //      {
  //          service.slotService.DeleteSlot(id);
  //          return Ok();
  //      }
    }

    public class SlotRequest
    {
        public DateTime Date { get; set; }
        public string Id { get; set; }
    }

}
