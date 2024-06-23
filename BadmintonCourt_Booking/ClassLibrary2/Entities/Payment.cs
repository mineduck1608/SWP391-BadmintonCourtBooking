using System;
using System.Collections.Generic;

namespace ClassLibrary2.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int Status { get; set; }

    public int UserId { get; set; }

    public DateOnly Date { get; set; }

    public int BookingId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
