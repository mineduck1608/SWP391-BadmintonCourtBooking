using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtDAOs
{
	public class SlotDAO
	{
		private readonly BadmintonCourtContext _dbContext = null;

		public SlotDAO()
		{
			if (_dbContext == null)
			{
				_dbContext = new BadmintonCourtContext();
			}
		}

		public SlotDAO(BadmintonCourtContext context)
		{
			_dbContext = context;
		}

		public List<BookedSlot> GetAllSlots() => _dbContext.BookedSlots.ToList();

		public BookedSlot GetSlotById(string id) => _dbContext.BookedSlots.FirstOrDefault(x => x.SlotId == id);

		public List<BookedSlot> GetSlotsByBookingId(string bookingId) => _dbContext.BookedSlots.Where(x => x.BookingId == bookingId).ToList();

		public List<BookedSlot> GetSLotsByDate(DateTime date) => _dbContext.BookedSlots.Where(x => x.StartTime >= new DateTime(date.Year, date.Month, date.Day, 0, 0, 1) && x.EndTime <= new DateTime(date.Year, date.Month, date.Day, 23, 59, 59)).ToList();

		public List<BookedSlot> GetSlotsByCourt(string id) => _dbContext.BookedSlots.Where(x => x.CourtId == id && x.IsDeleted == null).ToList();


		// 2 trường hợp sử dụng:

		// TH1: Tra tình trạng sân trong 1 ngày nào đó
		// KQ1: Trả về 1 list các slot đã đc đặt trong 1 ngày nếu có
		// Note1: Start: 0h 0 phút ngày x | End: 23h39 ngày x

		// TH2: Tra tình trạng sân trong 1 khoảng thời gian cụ thể trong cùng 1 ngày (xài để phục vụ nhu cầu đặt lịch theo tháng - +7 từng cục)
		// KQ2: Trả về 1 list các slot đã đặt trong khoảng thời gian cụ thể cần tìm nhầm tiếp tục +7 nếu khoảng thời gian đó của sân đó đã bị đặt
		// Note2: Start: 15h ngày x | End: 17h ngày x (X ở đây là ngày đầu tiên đặt sân - ngày đầu tiên chơi trong chuỗi 1, 2, 3 tháng | Vd: X là ngày 31/5/2024)
		public List<BookedSlot> GetA_CourtSlotsInTimeInterval(DateTime start, DateTime end, string id) => _dbContext.BookedSlots.Where(x => !(x.StartTime >= end || x.EndTime <= start) && x.CourtId == id && x.IsDeleted == null).ToList();
		//public List<BookedSlot> GetA_CourtSlotsInTimeInterval(DateTime start, DateTime end, string id) => _dbContext.BookedSlots.Where(x => x.StartTime >= start && x.EndTime <= end && x.CourtId == id && x.IsDeleted == null).ToList();
		//!(x.end < start || x.start > end)

		// Trả về danh sách các ngày đặt theo tháng để user confirm r mới đặt
		public List<BookedSlot> GetSlotsByFixedBooking(int monthNum, DateTime start, DateTime end, string id)
		{
			List<BookedSlot> result = new List<BookedSlot>();
			int count = 1;
			while (count <= monthNum * 4)
			{
				// Ở TH này áp dụng GetA_CourtSlotsInDay() cho trường hợp 2!!!
				List<BookedSlot> temporaryList = GetA_CourtSlotsInTimeInterval(start, end, id);
				if (temporaryList.Count == 0)
				{
					//result.Add(new Slot(start, end, true, id, null));
					result.Add(new BookedSlot { StartTime = start, EndTime = end, CourtId = id });
					// Check thử nếu list rỗng tức là hôm đó vào thời khoảng giữa start và end chưa có ai đặt thì cho vào List result để trả về cho user xem để xác nhận sau
					count++;
					// Thêm vào result thành công thì mới tăng count
					// Còn ko thì tiếp tục giữ nguyên count, +7 ngày để tiếp tục check tiếp cho đến khi add thêm đc vào result
				}
				// Thêm vào result fail hay thành công đều +7 để tiếp tục check
				start = start.AddDays(7);
				end = end.AddDays(7);
			}
			return result;
		}

		public void UpdateSlot(BookedSlot newSlot, string id)
		{
			BookedSlot tmp = GetSlotById(id);
			if (tmp != null)
			{
				tmp.StartTime = newSlot.StartTime;
				tmp.EndTime = newSlot.EndTime;
				_dbContext.BookedSlots.Update(tmp);
				_dbContext.SaveChanges();
			}
		}

		public void AddSlot(BookedSlot slot)
		{
			_dbContext.BookedSlots.Add(slot);
			_dbContext.SaveChanges();
		}

		public void DeleteSlot(string id)
		{
			_dbContext.BookedSlots.Remove(GetSlotById(id));
			_dbContext.SaveChanges();
		}

	}
}
