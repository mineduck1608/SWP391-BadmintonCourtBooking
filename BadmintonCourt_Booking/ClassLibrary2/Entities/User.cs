using System;
using System.Collections.Generic;

namespace ClassLibrary2.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? BranchId { get; set; }

    public int RoleId { get; set; }

    public double? Balance { get; set; }

    public bool? ActiveStatus { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual CourtBranch? Branch { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Role Role { get; set; } = null!;

    public virtual UserDetail? UserDetail { get; set; }
}
