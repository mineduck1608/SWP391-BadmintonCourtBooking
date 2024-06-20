using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtDAOs;
using BadmintonCourtServices.IService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		/// <summary>
		/// Attempt to add a slot until it can be done
		/// </summary>
		/// <param name="slot"></param>
		public void AttemptToAdd(BookedSlot slot)
		{
			//The original datetime
			DateTime startOri = slot.StartTime;
			DateTime endOri = slot.EndTime;
			//The one that will be used for probing
			DateTime start = slot.StartTime;
			DateTime end = slot.EndTime;
			int length = end.Hour - start.Hour;
			bool success = false;
			do
			{
				Debug.WriteLine($"{success}, {start}, {end}");
				var activeInInterval = _slotDAO.GetSlotsActiveInInterval(start, end, slot.CourtId);
				if (activeInInterval?.Count > 0) // Some slots are in this range
				{
					success = false;
					//Probe
					start = start.AddHours(length / 2);
					end = end.AddHours(length / 2);
				}
				else
				{
					success = true; //Probably is suitable
					//!!!!!!!!!!
					if ((start.Hour <= 7 || end.Hour >= 22) || (start.Hour >= 22 || end.Hour <= 7))
					{
						success = false;
						start = startOri;
						end = endOri;
					}
				}
			}
			while (!success);
			//Add that slot
			slot.StartTime = start;
			slot.EndTime = end;
			_slotDAO.AddSlot(slot);
		}
	}
}
