﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.VnPay
{
	public class VnPayResponseDTO
	{
		public bool Status { get; set; }
		public string PaymentMethod { get; set; }
		public string Description { get; set; }
		public string BookingId { get; set; }
		public string PaymentId { get; set; }
		public string TransactionId { get; set; }
		public string Token { get; set; }
		public string VnPayResponseCode { get; set; }
		public string DayList { get; set; }
		public string Interval { get; set; }
		public double Amount { get; set;  }
		public DateTime Date { get; set; }

	}
}
