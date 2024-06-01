using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class Payment 
{
    public int PaymentId { get; set; }

    public bool Status { get; set; }

    public DateTime? Date { get; set; }

    public int UserId { get; set; }

    public int BookingId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
