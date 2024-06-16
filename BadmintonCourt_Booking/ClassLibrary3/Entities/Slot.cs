using System;
using System.Collections.Generic;

namespace ClassLibrary3.Entities;

public partial class Slot
{
    public string SlotId { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public bool Status { get; set; }

    public string CourtId { get; set; } = null!;

    public string? BookingId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Court Court { get; set; } = null!;
}
