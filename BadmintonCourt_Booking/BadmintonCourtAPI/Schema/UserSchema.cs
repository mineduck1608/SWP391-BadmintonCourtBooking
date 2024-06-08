using BadmintonCourtBusinessObjects.Entities;
namespace BadmintonCourtAPI.Schema
{
	public class UserSchema
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public string Phone { get; set; }

		public string Email { get; set; }

		public UserSchema(string firstName, string lastName, string phone, string email)
		{
			FirstName = firstName;
			LastName = lastName;
			Phone = phone;
			Email = email;
		}
	}
}
