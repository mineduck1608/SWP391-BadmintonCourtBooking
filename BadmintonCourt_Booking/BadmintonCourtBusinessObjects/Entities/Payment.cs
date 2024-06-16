using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class Payment 
{
	public string PaymentId { get; set; }

	public string UserId { get; set; }

	public DateTime Date { get; set; }

	public string Method { get; set; }

	public string TransactionId { get; set; }

	public string BookingId { get; set; }
	public double Amount { get; set; }

	public virtual Booking Booking { get; set; }

	public virtual User User { get; set; }
}
