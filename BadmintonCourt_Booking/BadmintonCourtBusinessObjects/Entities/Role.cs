using System;
using System.Collections.Generic;

namespace BadmintonCourtBusinessObjects.Entities;

public partial class Role 
{
	public string RoleId { get; set; }

	public string RoleName { get; set; }

	public virtual ICollection<User> Users { get; set; } = new List<User>();

}
