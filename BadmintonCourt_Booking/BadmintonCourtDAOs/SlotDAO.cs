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

        public List<Slot> GetAllSlots() => _dbContext.Slots.ToList();

        public Slot GetSlotById(int id) => _dbContext.Slots.FirstOrDefault(x => x.SlotId == id);

        public List<Slot> GetSlotsByStatus(bool status) => _dbContext.Slots.Where(x => x.Status == status).ToList();

        public List<Slot> GetSLotsByDate(DateTime date) => _dbContext.Slots.Where(x => x.StartTime <= date &&  x.EndTime <= date).ToList();

        public List<Slot> GetSlotsByCourt(int id) => _dbContext.Slots.Where(x => x.CourtId == id).ToList(); 


        // 2 trường hợp sử dụng:

        // TH1: Tra tình trạng sân trong 1 ngày nào đó
        // KQ1: Trả về 1 list các slot đã đc đặt trong 1 ngày nếu có
        // Note1: Start: 0h 0 phút ngày x | End: 23h39 ngày x
        
        // TH2: Tra tình trạng sân trong 1 khoảng thời gian cụ thể trong cùng 1 ngày (xài để phục vụ nhu cầu đặt lịch theo tháng - +7 từng cục)
        // KQ2: Trả về 1 list các slot đã đặt trong khoảng thời gian cụ thể cần tìm nhầm tiếp tục +7 nếu khoảng thời gian đó của sân đó đã bị đặt
        // Note2: Start: 15h ngày x | End: 17h ngày x (X ở đây là ngày đầu tiên đặt sân - ngày đầu tiên chơi trong chuỗi 1, 2, 3 tháng | Vd: X là ngày 31/5/2024)
        public List<Slot> GetA_CourtSlotsInDay(DateTime start, DateTime end, int id) => _dbContext.Slots.Where(x => start >= x.StartTime && end <= x.EndTime && x.CourtId == id && x.Status == false && x.BookingId != null).ToList();


        // Trả về danh sách các ngày đặt theo tháng để user confirm r mới đặt
        public List<Slot> GetSlotsByFixedBooking(int monthNum, DateTime start, DateTime end, int id)
        {
            List<Slot> result = new List<Slot>();
            int count = 0;
            while (count <= monthNum*4)
            {
                // Ở TH này áp dụng GetA_CourtSlotsInDay() cho trường hợp 2!!!
                List<Slot> temporaryList = GetA_CourtSlotsInDay(start, end, id);
                if (temporaryList == null || temporaryList.Count == 0)
                {
                    result.Add(new Slot(start, end, true, id, null));
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

        public void UpdateSlot(Slot newSlot, int id)
        {
            Slot tmp = GetSlotById(id);
            if (tmp != null)
            {
                tmp.StartTime = newSlot.StartTime;
                tmp.EndTime = newSlot.EndTime;
                tmp.Status = newSlot.Status;
                _dbContext.Slots.Update(tmp);
                _dbContext.SaveChanges();
            }
        }

        public void AddSlot(Slot slot)
        {
            _dbContext.Slots.Add(slot);
            _dbContext.SaveChanges();
        }

        public void DeleteSlot(int id)
        {
            _dbContext.Slots.Remove(GetSlotById(id));
            _dbContext.SaveChanges();
        }

    }
}
