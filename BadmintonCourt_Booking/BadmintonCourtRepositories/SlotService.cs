using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtDAOs;
using BadmintonCourtServices.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices
{
    public class SlotService : ISlotService
    {

        private readonly SlotDAO _slotDAO = null;

        public SlotService()
        {
            if (_slotDAO == null)
            {
                _slotDAO = new SlotDAO();
            }
        }

        public void AddSlot(BookedSlot slot) => _slotDAO.AddSlot(slot);

        public void DeleteSlot(string id) => _slotDAO.DeleteSlot(id);

        public List<BookedSlot> GetAllSlots() => _slotDAO.GetAllSlots();

		public List<BookedSlot> GetSlotsByBookingId(string bookingId) => _slotDAO.GetSlotsByBookingId(bookingId);


		public BookedSlot GetSlotById(string id) => _slotDAO.GetSlotById(id);

        public List<BookedSlot> GetSLotsByDate(DateTime date) => _slotDAO.GetSLotsByDate(date);

        public List<BookedSlot> GetA_CourtSlotsInTimeInterval(DateTime start, DateTime end, string id) => _slotDAO.GetA_CourtSlotsInTimeInterval(start, end, id);

        public List<BookedSlot> GetSlotsByFixedBooking(int monthNum, DateTime start, DateTime end, string id) => _slotDAO.GetSlotsByFixedBooking(monthNum, start, end, id);

        public List<BookedSlot> GetSlotsByCourt(string id) => _slotDAO.GetSlotsByCourt(id);

        public void UpdateSlot(BookedSlot newSlot, string id) => _slotDAO.UpdateSlot(newSlot, id);
    }
}
