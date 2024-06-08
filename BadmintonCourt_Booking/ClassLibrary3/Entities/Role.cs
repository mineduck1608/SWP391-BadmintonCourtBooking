using System;
using System.Collections.Generic;

namespace ClassLibrary3.Entities;

public partial class Role
{
    public int RoleId { get; set; }

    public string Role1 { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
