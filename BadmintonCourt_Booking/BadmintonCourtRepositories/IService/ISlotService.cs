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
        public List<Slot> GetAllSlots();

        public Slot GetSlotById(int id);

        public List<Slot> GetSlotsByStatus(bool status);

        public List<Slot> GetSLotsByDate(DateTime date);

        public List<Slot> GetA_CourtSlotsInDay(DateTime start, DateTime end, int id);

        public List<Slot> GetSlotsByFixedBooking(int monthNum, DateTime start, DateTime end, int id);

        public List<Slot> GetSlotsByCourt(int id);

        public void UpdateSlot(Slot newSlot, int id);

        public void AddSlot(Slot slot);

        public void DeleteSlot(int id);

    }
}
