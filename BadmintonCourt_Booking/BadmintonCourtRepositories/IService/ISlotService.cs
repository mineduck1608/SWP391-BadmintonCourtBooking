using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
    public interface ISlotService
    {
        public List<BookedSlot> GetAllSlots();

        public BookedSlot GetSlotById(string id);

        public List<BookedSlot> GetSlotsByBookingId(string bookingId);

		public List<BookedSlot> GetSLotsByDate(DateTime date);

        public List<BookedSlot> GetA_CourtSlotsInTimeInterval(DateTime start, DateTime end, string id);

        public List<BookedSlot> GetSlotsByFixedBooking(int monthNum, DateTime start, DateTime end, string id);

        public List<BookedSlot> GetSlotsByCourt(string id);

        public void UpdateSlot(BookedSlot newSlot, string id);

        public void AddSlot(BookedSlot slot);

        public void DeleteSlot(string id);

    }
}
