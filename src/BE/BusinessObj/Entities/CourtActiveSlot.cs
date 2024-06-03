using System;
using System.Collections.Generic;

namespace DAO.Entities;

public partial class CourtActiveSlot
{
    public int CourtId { get; set; }

    public int SlotId { get; set; }

    public virtual Court Court { get; set; } = null!;

    public virtual Slot Slot { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
