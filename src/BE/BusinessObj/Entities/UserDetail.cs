using System;
using System.Collections.Generic;

namespace DAO.Entities;

public partial class UserDetail
{
    public int UserId { get; set; }

    public string Firstname { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
