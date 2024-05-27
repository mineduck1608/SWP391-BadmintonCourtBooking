using System;
using System.Collections.Generic;

namespace DAO.Entities;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int Rating { get; set; }

    public string Content { get; set; } = null!;

    public int? UserId { get; set; }

    public int? BranchId { get; set; }

    public virtual CourtBranch? Branch { get; set; }

    public virtual User? User { get; set; }
}
