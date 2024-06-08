using System;
using System.Collections.Generic;

namespace ClassLibrary3.Entities;

public partial class Slot
{
    public int SlotId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool Status { get; set; }

    public int CourtId { get; set; }

    public int? BookingId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Court Court { get; set; } = null!;
}
