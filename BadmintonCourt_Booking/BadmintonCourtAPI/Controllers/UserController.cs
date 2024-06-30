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
using NuGet.Common;

namespace BadmintonCourtAPI.Controllers
{
	public class UserController : Controller
	{
		private readonly BadmintonCourtService _service = null;
		private readonly IConfiguration _config;
		private const string verifyUrl = "https://localhost:7233/User/VerifyAction";

		public UserController(IConfiguration config)
		{
			if (_service == null)
			{
				_service = new BadmintonCourtService(_config);
			}
			_config = config;
		}

		private List<User> GetUserListFromFilterFailAccount()
		{
			List<User> result = new List<User>();
			List<User> uList = _service.UserService.GetAllUsers();
			foreach (var item in uList)
			{
				if (item.ActionPeriod != null && !item.Token.IsNullOrEmpty() && (DateTime.Now - item.ActionPeriod).Value.TotalMinutes > 15 && item.AccessFail == 0 && item.ActiveStatus == false)
					continue;
				else result.Add(item);
			}
			return result;
		}

		private List<UserDetail> GetUserDetailListFromFilterFailAccount(List<User> uList)
		{
			List<UserDetail> result = new List<UserDetail>();
			foreach (var item in uList)
				result.Add(_service.UserDetailService.GetUserDetailById(item.UserId));
			return result;
		}



