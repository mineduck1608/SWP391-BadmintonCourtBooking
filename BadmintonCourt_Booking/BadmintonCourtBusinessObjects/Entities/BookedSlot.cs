using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class BookedSlot
{
    public string? SlotId { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string CourtId { get; set; } = null!;

    public string BookingId { get; set; } = null!;
    public bool? IsDeleted { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Court Court { get; set; } = null!;
}
