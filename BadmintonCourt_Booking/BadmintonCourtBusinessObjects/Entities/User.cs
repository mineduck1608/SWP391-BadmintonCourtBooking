using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? BranchId { get; set; }

    public int RoleId { get; set; }

    public double? Balance { get; set; }

    public bool ActiveStatus { get; set; }

	public int? AccessFail { get; set; }

	public DateTime? LastFail { get; set; }

	public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual CourtBranch? Branch { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Role Role { get; set; } = null!;

    public virtual UserDetail? UserDetail { get; set; }

	public User(string userName, string password, int? branchId, int roleId, double? balance, bool activeStatus, int? accessFail, DateTime? lastFail)
	{
		UserName = userName;
		Password = password;
		BranchId = branchId;
		RoleId = roleId;
		Balance = balance;
		ActiveStatus = activeStatus;
		AccessFail = accessFail;
		LastFail = lastFail;
	}

	public User(string userName, string password, int? branchId, int roleId, double? balance, bool activeStatus, int? accessFail, DateTime? lastFail, UserDetail userDetail)
	{
		UserName = userName;
		Password = password;
		BranchId = branchId;
		RoleId = roleId;
		Balance = balance;
		ActiveStatus = activeStatus;
		AccessFail = accessFail;
		LastFail = lastFail;
		UserDetail = userDetail;
	}
}
