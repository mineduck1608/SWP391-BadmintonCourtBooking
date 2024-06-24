using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using BadmintonCourtAPI.Utils;
using BadmintonCourtBusinessObjects.SupportEntities.Account;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.CodeAnalysis.Emit;

namespace BadmintonCourtAPI.Controllers
{
	public class UserController : Controller
	{
		private readonly BadmintonCourtService _service = null;
		private readonly IConfiguration _config;

		public UserController(IConfiguration config)
		{
			if (_service == null)
			{
				_service = new BadmintonCourtService(_config);
			}
			_config = config;
		}

		//[HttpGet]
		//[Route("User/GetAll")]
		////[Authorize]
		//public async Task<ActionResult<IEnumerable<UserDetail>>> GetAllUsers() => Ok(_service.UserDetailService.GetAllUserDetails());

		[HttpGet]
		[Route("User/GetAll")]
		//[Authorize]
		public async Task<ActionResult<IEnumerable<User>>> GetAllUsers() => Ok(_service.UserService.GetAllUsers());

		[HttpGet]
		[Route("User/GetStaffsInBranch")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<User>>> GetStaffsByBranch(string id) => Ok(_service.UserService.GetStaffsByBranch(id).ToList());

		[HttpGet]
		[Route("User/GetByRole")]
		//[Authorize(Roles = "Admin")]
		//[Authorize(Roles = "Staff")]
		public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(string id) => Ok(_service.UserService.GetUsersByRole(id));

		[HttpGet]
		[Route("User/GetById")]
		//[Authorize(Roles = "Admin")]
		//[Authorize(Roles = "Staff")]
		public async Task<ActionResult<User>> GetUserByUserId(string id) => Ok(_service.UserService.GetUserById(id));


		//[HttpPost]
		//[Route("User/ExternalLogAuth")]
		//public async Task<IActionResult> ExternalAuth(string token)
		//{
		//	string email = Util.GetMailFromToken(token);
		//	UserDetail info = service.userDetailService.GetUserDetailByMail(email);

		//	if (info == null) // Chua co acc
		//	{

		//		service.userService.AddUser(new User { UserId = Util.GenerateUserId(service), UserName = "", Password = "", LastFail = new DateTime(1900, 1, 1, 0, 0, 0), ActiveStatus = true, Balance = 0, AccessFail = 0, BranchId = null});
		//		string id = service.userService.GetRecentAddedUser().UserId; 

		//		service.userDetailService.AddUserDetail(new UserDetail { UserId = id, Email = email, FirstName = "", LastName = "", Phone = ""} );
		//		return Ok(new { token = Util.GenerateToken(id, "", "", "Customer", _config) });
		//	}

		//	// Co acc
		//	User user = service.userService.GetUserById(info.UserId);
		//	return Ok(new { token = Util.GenerateToken(info.UserId, info.LastName, user.UserName, service.roleService.GetRoleById(user.RoleId).RoleName, _config) });
		//}


		[HttpPost]
		[Route("User/ExternalLogAuth")]
		public async Task<IActionResult> ExternalAuth(string email)
		{
			UserDetail info = _service.UserDetailService.GetUserDetailByMail(email);

			if (info == null) // Chua co acc
			{
				List<User> list = _service.UserService.GetAllUsers();
				_service.UserService.AddUser(new User { UserId = "S" + (_service.UserService.GetAllUsers().Count + 1).ToString("D7"), LastFail = new DateTime(1900, 1, 1, 0, 0, 0), ActiveStatus = true, Balance = 0, AccessFail = 0, BranchId = null, RoleId = "R003" });
				string id = _service.UserService.GetRecentAddedUser().UserId;

				_service.UserDetailService.AddUserDetail(new UserDetail { UserId = id, Email = email });
				return Ok(new { token = Util.GenerateToken(id, "", "", "Customer", _config) });
			}

			// Co acc
			User user = _service.UserService.GetUserById(info.UserId);
			return Ok(new { token = Util.GenerateToken(info.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config) });
		}

		[HttpPost]
		[Route("User/LoginAuth")]
		public async Task<IActionResult> LoginAuth(string username, string password)
		{
			//User user = service.userService.GetUserByLogin(username, password);
			User user = _service.UserService.GetAllUsers().FirstOrDefault(x => x.UserName == username && x.Password == password);

			// Nhập đúng username + pass
			if (user != null)
			{
				UserDetail info = _service.UserDetailService.GetUserDetailById(user.UserId);
				if (user.AccessFail == 0)
				{
					user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
					_service.UserService.UpdateUser(user, user.UserId);
					return Ok(new { token = Util.GenerateToken(user.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config) });
				}   // Legit, nhập đúng từ lần đầu

				else if (user.AccessFail < 3) // Trước đó nhập sai chưa tới lần 3
				{
					user.AccessFail = 0;
					user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
					user.ActiveStatus = true;
					_service.UserService.UpdateUser(user, user.UserId);
					return Ok(new { token = Util.GenerateToken(user.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config) });
				}

				else if (user.AccessFail == 3) // Trước đó nhập sai tới lần 3
				{
					if ((DateTime.Now - user.LastFail).Value.TotalMinutes >= 15)
					{
						user.AccessFail = 0;
						user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
						user.ActiveStatus = true;
						_service.UserService.UpdateUser(user, user.UserId);
						return Ok(new { token = Util.GenerateToken(user.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config) });
					}
					return BadRequest(new { msg = "Temporaly locked" });
				}

				else if (user.AccessFail == 4)  // Trước đó nhập sai tới lần 4
				{
					if ((DateTime.Now - user.LastFail).Value.TotalMinutes >= 30)
					{
						user.AccessFail = 0;
						user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
						user.ActiveStatus = true;
						_service.UserService.UpdateUser(user, user.UserId);
						return Ok(new { token = Util.GenerateToken(user.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config) });
					}
					return BadRequest(new { msg = "Temporaly locked" });
				}

				else if (user.AccessFail == 5)  // Trước đó nhập sai tới lần 5
				{
					if ((DateTime.Now - user.LastFail).Value.TotalHours >= 1)
					{
						user.AccessFail = 0;
						user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
						user.ActiveStatus = true;
						_service.UserService.UpdateUser(user, user.UserId);
						return Ok(new { token = Util.GenerateToken(user.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config) });
					}
					return BadRequest(new { msg = "Temporaly locked" });
				}

				else return BadRequest(new { msg = "Locked. Contact" });
			}

			else
			{
				User failCase = _service.UserService.GetAllUsers().FirstOrDefault(x => x.UserName == username);

				if (failCase != null) // Đúng username, sai pass
				{
					if (failCase.AccessFail <= 5) // Sai từ dưới 5 lần
					{
						failCase.AccessFail += 1;
						failCase.LastFail = DateTime.Now;
						if (failCase.AccessFail > 2)
							failCase.ActiveStatus = false;
						_service.UserService.UpdateUser(failCase, failCase.UserId);
						return BadRequest(new { msg = "Incorrect username or password!" });
					}
					return BadRequest(new { msg = "Locked! Contact" }); // Từ lần sai thứ 6 đổ đi đã khóa acc, liên hệ admin để giải quyết
				}
				return BadRequest(new { msg = "Incorrect username or password!" }); // Sai username, ko quan tâm pass
			}
		}


		[HttpPost]
		[Route("User/ForgetPassword")]
		public async Task<IActionResult> ResetPass(string newPass, string mail)
		{
			User user = _service.UserService.GetUserById(_service.UserDetailService.GetUserDetailByMail(mail).UserId);
			if (user.AccessFail <= 5)
			{
				string hashNewPass = Util.ToHashString(newPass);
				if (!hashNewPass.Equals(user.Password))
				{
					bool status = Util.IsPasswordSecure(newPass);
					if (status)
					{
						user.Password = hashNewPass;
						_service.UserService.UpdateUser(user, user.UserId);
						return Json("Success");
					}
					return Json("Password is not properly secure enough");
				}
				return Json("New password can't be same as previous one");
			}
			return Ok("Locked! Contact Admin");
		}


		[HttpPut]
		[Route("User/Update")]
		//[Authorize]
		public async Task<IActionResult> UpdateUser(string id, string username, string password, string branchId, string roleId, string firstName, string lastName, string phone, string? email, string facebook, float balence, int? accessFail, bool status, string img)
		{
			List<User> storage = _service.UserService.GetAllUsers();
			List<UserDetail> infoStorage = _service.UserDetailService.GetAllUserDetails();

			User user = _service.UserService.GetUserById(id);
			if (!username.IsNullOrEmpty())
			{
				if (user.UserName == username)
					user.UserName = username;
				else if (storage.FirstOrDefault(x => x.UserName == username) == null)
					user.UserName = username;
				else return BadRequest(new { msg = "Username existed" });
			}
			if (Util.IsPasswordSecure(password))
				//if (user.Password != Util.ToHashString(password)) // hash pass
				user.Password = Util.ToHashString(password); // Lay lai pass cu
			if (accessFail != null)
			{
				if (accessFail == 0)
					user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
				user.AccessFail = accessFail;
			}
			if (!roleId.IsNullOrEmpty())
			{
				user.RoleId = roleId;
				if (roleId != "R002")
					if (!branchId.IsNullOrEmpty())
						return BadRequest(new { msg = "Only staffs can contain working place" });
			}
			user.BranchId = branchId;
			if (balence != null)
			{
				if (balence >= 0)
					user.Balance = balence;
				else return BadRequest(new { msg = "Balence must be at least 0" });
			}
			if (status != null)
			{
				if (status == true)
					if (accessFail.HasValue && accessFail == 0)
						user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
				user.ActiveStatus = status;
			}
			_service.UserService.UpdateUser(user, id);

			// Update xong user chuyển tiếp qua detail
			UserDetail info = _service.UserDetailService.GetUserDetailById(id);
			if (!firstName.IsNullOrEmpty())
				info.FirstName = firstName;
			if (!lastName.IsNullOrEmpty())
				info.LastName = lastName;
			if (!img.IsNullOrEmpty())
				info.Img = img;
			if (!email.IsNullOrEmpty())
			{
				if (info.Email == email)
					info.Email = email;
				else if (infoStorage.FirstOrDefault(x => x.Email == email) == null)
					info.Email = email;
				else return BadRequest(new { msg = "Email registered" });
			}
			if (!phone.IsNullOrEmpty())
			{
				if (info.Phone == phone)
					info.Phone = phone;
				else if (infoStorage.FirstOrDefault(x => x.Phone == phone) == null)
					info.Phone = phone;
				else return BadRequest(new { msg = "Phone number registered" });
			}
			if (!facebook.IsNullOrEmpty())
			{
				if (info.Facebook == facebook)
					info.Facebook = facebook;
				else if (infoStorage.FirstOrDefault(x => x.Facebook == facebook) == null)
					info.Facebook = facebook;
				else return BadRequest(new { msg = "Facebook registered" });
			}
			if (Util.IsPhoneFormatted(phone))
				info.Phone = phone;
			_service.UserDetailService.UpdateUserDetail(info, id);
			return Ok(new { msg = "Success" });
		}


		[HttpPost]
		[Route("User/Register")]
		public async Task<IActionResult> Register(string username, string password, string firstName, string lastName, string email, string phone)
		{
			UserDetail info = _service.UserDetailService.GetAllUserDetails().FirstOrDefault(x => x.Email.Equals(email) || x.Phone == phone);
			if (info == null)
			{
				if (!username.IsNullOrEmpty())
				{
					if (_service.UserService.GetAllUsers().FirstOrDefault(x => x.UserName.Equals(username)) == null)
					{
						if (Util.IsPhoneFormatted(phone))
						{

							if (Util.IsPasswordSecure(password))
							{
								//Hash pass
								//service.userService.AddUser(new User { UserId = GenerateId(), UserName = username, Password = ToHashString(password), AccessFail = 0, ActiveStatus = true, Balance = 0, BranchId = null, LastFail = new DateTime(1900, 1, 1, 0, 0, 0), RoleId = "R003" });

								string userId = "U" + (_service.UserService.GetAllUsers().Count + 1).ToString("D7");
								_service.UserService.AddUser(new User { UserId = userId, UserName = username, Password = password, AccessFail = 0, ActiveStatus = true, Balance = 0, BranchId = null, LastFail = new DateTime(1900, 1, 1, 0, 0, 0), RoleId = "R003" });
								User user = _service.UserService.GetUserById(userId);
								_service.UserDetailService.AddUserDetail(new UserDetail { UserId = userId, FirstName = firstName, LastName = lastName, Email = email, Phone = phone });
								return Ok(Util.GenerateToken(userId, _service.UserDetailService.GetUserDetailById(userId).LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config));
							}
							return BadRequest(new { msg = "Password is not properly secured" });
						}
						return BadRequest(new { msg = "Phone number is not properly formatted" });
					}
					return BadRequest(new { msg = "Username existed" });
				}
				return BadRequest(new { msg = "Username can't be empty" });

			}
			return BadRequest(new { msg = "Email registered" });
		}


		[HttpPost]
		[Route("User/Add")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> AddUser(ProvideAccount account) 
		{
			List<User> storage = _service.UserService.GetAllUsers();
			List<UserDetail> infoStorage = _service.UserDetailService.GetAllUserDetails();

			if (!account.UserName.IsNullOrEmpty())
				if (storage.FirstOrDefault(x => x.UserName == account.UserName) != null)
					return BadRequest(new { msg = "Account existed" });
			if (!account.Email.IsNullOrEmpty())
			{
				if (infoStorage.FirstOrDefault(x => x.Email == account.Email) != null)
					return BadRequest(new { msg = "Account existed" });
			}
			else return BadRequest(new { msg = "At least email can't be empty" });
			if (!account.Facebook.IsNullOrEmpty())
				if (infoStorage.FirstOrDefault(p => p.Facebook == account.Facebook) != null)
					return BadRequest(new { msg = "Account existed" });
			if (account.Balance < 0)
				return BadRequest(new { msg = "Balance must be at least 0" });

			if (account.RoleId != "R002")
				if (!account.BranchId.IsNullOrEmpty())
					return BadRequest(new { msg = "Only staffs can contatin working place" });

			double balence;
			if (account.Balance == null)
				balence = 0;
			else balence = account.Balance.Value;
			string id = "U" + (storage.Count + 1).ToString("D7");
			_service.UserService.AddUser(new User { UserId = id, AccessFail = 0, ActiveStatus = true, LastFail = new DateTime(1900, 1, 1, 0, 0, 0), UserName = account.UserName, Password = account.Password, Balance = balence, BranchId = account.BranchId, RoleId = account.RoleId });
			_service.UserDetailService.AddUserDetail(new UserDetail { Email = account.Email, Facebook = account.Facebook, FirstName = account.FirstName, LastName = account.LastName, Phone = Util.IsPhoneFormatted(account.Phone) ? account.Phone : null, UserId = id, Img = account.Img });
			return Ok(new { msg = "Success" });
		}

		// Xóa account
		// Xóa userdetail -> xóa user: Set ActiveStatus = false
		// Ongoing

		[HttpDelete]
		[Route("User/Delete")]
		//[Authorize(Roles = "")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			_service.UserService.DeleteUser(id);
			return Ok();
		}

	}
}
