using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class UserDetail
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public UserDetail(int userId, string firstName, string lastName, string email, string phone)
    {
        this.UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
    }
}
