using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class CourtBranch 
{
    public int BranchId { get; set; }

    public string Location { get; set; } = null!;

    public string BranchName { get; set; } = null!;

    public string BranchPhone { get; set; } = null!;

    public string? BranchImg { get; set; }

    public virtual ICollection<Court> Courts { get; set; } = new List<Court>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
