using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.VnPay
{
	public class VnPayRequestDTO
	{
		public int UserId { get; set; }
		public string Content { get; set; }
		public double Amount { get; set; }
		public string OrderId { get; set; }
		public string Type { get; set; }
		public string DaysList { get; set; }
		public string Interval { get; set; }
		public DateTime Date { get; set; }
	}
}
