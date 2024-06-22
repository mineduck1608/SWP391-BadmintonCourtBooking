using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtBusinessObjects.SupportEntities.Payment
{
	public class TransactionDTO
	{
		public int Method {  get; set; } // Momo, vnpay, ...
		public int? Start { get; set; }  // 3h
		public int? End { get; set; }  // 5h
		public string? UserId { get; set; }
		public string Type { get; set; }
		public DateTime? Date { get; set; }  // 21/5/2024
		public string? CourtId { get; set; }
		public int? NumMonth { get; set; }  // 2 tháng, 3 tháng
		public float? Amount { get; set; }   // 10k 20k 50k		
	}
}
