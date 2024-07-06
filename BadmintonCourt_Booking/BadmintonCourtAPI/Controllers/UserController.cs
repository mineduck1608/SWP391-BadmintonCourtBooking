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
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using BadmintonCourtServices.IService;

namespace BadmintonCourtAPI.Controllers
{
	public class UserController : Controller
	{
		private readonly IUserService _service;
		private readonly IUserDetailService _infoService = new UserDetailService();
		private readonly IRoleService _roleService = new RoleService();
		private readonly IMailService _mailService = new MailService();

		private readonly IConfiguration _config;
		private const string verifyUrl = "https://localhost:7233/User/VerifyAction";
		private const string resetPassUrl = "http://localhost:3000/ResetPassword";
		private const string loginUrl = "http://localhost:3000/signin";
		private const string registerMsg = "If you did not register for this account, ";
		public const string emailUpdateMsg = "If you did not request an email update, ";
		public const string resetPassMsg = "If you did not request a password reset, ";

		//public UserController(IConfiguration config)
		//{
		//	if (_service == null)
		//	{
		//		_service = new BadmintonCourtService(_config);
		//	}
		//	_config = config;
		//}

		public UserController(IUserService service)
		{
			_service = service;
		}

		private List<User> GetUserListFromFilterFailAccount()
		{
			List<User> result = new List<User>();
			List<User> uList = _service.GetAllUsers();
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
				result.Add(_infoService.GetUserDetailById(item.UserId));
			return result;
		}

		private string GenerateMailBody(int type) => "<p>" + (type == 1 ? registerMsg : (type == 2 ? resetPassMsg : emailUpdateMsg)) + "please ignore this email or contact us for support.</p>\r\n<p>Best regards,<br>\r\nBadmintonCourtBooking BMTC</p>\r\n<p>Contact Information:<br>\r\nPhone: 0977300916<br>\r\nAddress: 123 Badminton St, Hanoi, Vitnam<br>\r\nEmail: externalauthdemo1234@gmail.com</p>";


		[HttpGet]
		[Route("User/GetAll")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<User>>> GetAllUsers() => Ok(_service.GetAllUsers());

		[HttpGet]
		[Route("User/GetStaffsInBranch")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<IEnumerable<User>>> GetStaffsByBranch(string id) => Ok(_service.GetStaffsByBranch(id).ToList());

		[HttpGet]
		[Route("User/GetByRole")]
		[Authorize(Roles = "Admin")]
		[Authorize(Roles = "Staff")]
		public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(string id) => Ok(_service.GetUsersByRole(id));

		[HttpGet]
		[Route("User/GetById")]
		[Authorize]
		public async Task<ActionResult<User>> GetUserByUserId(string id) => Ok(_service.GetUserById(id));


		[HttpPost]
		[Route("User/ExternalLogAuth")]
		public async Task<IActionResult> ExternalAuth(string token)
		{
			string email = Util.GetMailFromToken(token);
			UserDetail info = _infoService.GetUserDetailByMail(email);

			if (info == null) // Chua co acc
			{
				string userId = "U" + (_service.GetAllUsers().Count() + 1).ToString("D7");
				_service.AddUser(new User { UserId = userId, UserName = "", Password = "", LastFail = new DateTime(1900, 1, 1, 0, 0, 0), ActiveStatus = true, Balance = 0, AccessFail = 0, BranchId = null, RoleId = "R003" });


				_infoService.AddUserDetail(new UserDetail { UserId = userId, Email = email, FirstName = "", LastName = "", Phone = "" });
				return Ok(new { token = Util.GenerateToken(userId, true, "", "Customer", 1) });
			}

			// Co acc

			User user = _service.GetUserById(info.UserId);
			if (user.Token != null || user.ActionPeriod != null)
				ResetFailAction(user);
			return Ok(new { token = Util.GenerateToken(info.UserId, user.ActiveStatus.Value, user.UserName, _roleService.GetRoleById(user.RoleId).RoleName, 1) });
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
			user.AccessFail = 0;
			user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
			user.ActiveStatus = true;
			_service.UpdateUser(user, user.UserId);
			return Util.GenerateToken(user.UserId, true, user.UserName, _roleService.GetRoleById(user.RoleId).RoleName, 1);
		}



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
					_service.UpdateUser(user, user.UserId);
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
				if (user.RoleId == "R001")
					return Ok(new { token = Util.GenerateToken(user.UserId, true, username, "Admin", 1) });

				if (user.Token != null || user.ActionPeriod != null)
					ResetFailAction(user);
				//-------------------------------------------------------------------------
				if (user.AccessFail == 0)
				{
					user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
					_service.UpdateUser(user, user.UserId);
					return Ok(new { token = Util.GenerateToken(user.UserId, true, username, _roleService.GetRoleById(user.RoleId).RoleName, 1) });
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
					if (failCase.RoleId == "R001") // Admin thi` k lock acc
						return BadRequest(new { msg = "Incorrect username or password" });

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
						_service.UpdateUser(failCase, failCase.UserId);
						return BadRequest(new { msg = "Incorrect username or password!" });
					}
					return BadRequest(new { msg = "Locked! Contact" }); // Từ lần sai thứ 6 đổ đi đã khóa acc, liên hệ admin để giải quyết
				}
				return BadRequest(new { msg = "Incorrect username or password!" }); // Sai username, ko quan tâm pass
			}
		}


