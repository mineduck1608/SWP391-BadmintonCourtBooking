using System;
using System.Collections.Generic;

namespace ClassLibrary2.Entities;

public partial class UserDetail
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