		[HttpGet]
		[Route("User/GetAll")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<User>>> GetAllUsers() => Ok(_service.UserService.GetAllUsers());

		[HttpGet]
		[Route("User/GetStaffsInBranch")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<User>>> GetStaffsByBranch(string id) => Ok(_service.UserService.GetStaffsByBranch(id).ToList());

		[HttpGet]
		[Route("User/GetByRole")]
		[Authorize(Roles = "Admin")]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(string id) => Ok(_service.UserService.GetUsersByRole(id));

		[HttpGet]
		[Route("User/GetById")]
		[Authorize]
		public async Task<ActionResult<User>> GetUserByUserId(string id) => Ok(_service.UserService.GetUserById(id));


		[HttpPost]
		[Route("User/ExternalLogAuth")]
		public async Task<IActionResult> ExternalAuth(string token)
		{
			string email = Util.GetMailFromToken(token);
			UserDetail info = _service.UserDetailService.GetUserDetailByMail(email);

			if (info == null) // Chua co acc
			{
				string userId = "U" + (_service.UserService.GetAllUsers().Count() + 1).ToString("D7");
				_service.UserService.AddUser(new User { UserId = userId, UserName = "", Password = "", LastFail = new DateTime(1900, 1, 1, 0, 0, 0), ActiveStatus = true, Balance = 0, AccessFail = 0, BranchId = null });


				_service.UserDetailService.AddUserDetail(new UserDetail { UserId = userId, Email = email, FirstName = "", LastName = "", Phone = "" });
				return Ok(new { token = Util.GenerateToken(userId, "", "", "Customer", _config) });
			}

			// Co acc
			User user = _service.UserService.GetUserById(info.UserId);
			return Ok(new { token = Util.GenerateToken(info.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config) });
		}


		//[HttpPost]
		//[Route("User/ExternalLogAuth")]
		//public async Task<IActionResult> ExternalAuth(string email)
		//{
		//	UserDetail info = _service.UserDetailService.GetUserDetailByMail(email);

		//	if (info == null) // Chua co acc
		//	{
		//		List<User> list = _service.UserService.GetAllUsers();
		//		_service.UserService.AddUser(new User { UserId = "S" + (_service.UserService.GetAllUsers().Count + 1).ToString("D7"), LastFail = new DateTime(1900, 1, 1, 0, 0, 0), ActiveStatus = true, Balance = 0, AccessFail = 0, BranchId = null, RoleId = "R003" });
		//		string id = _service.UserService.GetRecentAddedUser().UserId;

		//		_service.UserDetailService.AddUserDetail(new UserDetail { UserId = id, Email = email });
		//		return Ok(new { token = Util.GenerateToken(id, "", "", "Customer", _config) });
		//	}

		//	// Co acc
		//	User user = _service.UserService.GetUserById(info.UserId);
		//	return Ok(new { token = Util.GenerateToken(info.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config) });
		//}


		private string ResetUserStatus(User user)
		{
			UserDetail info = _service.UserDetailService.GetUserDetailById(user.UserId);
			user.AccessFail = 0;
			user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
			user.ActiveStatus = true;
			_service.UserService.UpdateUser(user, user.UserId);
			return Util.GenerateToken(user.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config);
		}

		//[HttpPost]
		//[Route("User/LoginAuth")]
		//public async Task<IActionResult> LoginAuth(string? username, string? password)
		//{
		//	//User user = service.userService.GetUserByLogin(username, password);
		//	User user = _service.UserService.GetAllUsers().FirstOrDefault(x => x.UserName == username && x.Password == password);

		//	// Nhập đúng username + pass
		//	if (user != null)
		//	{
		//		if (user.AccessFail == 0)
		//		{
		//			UserDetail info = _service.UserDetailService.GetUserDetailById(user.UserId);
		//			user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
		//			_service.UserService.UpdateUser(user, user.UserId);
		//			return Ok(new { token = Util.GenerateToken(user.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config) });
		//		}   // Legit, nhập đúng từ lần đầu

		//		else if (user.AccessFail < 3) // Trước đó nhập sai chưa tới lần 3
		//		{
		//			return Ok(new { token = ResetUserStatus(user) });
		//		}

		//		else if (user.AccessFail == 3) // Trước đó nhập sai tới lần 3
		//		{
		//			if ((DateTime.Now - user.LastFail).Value.TotalMinutes >= 15)
		//			{
		//				user.AccessFail = 0;
		//				user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
		//				user.ActiveStatus = true;
		//				_service.UserService.UpdateUser(user, user.UserId);
		//				return Ok(new { token = Util.GenerateToken(user.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config) });
		//			}
		//			return BadRequest(new { msg = "Temporaly locked" });
		//		}

		//		else if (user.AccessFail == 4)  // Trước đó nhập sai tới lần 4
		//		{
		//			if ((DateTime.Now - user.LastFail).Value.TotalMinutes >= 30)
		//			{
		//				user.AccessFail = 0;
		//				user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
		//				user.ActiveStatus = true;
		//				_service.UserService.UpdateUser(user, user.UserId);
		//				return Ok(new { token = Util.GenerateToken(user.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config) });
		//			}
		//			return BadRequest(new { msg = "Temporaly locked" });
		//		}

		//		else if (user.AccessFail == 5)  // Trước đó nhập sai tới lần 5
		//		{
		//			if ((DateTime.Now - user.LastFail).Value.TotalHours >= 1)
		//			{
		//				user.AccessFail = 0;
		//				user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
		//				user.ActiveStatus = true;
		//				_service.UserService.UpdateUser(user, user.UserId);
		//				return Ok(new { token = Util.GenerateToken(user.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config) });
		//			}
		//			return BadRequest(new { msg = "Temporaly locked" });
		//		}

		//		else return BadRequest(new { msg = "Locked. Contact" });
		//	}

		//	else
		//	{
		//		User failCase = _service.UserService.GetAllUsers().FirstOrDefault(x => x.UserName == username);
		//		//----------------------------------------------------------------------------------------------
		//		if (failCase != null) // Đúng username, sai pass
		//		{
		//			if (failCase.AccessFail <= 5) // Sai từ dưới 5 lần
		//			{
		//				//----------------------------------------------
		//				if (failCase.AccessFail == 3)
		//				{
		//					if ((DateTime.Now - failCase.LastFail).Value.TotalMinutes < 15) 
		//					{
		//						return BadRequest(new { msg = "Temporarily locked" });
		//					}
		//				}
		//				//----------------------------------------------
		//				else if (failCase.AccessFail == 4)
		//				{
		//					if ((DateTime.Now - failCase.LastFail).Value.TotalMinutes < 30)
		//					{
		//						return BadRequest(new { msg = "Temporarily locked" });
		//					}
		//				}
		//				//----------------------------------------------
		//				else if (failCase.AccessFail == 5)
		//				{
		//					if ((DateTime.Now - failCase.LastFail).Value.TotalHours < 1)
		//					{
		//						return BadRequest(new { msg = "Temporarily locked" });
		//					}
		//				}
		//				//----------------------------------------------
		//				failCase.AccessFail += 1;
		//				failCase.LastFail = DateTime.Now;
		//				if (failCase.AccessFail > 2)
		//					failCase.ActiveStatus = false;
		//				_service.UserService.UpdateUser(failCase, failCase.UserId);
		//				return BadRequest(new { msg = "Incorrect username or password!" });
		//			}
		//			return BadRequest(new { msg = "Locked! Contact" }); // Từ lần sai thứ 6 đổ đi đã khóa acc, liên hệ admin để giải quyết
		//		}
		//		return BadRequest(new { msg = "Incorrect username or password!" }); // Sai username, ko quan tâm pass
		//	}
		//}

		private void ResetFailAction(User user) // Công dụng: giành để reset lại các account chưa bị ban thực hiện việc thay đổi password/ quên pass rồi reset pass nhưng chưa verify lúc nhận đc mail xác nhận hoặc quá hạn xác nhận mà hủy chưa update đc xuống db
		{
			if ((DateTime.Now - user.ActionPeriod).Value.TotalMinutes > 15)
			{
				bool check = false;
				if (user.ActionPeriod != null)
				{
					user.ActionPeriod = null;
					check = true;
				}
				if (user.Token != null)
				{
					user.Token = null;
					check = true;
				}
				if (check)
					_service.UserService.UpdateUser(user, user.UserId);
			}
		}


		[HttpPost]
		[Route("User/LoginAuth")]
		public async Task<IActionResult> LoginAuth(string? username, string? password) // Thử nghiệm lại chạy thử nếu ko ổn hoặc văng lỗi thì xài lại hàm đã cmt ở trên
		{
			//User user = service.userService.GetUserByLogin(username, password);
			User user = GetUserListFromFilterFailAccount().FirstOrDefault(x => x.UserName == username && x.Password == password);

			// Nhập đúng username + pass
			if (user != null)
			{
				if (user.Token != null || user.ActionPeriod != null)
					ResetFailAction(user);
				//-------------------------------------------------------------------------
				if (user.AccessFail == 0)
				{
					UserDetail info = _service.UserDetailService.GetUserDetailById(user.UserId);
					user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
					_service.UserService.UpdateUser(user, user.UserId);
					return Ok(new { token = Util.GenerateToken(user.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config) });
				}   // Legit, nhập đúng từ lần đầu
					//-------------------------------------------------------------------------
				else if (user.AccessFail < 3) // Trước đó nhập sai chưa tới lần 3
				{
					return Ok(new { token = ResetUserStatus(user) });
				}
				//-------------------------------------------------------------------------
				else if (user.AccessFail == 3) // Trước đó nhập sai tới lần 3
				{
					if ((DateTime.Now - user.LastFail).Value.TotalMinutes < 15) // Chưa hết tg ban
						return BadRequest(new { msg = "Temporaly locked" });
				}
				//-------------------------------------------------------------------------
				else if (user.AccessFail == 4)  // Trước đó nhập sai tới lần 4
				{
					if ((DateTime.Now - user.LastFail).Value.TotalMinutes < 30) // Chưa hết tg ban
						return BadRequest(new { msg = "Temporaly locked" });
				}
				//-------------------------------------------------------------------------
				else if (user.AccessFail == 5)  // Trước đó nhập sai tới lần 5
				{
					if ((DateTime.Now - user.LastFail).Value.TotalHours < 1) // Chưa hết tg ban
						return BadRequest(new { msg = "Temporaly locked" });
				}
				//-------------------------------------------------------------------------
				else if (user.AccessFail == 6) // Hết cứu 6 lần 
					return BadRequest(new { msg = "Locked. Contact Admin!" });
				//-------------------------------------------------------------------------
				return Ok(new { token = ResetUserStatus(user) }); // Legit pass hết đk: ko quá 6 lần fail, đã hết thời gian ban
			}
			else
			{
				User failCase = GetUserListFromFilterFailAccount().FirstOrDefault(x => x.UserName == username);
				//----------------------------------------------------------------------------------------------
				if (failCase != null) // Đúng username, sai pass
				{
					if (failCase.Token != null || failCase.ActionPeriod != null)
						ResetFailAction(failCase);
					//----------------------------------------------------------------------------------------------
					if (failCase.AccessFail <= 5) // Sai từ dưới 5 lần
					{
						//----------------------------------------------
						if (failCase.AccessFail == 3)
						{
							if ((DateTime.Now - failCase.LastFail).Value.TotalMinutes < 15)
								return BadRequest(new { msg = "Temporarily locked" });
						}
						//----------------------------------------------
						else if (failCase.AccessFail == 4)
						{
							if ((DateTime.Now - failCase.LastFail).Value.TotalMinutes < 30)
								return BadRequest(new { msg = "Temporarily locked" });
						}
						//----------------------------------------------
						else if (failCase.AccessFail == 5)
						{
							if ((DateTime.Now - failCase.LastFail).Value.TotalHours < 1)
								return BadRequest(new { msg = "Temporarily locked" });
						}
						//----------------------------------------------
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
		[Route("User/ForgotPassReset")]
		public async Task<IActionResult> ResetPass(string id, string? password)
		{
			if (Util.IsPasswordSecure(password))
			{
				if (!id.IsNullOrEmpty())
				{
					User user = _service.UserService.GetUserById(id);
					if (user != null)  // Đảm bảo id real, ko fake url
					{
						// user.Password = Util.ToHashString(password); // Hash pass
						user.Password = password;
						user.ActiveStatus = true;
						user.AccessFail = 0;
						user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
						user.Token = null;
						user.ActionPeriod = null;
						_service.UserService.UpdateUser(user, id);
						return Ok(new { msg = "Success" });
					}
					return BadRequest(new { msg = "Account not exist" });
				}
				return BadRequest(new { msg = "Account not exist" });
			}
			return BadRequest(new { msg = "Password is not secure enough" });
		}


		[HttpPost]
		[Route("User/VerifyBeforeReset")]
		public async Task<IActionResult> VerifyBeforeReset(string? mail)
		{
			UserDetail info = _service.UserDetailService.GetUserDetailByMail(mail);
			if (info != null) // Có tồn tại acc này
			{
				User user = _service.UserService.GetUserById(info.UserId);
				if (user.AccessFail <= 5) // Chưa bị ban vv
				{
					string token = Util.GenerateToken(user.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config);
					user.ActionPeriod = DateTime.Now;
					user.Token = token;
					_service.UserService.UpdateUser(user, user.UserId);
					_service.MailService.SendMail(info.Email, $"Click <a href='{verifyUrl}?rawToken={token}:2'>HERE</a> to verify your reset-password process", "BMTC - Account Reset-Password Verification");
					return Ok(new { msg = "Please check your mail box to verify the reset-pass process" });
				}
				return BadRequest(new { msg = "Locked! Contact Admin" });
			}
			return BadRequest(new { msg = "Account not existed" });
		}


		[HttpPut]
		[Route("User/Update")]
		[Authorize]
		public async Task<IActionResult> UpdateUser(string id, string username, string password, string branchId, string roleId, string firstName, string lastName, string phone, string? email, string facebook, float balence, int? accessFail, bool status, string img)
		{
			List<User> storage = GetUserListFromFilterFailAccount();
			List<UserDetail> infoStorage = GetUserDetailListFromFilterFailAccount(storage);

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
				user.AccessFail = accessFail.Value;
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

			// Update xong user chuyển tiếp qua detail
			UserDetail info = _service.UserDetailService.GetUserDetailById(id);
			if (!firstName.IsNullOrEmpty())
				info.FirstName = firstName;
			if (!lastName.IsNullOrEmpty())
				info.LastName = lastName;
			if (!img.IsNullOrEmpty())
				info.Img = img;
			if (!phone.IsNullOrEmpty())
			{
				if (Util.IsPhoneFormatted(phone))
				{
					if (info.Phone == phone)
						info.Phone = phone;
					else if (infoStorage.FirstOrDefault(x => x.Phone == phone) == null)
						info.Phone = phone;
					else return BadRequest(new { msg = "Phone number registered" });
				}
				else return BadRequest(new { msg = "Phone number is not properly formatted" });
			}


			if (!facebook.IsNullOrEmpty())
			{
				if (info.Facebook == facebook)
					info.Facebook = facebook;
				else if (infoStorage.FirstOrDefault(x => x.Facebook == facebook) == null)
					info.Facebook = facebook;
				else return BadRequest(new { msg = "Facebook registered" });
			}
			if (!string.IsNullOrEmpty(email))
			{
				if (info.Email == email)
					info.Email = email;
				else if (infoStorage.FirstOrDefault(x => x.Email == email) == null)
				{
					string token = Util.GenerateToken(user.UserId, info.LastName, user.UserName, _service.RoleService.GetRoleById(user.RoleId).RoleName, _config);
					user.ActionPeriod = DateTime.Now;
					user.Token = token;
					_service.UserService.UpdateUser(user, id);
					_service.UserDetailService.UpdateUserDetail(info, id);
					_service.MailService.SendMail(email, $"Click <a href='{verifyUrl}?rawToken={token}:2:{email}'>HERE</a> to verify your update-mail profile process", "BMTC - Account Update Mail Profile Verification");
					return Ok(new { msg = "Please check your mail box to activate your new email" });
				}
				else return BadRequest(new { msg = "Email registered" });
			}
			_service.UserService.UpdateUser(user, id);
			_service.UserDetailService.UpdateUserDetail(info, id);
			return Ok(new { msg = "Success" });
		}

		[HttpPost]
		[Route("User/Register")]
		public async Task<IActionResult> Register(string? username, string? password, string? firstName, string? lastName, string? email, string? phone)
		{
			if (!Util.IsPhoneFormatted(phone))
				return BadRequest(new { msg = "Phone number is not properly formatted" });
			if (username.IsNullOrEmpty())
				return BadRequest(new { msg = "Username can't be empty" });
			if (email.IsNullOrEmpty())
				return BadRequest(new { msg = "Email can't be empty" });
			if (firstName.IsNullOrEmpty() || lastName.IsNullOrEmpty())
				return BadRequest(new { msg = "Name can't be empty" });
			//if (Util.IsPasswordSecure(password))  // Check password manh. yeu^'
			//	return BadRequest(new { msg = "Password invalid" });
			if (password.IsNullOrEmpty())
				return BadRequest(new { msg = "Password can't be empty" });


			//User user = _service.UserService.GetAllUsers().FirstOrDefault(x => x.UserName == username);
			//UserDetail info = _service.UserDetailService.GetAllUserDetails().FirstOrDefault(x => x.Phone == phone || x.Email == email);
			if (_service.UserService.GetAllUsers().FirstOrDefault(x => x.UserName == username) != null || _service.UserDetailService.GetAllUserDetails().FirstOrDefault(x => x.Phone == phone || x.Email == email) != null)
				return BadRequest(new { msg = "Account existed" });

			string userId = "U" + (_service.UserService.GetAllUsers().Count() + 1).ToString("D7");
			string token = Util.GenerateToken(userId, lastName, username, "R003", _config);
			string content = $"Click to verify your account";

			_service.UserService.AddUser(new User
			{
				UserId = userId,
				AccessFail = 0,
				LastFail = new DateTime(1900, 1, 1, 0, 0, 0),
				Balance = 0,
				ActiveStatus = false,
				UserName = username,
				//Password = Util.ToHashString(password),   // Hash pass
				Password = password,
				RoleId = "R003",
				Token = token,
				ActionPeriod = DateTime.Now
			});

			_service.UserDetailService.AddUserDetail(new UserDetail
			{
				UserId = userId,
				Email = email,
				FirstName = firstName,
				LastName = lastName,
				Phone = phone,
			});

			_service.MailService.SendMail(email, $"Click <a href='{verifyUrl}?rawToken={token}:1'>HERE</a> to verify your account", "BMTC - Account Registration Verification");
			return Ok(new { token = token }); // Trả lại để check chơi
											  // Vào demo thực tế sẽ bỏ
		}


		[HttpGet]
		[Route("User/VerifyAction")]
		public async Task<IActionResult> VerifyTokenAction(string? rawToken)
		{
			string token = rawToken.Split(':')[0];
			User user = _service.UserService.GetAllUsers().FirstOrDefault(x => x.Token == token);
			if (user != null)
			{
				string type = rawToken.Split(':')[1]; // Loại verify token: verify đkí - 1 / verify reset pass - 2
				if ((DateTime.Now - user.ActionPeriod.Value).TotalMinutes > 15) // Quá hạn token	
				{
					if (type == "1") // Loại đkí mà quá hạn thời gian
					{
						_service.UserDetailService.DeleteUserDetail(user.UserId);
						_service.UserService.DeleteUser(user.UserId);
					}
					return BadRequest(new { msg = "Out of time" });
				}
				//--------------------------------------------------------
				// Còn tg

				user.Token = null;
				user.ActionPeriod = null;
				if (type == "2") // Loại reset pass mà đủ tg 
				{
					_service.UserService.UpdateUser(user, user.UserId);
					return Redirect($"url?id={user.UserId}"); // Trả về trang nhập pass mới - đây là url của FE với tham số para truyền đi là id 
				}
				else if (type == "3")
				{
					UserDetail info = _service.UserDetailService.GetUserDetailById(user.UserId);
					info.Email = rawToken.Split(':')[2];
					_service.UserDetailService.UpdateUserDetail(info, user.UserId);
					_service.UserService.UpdateUser(user, user.UserId);
					return Ok(new { msg = "Success" });
				}
				user.ActiveStatus = true;
				_service.UserService.UpdateUser(user, user.UserId);
				return Ok(new { msg = "Success" });
			}
			// Fake info, url
			return BadRequest(new { msg = "Invalid information" });
		}


		[HttpPost]
		[Route("User/Add")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<IActionResult> AddUser(ProvideAccount account)
		{
			List<User> storage = GetUserListFromFilterFailAccount();
			List<UserDetail> infoStorage = GetUserDetailListFromFilterFailAccount(storage);

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



			string id = "U" + (_service.UserService.GetAllUsers().Count + 1).ToString("D7");
			_service.UserService.AddUser(new User { UserId = id, AccessFail = 0, ActiveStatus = true, LastFail = new DateTime(1900, 1, 1, 0, 0, 0), UserName = account.UserName, Password = account.Password, Balance = account.Balance == null ? 0 : account.Balance.Value, BranchId = account.BranchId, RoleId = account.RoleId });
			_service.UserDetailService.AddUserDetail(new UserDetail { Email = account.Email, Facebook = account.Facebook, FirstName = account.FirstName, LastName = account.LastName, Phone = Util.IsPhoneFormatted(account.Phone) ? account.Phone : null, UserId = id, Img = account.Img });
			return Ok(new { msg = "Success" });
		}


		[HttpDelete]
		[Route("User/Delete")]
		[Authorize]
		public async Task<IActionResult> DeleteUser(string id)
		{
			_service.UserService.DeleteUser(id);
			return Ok();
		}

	}
}
