using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class Court 
{
    public int CourtId { get; set; }

    public string? CourtImg { get; set; }

    public int BranchId { get; set; }

    public double Price { get; set; }

    public string Description { get; set; } = null!;

    public virtual CourtBranch Branch { get; set; } = null!;

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    public Court(int courtId, string? courtImg, int branchId, double price, string description)
    {
        this.CourtId = courtId;
        CourtImg = courtImg;
        BranchId = branchId;
        Price = price;
        Description = description;
    }
}
