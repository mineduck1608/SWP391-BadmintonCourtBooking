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

        private readonly SlotDAO slotDAO = null;

        public SlotService()
        {
            if (slotDAO == null)
            {
                slotDAO = new SlotDAO();
            }
        }

        public void AddSlot(Slot slot) => slotDAO.AddSlot(slot);

        public void DeleteSlot(int id) => slotDAO.DeleteSlot(id);

        public List<Slot> GetAllSlots() => slotDAO.GetAllSlots();

        public Slot GetSlotById(int id) => slotDAO.GetSlotById(id);

        public List<Slot> GetSLotsByDate(DateTime date) => slotDAO.GetSLotsByDate(date);

        public List<Slot> GetSlotsByStatus(bool status) => slotDAO.GetSlotsByStatus(status);

        public List<Slot> GetA_CourtSlotsInDay(DateTime start, DateTime end, int id) => slotDAO.GetA_CourtSlotsInDay(start, end, id);

        public List<Slot> GetSlotsByFixedBooking(int monthNum, DateTime start, DateTime end, int id) => slotDAO.GetSlotsByFixedBooking(monthNum, start, end, id);

        public List<Slot> GetSlotsByCourt(int id) => slotDAO.GetSlotsByCourt(id);

        public void UpdateSlot(Slot newSlot, int id) => slotDAO.UpdateSlot(newSlot, id);
    }
}
