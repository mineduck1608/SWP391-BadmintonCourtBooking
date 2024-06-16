using System;
using System.Collections.Generic;

namespace ClassLibrary3.Entities;

public partial class Feedback
{
    public string FeedbackId { get; set; } = null!;

    public int Rating { get; set; }

    public string Content { get; set; } = null!;

    public string? UserId { get; set; }

    public string BranchId { get; set; } = null!;

    public virtual CourtBranch Branch { get; set; } = null!;

    public virtual User? User { get; set; }
}
