using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class UserDetail
{
    public string UserId { get; set; }

    public string? FirstName { get; set; } = null!;

    public string? LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; } = null!;

    public string? Facebook { get; set; } = null!;

    public virtual User User { get; set; } = null!;

}
