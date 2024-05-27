using DAO.Entities;
using DAO;

namespace Services
{
	public class Services
	{
		public JsonResult Authorize(string username, string password)
		{
			BadmintonCourtContext context = new BadmintonCourtContext();
			//Get the user by username
			var user = context.Users
				.Where<User>(user => user.UserName == username && user.Password == password)
				.FirstOrDefault();

			JsonResult rs = new JsonResult(user != null ? user : new DAO.Entities.User() { UserName = "Not Allowed" });
			return rs;
		}
	}
}
