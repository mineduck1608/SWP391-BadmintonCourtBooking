using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtBusinessObjects.SupportEntities.Account
{
	public class ProvideAccount
	{
		public string? UserName { get; set; }
		public string? Password { get; set; }
		public string? BranchId { get; set; }
		public string RoleId { get; set; }
		public double? Balance { get; set; }
		public string Email {  get; set; }
		public string? Phone {  get; set; }
		public string? Facebook { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Img { get; set;}
	}
}
