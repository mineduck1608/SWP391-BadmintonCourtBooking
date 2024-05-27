using System;
using System.Collections.Generic;

namespace DAO.Entities;

public partial class Court
{
    public int CourtId { get; set; }

    public int CourtNumber { get; set; }

    public double Price { get; set; }

    public string Description { get; set; } = null!;

    public int? BranchId { get; set; }

    public string CourtImg { get; set; } = null!;

    public virtual CourtBranch? Branch { get; set; }

    public virtual ICollection<CourtActiveSlot> CourtActiveSlots { get; set; } = new List<CourtActiveSlot>();
}
