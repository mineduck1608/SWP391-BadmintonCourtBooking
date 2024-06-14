using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BadmintonCourtAPI.Utils;
namespace BadmintonCourtAPI.Controllers
{
    public class SlotController : Controller
    {
        private readonly BadmintonCourtService service;
        public SlotController(IConfiguration config)
        {
            if (service == null)
            {
                service = new BadmintonCourtService(config);
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
		[Route("Slot/Booking")]
		//[Authorize]
		public async Task<IActionResult> AddBookedSLot(DateTime date, int start, int end, string courtId, string userId)
		{
			User user = service.userService.GetUserById(userId);
			DateTime startDate = new DateTime(date.Year, date.Month, date.Day, start, 0, 0);
			DateTime endDate = new DateTime(date.Year, date.Month, date.Day, end, 0, 0);
			List<Slot> tmpStorage = service.slotService.GetA_CourtSlotsInDay(startDate, endDate, courtId);
			if (tmpStorage == null || tmpStorage.Count == 0)
			{
				if (user.Balance > (end - start))
				{
					service.bookingService.AddBooking(new Booking { BookingId = Util.GenerateBookingId(service), Amount = (end - start) * service.courtService.GetCourtByCourtId("").Price, BookingType = 1, UserId = userId });
					service.slotService.AddSlot(new Slot { SlotId = Util.GenerateSlotId(service), BookingId = service.bookingService.GetRecentAddedBooking().BookingId, CourtId = courtId, Status = false, StartTime = startDate, EndTime = endDate });
					user.Balance -= (end - start);
					service.userService.UpdateUser(user, userId);
					return Ok("Success");
				}
				return Ok("Balance not enough");
			}
			return Ok("This slot has been booked");
		
		}

		[HttpGet]
		[Route("Slot/GetSLotCourtInDay")]
		public async Task<IEnumerable<Slot>> GetA_CourtSlotsInDay(DateTime date, string id) => service.slotService.GetA_CourtSlotsInDay(new DateTime(date.Year, date.Month, date.Day, 0, 0, 0), new DateTime(date.Year, date.Month, date.Day, 23, 59, 0), id).ToList();


		[HttpGet]
		[Route("Slot/GetBeforeConfirm")]
		public async Task<ActionResult<IEnumerable<Slot>>> GetSlotsByFixedBookingBeforeConfirm(int monthNum, DateTime date, int start, int end, string id) => Ok(service.slotService.GetSlotsByFixedBooking(monthNum, new DateTime(date.Year, date.Month, date.Day, start, 0, 0), new DateTime(date.Year, date.Month, date.Day, end, 0, 0), id).ToList());


		// Update thời gian hoạt động của sân đồng thời update toàn bộ các sân
		// Bắt đầu từ Slot gốc - slot đầu tiên có id là 1

		
        [HttpPut]
		[Route("Slot/UpdateOfficeHours")]
        //[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateOfficeHours(int start, int end)
        {
            int oldStart = service.slotService.GetSlotById("S00000001").StartTime.Hour;
            int oldEnd = service.slotService.GetSlotById("S00000001").EndTime.Hour;
            List<Slot> storage = service.slotService.GetAllSlots().Where(x => x.StartTime.Hour == oldStart && x.EndTime.Hour == oldEnd).ToList();
            foreach (Slot slot in storage)
            {
                slot.StartTime = Util.CustomizeDate(start);
                slot.EndTime = Util.CustomizeDate(end);
				service.slotService.UpdateSlot(slot, slot.SlotId);
            }
            return Ok();
        }


		[HttpDelete]
		[Route("Slot/Delete")]
        //[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteSlot(string id)
        {
            service.slotService.DeleteSlot(id);
            return Ok();
        }
    }
}
