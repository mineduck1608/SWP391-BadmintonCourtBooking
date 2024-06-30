using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class Payment
{
    public string PaymentId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateTime Date { get; set; }

    public string? BookingId { get; set; }

    public int Method { get; set; }

    public double Amount { get; set; }

    public string TransactionId { get; set; } = null!;

    public virtual Booking? Booking { get; set; }


	public virtual User User { get; set; } = null!;
}
