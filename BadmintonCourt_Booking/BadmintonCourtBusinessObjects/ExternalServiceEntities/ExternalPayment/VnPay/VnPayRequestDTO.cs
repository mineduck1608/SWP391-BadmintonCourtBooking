using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.VnPay
{
	public class VnPayRequestDTO
	{
		public string UserId { get; set; }
		public string Content { get; set; }
		public float? Amount { get; set; }
		public string OrderId { get; set; }
		public DateTime Date { get; set; }
	}
}
