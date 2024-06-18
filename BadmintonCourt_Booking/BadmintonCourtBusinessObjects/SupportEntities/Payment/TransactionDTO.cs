using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtBusinessObjects.SupportEntities.Payment
{
	public class TransactionDTO
	{
		public string Method {  get; set; } // Momo, vnpay, ...
		public string Start { get; set; }  // 3h
		public string End { get; set; }  // 5h
		public string UserId { get; set; }

		public DateOnly Date { get; set; }  // 21/5/2024
		public string CourtId { get; set; }
		public string Type { get; set; } // Cố định, linh hoạt
		public int NumMonth { get; set; }  // 2 tháng, 3 tháng
		public string Amount { get; set; }   // 10k 20k 50k
		public string DaysList { get; set; } // Giành cho chơi cố định, request gửi về list các ngày đã đc accept để tạo ngầm slot
		// Nếu giao dịch ko thành công sẽ tự động hủy slot đã tạo ngầm trong db
		
	}
}
