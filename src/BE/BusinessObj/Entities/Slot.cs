using System;
using System.Collections.Generic;

namespace DAO.Entities;

public partial class Slot
{
    public int SlotId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int Status { get; set; }

    public virtual ICollection<CourtActiveSlot> CourtActiveSlots { get; set; } = new List<CourtActiveSlot>();
}
