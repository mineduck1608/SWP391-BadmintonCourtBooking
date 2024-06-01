using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Mvc;

namespace BadmintonCourtAPI.Controllers
{
    public class SlotController : Controller
    {
        private readonly BadmintonCourtService service;

        public SlotController()
        {
            if (service == null)
            {
                service = new BadmintonCourtService();
            }
        }

        public DateTime CustomizeDate(int period) => new DateTime(1, 1, 1, period, 0, 0);


		[HttpPost]
		[Route("Slot/AddDefault")]
		public async Task<IActionResult> AddDefaultSlot(Slot slot)
        {
            service.slotService.AddSlot(slot);
            return Ok();
        }


		[HttpPost]
		[Route("Slot/AddBooked")]
		public async Task<IActionResult> AddBookedSLot(DateTime date, int start, int end, int bookingId, int courtId, int id)
        {
			service.slotService.AddSlot(new Slot(1, new DateTime(date.Year, date.Month, date.Day, start, 0, 0), new DateTime(date.Year, date.Month, date.Day, end, 0, 0), false, courtId, bookingId));
            return Ok();
        }


		[HttpGet]
		[Route("Slot/GetSLotCourtInDay")]
		public async Task<IEnumerable<Slot>> GetA_CourtSlotsInDay(DateTime date, int id) => service.slotService.GetA_CourtSlotsInDay(new DateTime(date.Year, date.Month, date.Day, 0, 0, 0), new DateTime(date.Year, date.Month, date.Day, 23, 59, 0), id).ToList();


		[HttpGet]
		[Route("Slot/GetBeforeConfirm")]
		public async Task<IEnumerable<Slot>> GetSlotsByFixedBookingBeforeConfirm(int monthNum, DateTime date, int start, int end, int id) => service.slotService.GetSlotsByFixedBooking(monthNum, new DateTime(date.Year, date.Month, date.Day, start, 0, 0), new DateTime(date.Year, date.Month, date.Day, end, 0, 0), id).ToList();


		// Update thời gian hoạt động của sân đồng thời update toàn bộ các sân
		// Bắt đầu từ Slot gốc - slot đầu tiên có id là 1

		
        [HttpPut]
		[Route("Slot/UpdateOfficeHours")]
		public async Task<IActionResult> UpdateOfficeHours(int start, int end)
        {
            int oldStart = service.slotService.GetSlotById(1).StartTime.Hour;
            int oldEnd = service.slotService.GetSlotById(1).EndTime.Hour;
            List<Slot> storage = service.slotService.GetAllSlots().Where(x => x.StartTime.Hour == oldStart && x.EndTime.Hour == oldEnd).ToList();
            foreach (Slot slot in storage)
            {
                slot.StartTime = CustomizeDate(start);
                slot.EndTime = CustomizeDate(end);
                service.slotService.UpdateSlot(slot, slot.SlotId);
            }
            return Ok();
        }


		[HttpDelete]
		[Route("Slot/Delete")]
		public async Task<IActionResult> DeleteSlot(int id)
        {
            service.slotService.DeleteSlot(id);
            return Ok();
        }
    }
}