		[HttpPost]
		[Route("User/ForgotPassReset")]
		public async Task<IActionResult> ResetPass(string id, string? password, string? confirmPassword)
		{
			if (password.IsNullOrEmpty() || confirmPassword.IsNullOrEmpty() || confirmPassword != password)
			{
				return Redirect($"{resetPassUrl}?id={id}&&msg=fail");
			}
			if (Util.IsPasswordSecure(password))
			{
				if (!id.IsNullOrEmpty())
				{
					User user = _service.GetUserById(id);
					if (user != null)  // Đảm bảo id real, ko fake url
					{
						// user.Password = Util.ToHashString(password); // Hash pass
						user.Password = password;
						user.ActiveStatus = true;
						user.AccessFail = 0;
						user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
						user.Token = null;
						user.ActionPeriod = null;
						_service.UpdateUser(user, id);
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
			//UserDetail info = _service.UserDetailService.GetUserDetailByMail(mail);
			UserDetail info = _infoService.GetUserDetailByMail(mail);
			if (info != null) // Có tồn tại acc này
			{
				User user = _service.GetUserById(info.UserId);
				if (user.AccessFail <= 5) // Chưa bị ban vv
				{
					string token = Util.GenerateToken(user.UserId, user.ActiveStatus.Value, user.UserName, _roleService.GetRoleById(user.RoleId).RoleName, 0);
					user.ActionPeriod = DateTime.Now;
					user.Token = token;
					_service.UpdateUser(user, user.UserId);
					int type = 2;
					string begin = "<p>Dear ";
					begin += (info.FirstName.IsNullOrEmpty() && info.LastName.IsNullOrEmpty()) ? $"{mail}," : $"{info.FirstName} {info.LastName},";
					begin += $"</p><p>We received a request to reset your password for your BadmintonCourtBooking account. Click the link below to verify your reset-password process:</p>\r\n<p><a href='{verifyUrl}?rawToken={token}:{type}'>HERE</a></p>";
					_mailService.SendMail(info.Email, begin + GenerateMailBody(type), "BMTC - Account Reset-Password Verification");
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

			User user = _service.GetUserById(id);
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
				//user.Password = Util.ToHashString(password); // Lay lai pass cu
				user.Password = password;
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
				else return BadRequest(new { msg = "Balance must be at least 0" });
			}
			if (status != null)
			{
				if (status == true)
					if (accessFail.HasValue && accessFail == 0)
						user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
				user.ActiveStatus = status;
			}

			// Update xong user chuyển tiếp qua detail
			UserDetail info = _infoService.GetUserDetailById(id);
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
				if (info.Email.ToLower() == email.ToLower())
					info.Email = email;
				else
				{
					if (Util.IsMailFormatted(email))
					{
						if (infoStorage.FirstOrDefault(x => x.Email.ToLower() == email.ToLower()) == null)
						{
							string token = Util.GenerateToken(user.UserId, user.ActiveStatus.Value, user.UserName, _roleService.GetRoleById(user.RoleId).RoleName, 0);
							user.ActionPeriod = DateTime.Now;
							user.Token = token;
							_service.UpdateUser(user, id);
							_infoService.UpdateUserDetail(info, id);
							//---------------------------------------------------------
							int type = 3;
							string begin = "<p>Dear ";
							begin += (info.FirstName.IsNullOrEmpty() && info.LastName.IsNullOrEmpty()) ? $"{email}," : $"{info.FirstName} {info.LastName},";
							begin += $"</p><p>We received a request to update your email for your BadmintonCourtBooking account. Click the link below to verify your reset-password process:</p>\r\n<p><a href='{verifyUrl}?rawToken={token}:{type}:{email}'>HERE</a></p>";
							//---------------------------------------------------------
							_mailService.SendMail(email, begin + GenerateMailBody(type), "BMTC - Account Update Mail Profile Verification");
							return Ok(new { msg = "Please check your mail box to activate your new email" });
						}
						else return BadRequest(new { msg = "Email registered" });
					}
					else return BadRequest(new { msg = "Email is not properly formatted" });
				}
			}
			_service.UpdateUser(user, id);
			_infoService.UpdateUserDetail(info, id);
			return Ok(new { msg = "Success" });
		}

		[HttpPost]
		[Route("User/Register")]
		public async Task<IActionResult> Register(string? username, string? password, string? firstName, string? lastName, string? email, string? phone)
		{
			if (!Util.IsPhoneFormatted(phone.Trim()))
				return BadRequest(new { msg = "Phone number is not properly formatted" });
			if (username.IsNullOrEmpty())
				return BadRequest(new { msg = "Username can't be empty" });
			if (email.IsNullOrEmpty())
				return BadRequest(new { msg = "Email can't be empty" });
			if (!Util.IsMailFormatted(email.Trim()))
				return BadRequest(new { msg = "Email is not properly formatted" });
			if (firstName.IsNullOrEmpty() || lastName.IsNullOrEmpty())
				return BadRequest(new { msg = "Name can't be empty" });
			//if (Util.IsPasswordSecure(password))  // Check password manh. yeu^'
			//	return BadRequest(new { msg = "Password invalid" });
			if (password.IsNullOrEmpty())
				return BadRequest(new { msg = "Password can't be empty" });
			if (Util.IsPasswordSecure(password))
				return BadRequest(new { msg = "Password is not secure enough" });


			//User user = _service.UserService.GetAllUsers().FirstOrDefault(x => x.UserName == username);
			//UserDetail info = _service.UserDetailService.GetAllUserDetails().FirstOrDefault(x => x.Phone == phone || x.Email == email);
			List<User> checkStorage = GetUserListFromFilterFailAccount();
			List<UserDetail> infoCheckStorage = GetUserDetailListFromFilterFailAccount(checkStorage);
			if (checkStorage.FirstOrDefault(x => x.UserName == username) != null)
				return BadRequest(new { msg = "Username existed" });
			if (infoCheckStorage.FirstOrDefault(x => x.Phone == phone) != null)
				return BadRequest(new { msg = "Phone number registered" });
			if (infoCheckStorage.FirstOrDefault(x => x.Email.ToLower() == email.ToLower()) != null)
				return BadRequest(new { msg = "Email registered" });

			string userId = "U" + (_service.GetAllUsers().Count() + 1).ToString("D7");
			string token = Util.GenerateToken(userId, false, username, "R003", 0);
			_service.AddUser(new User
			{
				UserId = userId,
				AccessFail = 0,
				LastFail = new DateTime(1900, 1, 1, 0, 0, 0),
				Balance = 0,
				ActiveStatus = false,
				UserName = username.Trim(),
				//Password = Util.ToHashString(password.Trim()),   // Hash pass
				Password = password.Trim(),
				RoleId = "R003",
				Token = token,
				ActionPeriod = DateTime.Now
			});

			_infoService.AddUserDetail(new UserDetail
			{
				UserId = userId,
				Email = email.Trim(),
				FirstName = firstName.Trim(),
				LastName = lastName.Trim(),
				Phone = phone.Trim(),
			});

			int type = 1;
			string style = "style='padding: 8px; border: 1px solid #ddd;'";
			string begin = $"<p>Dear {firstName} {lastName}, <p>Thank you for registering an account with BadmintonCourtBooking. Please click the link below to activate your account:</p>\r\n<p><a href='{verifyUrl}?rawToken={token}:{type}'>HERE</a></p>";
			_mailService.SendMail(email, $@"
<table {style}>
    <tr>
        <td {style}>Username</td>
        <td {style}>{username}</td>
    </tr>
    <tr>
        <td {style}>First name</td>
        <td {style}>{firstName}</td>
    </tr>
    <tr>
        <td {style}>Last name</td>
        <td {style}>{lastName}</td>
    </tr>
    <tr>
        <td {style}>Phone number</td>
        <td {style}>{phone}</td>
    </tr>
</table>
<br>" + begin
+ GenerateMailBody(type),
			"BMTC - Account Registration Verification");
			//------------------------------------------------------------
			return Ok(new { msg = "Please check your mail box to finish the registration process" });
		}


		[HttpGet]
		[Route("User/VerifyAction")]
		public async Task<IActionResult> VerifyTokenAction(string? rawToken)
		{
			string token = rawToken.Split(':')[0];
			User user = _service.GetAllUsers().FirstOrDefault(x => x.Token == token);
			if (user != null)
			{
				if ((DateTime.Now - user.ActionPeriod.Value).TotalMinutes > 15) // Quá hạn token	
				{
					return BadRequest(new { msg = "Out of time" });
				}
				//--------------------------------------------------------
				// Còn tg
				user.Token = null;
				user.ActionPeriod = null;
				string type = rawToken.Split(':')[1]; // Loại verify token: verify đkí - 1 / verify reset pass - 2 / Update profile - new email - 3
				if (type == "2") // Loại reset pass mà đủ tg 
				{
					_service.UpdateUser(user, user.UserId);
					return Redirect($"{resetPassUrl}?id={user.UserId}&&msg=initial"); // Trả về trang nhập pass mới - đây là url của FE với tham số para truyền đi là id 
				}
				else if (type == "3")
				{
					UserDetail info = _infoService.GetUserDetailById(user.UserId);
					info.Email = rawToken.Split(':')[2];
					_infoService.UpdateUserDetail(info, user.UserId);
					_service.UpdateUser(user, user.UserId);
					return Ok(new { msg = "Success" });
				}
				user.ActiveStatus = true;
				_service.UpdateUser(user, user.UserId);
				return Redirect(loginUrl);
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
					return BadRequest(new { msg = "Usrname existed" });
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



			string id = "U" + (_service.GetAllUsers().Count + 1).ToString("D7");
			_service.AddUser(new User { UserId = id, AccessFail = 0, ActiveStatus = true, LastFail = new DateTime(1900, 1, 1, 0, 0, 0), UserName = account.UserName, Password = account.Password, Balance = account.Balance == null ? 0 : account.Balance.Value, BranchId = account.BranchId, RoleId = account.RoleId });
			//_service.UserDetailService.AddUserDetail(new UserDetail { Email = account.Email, Facebook = account.Facebook, FirstName = account.FirstName, LastName = account.LastName, Phone = Util.IsPhoneFormatted(account.Phone) ? account.Phone : null, UserId = id, Img = account.Img });

			_infoService.AddUserDetail(new UserDetail { Email = account.Email, Facebook = account.Facebook, FirstName = account.FirstName, LastName = account.LastName, Phone = Util.IsPhoneFormatted(account.Phone) ? account.Phone : null, UserId = id, Img = account.Img });
			return Ok(new { msg = "Success" });
		}


		[HttpDelete]
		[Route("User/Delete")]
		[Authorize]
		public async Task<IActionResult> DeleteUser(string id)
		{
			_service.DeleteUser(id);
			return Ok();
		}

	}
}
