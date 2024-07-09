using System;
using System.Collections.Generic;

namespace ClassLibrary3.Entities;

public partial class Court
{
    public string CourtId { get; set; } = null!;

    public string? CourtImg { get; set; }

    public string BranchId { get; set; } = null!;

    public float Price { get; set; }

    public string Description { get; set; } = null!;

    public string CourtName { get; set; } = null!;

    public bool CourtStatus { get; set; }

    public virtual ICollection<BookedSlot> BookedSlots { get; set; } = new List<BookedSlot>();

    public virtual CourtBranch Branch { get; set; } = null!;
}
