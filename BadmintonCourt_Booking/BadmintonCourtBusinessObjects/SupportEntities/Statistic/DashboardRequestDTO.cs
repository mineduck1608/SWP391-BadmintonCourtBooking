using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtBusinessObjects.SupportEntities.Statistic
{
	public class DashboardRequestDTO
	{
		public int? Year { get; set; } // 2024, 2023, ...
		public int? Type { get; set; } // Năm, tháng, tuần
		public int? StartMonth { get; set; } // Tháng bắt đầu 
		public int? MonthNum { get; set; } // Số lượng tháng, max 3
		public int? Week { get; set; } // Tuần của tháng nếu chọn lại tuần, giá trị: 1 - 4


	}
}
