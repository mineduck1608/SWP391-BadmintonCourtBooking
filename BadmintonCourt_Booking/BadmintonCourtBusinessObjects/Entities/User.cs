using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? BranchId { get; set; }

    public string? RoleId { get; set; } = null!;

    public double? Balance { get; set; }

    public int? AccessFail { get; set; }
    public string? Token {  get; set; }
    public DateTime? ActionPeriod { get; set; }

    public DateTime? LastFail { get; set; }

    public bool? ActiveStatus { get; set; }
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual CourtBranch? Branch { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Role Role { get; set; } = null!;

    public virtual UserDetail? UserDetail { get; set; }

}
