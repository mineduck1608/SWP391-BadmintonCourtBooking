using BadmintonCourtAPI.Schema;
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

namespace BadmintonCourtAPI.Controllers
{
    public class UserController : Controller
    {
        private readonly BadmintonCourtService service = null;
        public IConfiguration _config = null;

        public UserController(IConfiguration config)
        {
            if (service == null)
            {
                service = new BadmintonCourtService();
            }
            _config = config;
        }

        private string GetMailFromToken(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            return jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email").Value;
        }

        public bool IsPhoneFormatted(string phone) => new Regex(@"\d{9,11}").IsMatch(phone);

        private bool IsPasswordSecure(string password) => password != null ? new Regex(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=<>?])[A-Za-z\d!@#$%^&*()_\-+=<>?]{12,}").IsMatch(password) : false;

        public string GenerateToken(int id, string lastName, string username, string roleName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("UserId", id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Surname, lastName),
                new Claim(ClaimTypes.Role, roleName)
            };
            var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
              audience: _config["Jwt:Audience"],
    claims: claims,
    expires: DateTime.Now.AddMinutes(30),
    signingCredentials: credentials
    );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string ToHashString(string s)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash of the input string
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(s));

                // Convert the byte array to a hexadecimal string
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    // Convert each byte to a hexadecimal string and append to the builder
                    result.Append(bytes[i].ToString("x2"));
                return result.ToString();
            }
        }


        [HttpGet]
        [Route("User/GetAll")]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<UserSchema>>> GetAllUsers() => Ok(service.userService.GetAllUsers());

        [HttpGet]
        [Route("User/GetStaffsInBranch")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetStaffsByBranch(int id) => Ok(service.userService.GetStaffsByBranch(id).ToList());

        [HttpGet]
        [Route("User/GetByRole")]
        //[Authorize(Roles = "Admin")]
        //[Authorize(Roles = "Staff")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(int id) => Ok(service.userService.GetUsersByRole(id));

        [HttpGet]
        [Route("User/GetById")]
        //[Authorize(Roles = "Admin")]
        //[Authorize(Roles = "Staff")]
        public async Task<ActionResult<User>> GetUserByUserId(int id) => Ok(service.userService.GetUserById(id));


        [HttpPost]
        [Route("User/ExternalLogAuth")]
        public async Task<IActionResult> ExternalAuth(string token)
        {
            string email = GetMailFromToken(token);
            UserDetail info = service.userDetailService.GetUserDetailByMail(email);

            if (info == null) // Chua co acc
            {
                service.userService.AddUser(new User("", "", null, 3, 0, true, 0, new DateTime(1900, 1, 1, 0, 0, 0)));
                service.userDetailService.AddUserDetail(new UserDetail(service.userService.GetRecentAddedUser().UserId, "", "", email, ""));
                int id = service.userService.GetRecentAddedUser().UserId;
                return Ok(new { token = GenerateToken(id, "", "", "Customer") });
            }

            // Co acc
            User user = service.userService.GetUserById(info.UserId);
            return Ok(new { token = GenerateToken(info.UserId, info.LastName, user.UserName, service.roleService.GetRoleById(user.RoleId).RoleName) });
        }

        [HttpPost]
        [Route("User/LoginAuth")]
        public async Task<IActionResult> LoginAuth(string username, string password)
        {
            //User user = service.userService.GetUserByLogin(username, password);
            User user = service.userService.GetAllUsers().FirstOrDefault(x => x.UserName == username && x.Password == password);

            // Nhập đúng username + pass
            if (user != null)
            {
                UserDetail info = service.userDetailService.GetUserDetailById(user.UserId);
                if (user.AccessFail == 0)
                {
                    user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
                    service.userService.UpdateUser(user, user.UserId);
                    return Ok(new { token = GenerateToken(user.UserId, info.LastName, user.UserName, service.roleService.GetRoleById(user.RoleId).RoleName) });

                }   // Legit, nhập đúng từ lần đầu

                else if (user.AccessFail < 3) // Trước đó nhập sai chưa tới lần 3
                {
                    user.AccessFail = 0;
                    user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
                    user.ActiveStatus = true;
                    service.userService.UpdateUser(user, user.UserId);
                    return Ok(new { token = GenerateToken(user.UserId, info.LastName, user.UserName, service.roleService.GetRoleById(user.RoleId).RoleName) });
                }

                else if (user.AccessFail == 3) // Trước đó nhập sai tới lần 3
                {
                    if ((DateTime.Now - user.LastFail).Value.TotalMinutes >= 15)
                    {
                        user.AccessFail = 0;
                        user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
                        user.ActiveStatus = true;
                        service.userService.UpdateUser(user, user.UserId);
                        return Ok(new { token = GenerateToken(user.UserId, info.LastName, user.UserName, service.roleService.GetRoleById(user.RoleId).RoleName) });
                    }
                    return Ok("Temporarily locked");
                }

                else if (user.AccessFail == 4)  // Trước đó nhập sai tới lần 4
                {
                    if ((DateTime.Now - user.LastFail).Value.TotalMinutes >= 30)
                    {
                        user.AccessFail = 0;
                        user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
                        user.ActiveStatus = true;
                        service.userService.UpdateUser(user, user.UserId);
                        return Ok(new { token = GenerateToken(user.UserId, info.LastName, user.UserName, service.roleService.GetRoleById(user.RoleId).RoleName) });
                    }
                    return Ok("Temporarily locked");
                }

                else if (user.AccessFail == 5)  // Trước đó nhập sai tới lần 5
                {
                    if ((DateTime.Now - user.LastFail).Value.TotalHours >= 1)
                    {
                        user.AccessFail = 0;
                        user.LastFail = new DateTime(1900, 1, 1, 0, 0, 0);
                        user.ActiveStatus = true;
                        service.userService.UpdateUser(user, user.UserId);
                        return Ok(new { token = GenerateToken(user.UserId, info.LastName, user.UserName, service.roleService.GetRoleById(user.RoleId).RoleName) });
                    }
                    return Ok("Temporarily locked");
                }

                else return Ok("Locked! Contact Admin");
            }

            else
            {
                User failCase = service.userService.GetAllUsers().FirstOrDefault(x => x.UserName == username);

                if (failCase != null) // Đúng username, sai pass
                {
                    if (failCase.AccessFail <= 5) // Sai dưới 5 lần
                    {
                        failCase.AccessFail += 1;
                        failCase.LastFail = DateTime.Now;
                        if (failCase.AccessFail > 2)
                            failCase.ActiveStatus = false;
                        service.userService.UpdateUser(failCase, failCase.UserId);
                        return Ok("Incorrect username or password!");
                    }
                    return Ok("Locked! Contact Admin"); // Từ lần sai thứ 6 đổ đi đã khóa acc, liên hệ admin để giải quyết
                }
                return NotFound("Not found"); // Sai username, ko quan tâm pass
            }
        }


        [HttpPost]
        [Route("User/ForgetPassword")]
        public async Task<IActionResult> ResetPass(string newPass, string mail)
        {
            User user = service.userService.GetUserById(service.userDetailService.GetUserDetailByMail(mail).UserId);
            if (user.AccessFail <= 5)
            {
                string hashNewPass = ToHashString(newPass);
                if (!hashNewPass.Equals(user.Password))
                {
                    bool status = IsPasswordSecure(newPass);
                    if (status)
                    {
                        user.Password = hashNewPass;
                        service.userService.UpdateUser(user, user.UserId);
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
        public async Task<IActionResult> UpdateUser(int id, string username, string password, int? branchId, int? roleId, string firstName, string lastName, string phone, string email)
        {
            User tmp = service.userService.GetUserById(id);
            if (!username.IsNullOrEmpty())
                tmp.UserName = username;
            if (IsPasswordSecure(password)) // Pass k secure = cook
                tmp.Password = (password); // Lay lai pass cu

            // Check role phân quyền đc update những info nào
            if (roleId != null) // Chỉ staff/Admin mới đc update role và chi nhánh
            {
                if (roleId == 2) // Nếu đc update thành Staff
                {
                    tmp.RoleId = 2;
                    tmp.BranchId = branchId; // -> Staff mới có chi nhánh
                }
                else // Trường hợp còn lại khi đc update Role: Admin
                {
                    tmp.RoleId = int.Parse(roleId.ToString());
                    tmp.BranchId = null; // -> Admun ko cần chi nhánh
                }
            }
            service.userService.UpdateUser(tmp, id);


            // Update xong user chuyển tiếp qua detail
            UserDetail info = service.userDetailService.GetUserDetailById(id);
            if (!firstName.IsNullOrEmpty())
                info.FirstName = firstName;
            if (!lastName.IsNullOrEmpty())
                info.LastName = lastName;
            if (!email.IsNullOrEmpty())
                info.Email = email;
            if (!phone.IsNullOrEmpty())
                if (IsPhoneFormatted(phone))
                    info.Phone = phone;
            service.userDetailService.UpdateUserDetail(info, id);
            return Ok(new
            {
                Username = tmp.UserName,
                Password = tmp.Password,
                FirstName = info.FirstName,
                LastName = info.LastName,
                Email = info.Email,
                Phone = info.Phone
            }
  );
        }


        [HttpPost]
        [Route("User/Register")]
        public async Task<IActionResult> AddUser(string username, string password, string firstName, string lastName, string email, string phone)
        {
            UserDetail info = service.userDetailService.GetAllUserDetails().FirstOrDefault(x => x.Email.Equals(email) && x.Phone.Equals(phone));
            if (info == null)
            {
                if (IsPhoneFormatted(phone))
                {
                        //Hash pass
                        service.userService.AddUser(new User(username, password, null, 3, 0, true, 0, new DateTime(1900, 1, 1, 0, 0, 0)));
                        User user = service.userService.GetRecentAddedUser();
                        service.userDetailService.AddUserDetail(new UserDetail(user.UserId, firstName, lastName, email, phone));
                        return Ok(new { token = GenerateToken(user.UserId, service.userDetailService.GetUserDetailById(user.UserId).LastName, user.UserName, service.roleService.GetRoleById(user.RoleId).RoleName) });
                    return Json("Password is not properly secured");
                }
                return Json("Phone number is not properly formatted");
            }
            return Json("User has already existed");
        }

        [HttpPost]
        [Route("User/RegisterAdmin")]
        public async Task<IActionResult> AddUser(string username, string password, string firstName, string lastName, int role, int branch, string email, string phone)
        {
        	UserDetail info = service.userDetailService.GetAllUserDetails().FirstOrDefault(x => x.Email.Equals(email) && x.Phone.Equals(phone));
        	if (info == null)
        	{
        		if (IsPhoneFormatted(phone))
        		{
        			//Hash pass
        			service.userService.AddUser(new User(username, password, branch, role, null, true, 0, new DateTime(1900, 1, 1, 0, 0, 0)));
        			User user = service.userService.GetRecentAddedUser();
        			service.userDetailService.AddUserDetail(new UserDetail(user.UserId, firstName, lastName, email, phone));
        			return Ok(new { token = GenerateToken(user.UserId, service.userDetailService.GetUserDetailById(user.UserId).LastName, user.UserName, service.roleService.GetRoleById(user.RoleId).RoleName) });
        			return Json("Password is not properly secured");
        		}
        		return Json("Phone number is not properly formatted");
        	}
        	return Json("User has already existed");
        }


        // Xóa account
        // Xóa userdetail -> xóa user: Set ActiveStatus = false
        // Ongoing
        [HttpDelete]
        [Route("User/Delete")]
        //[Authorize(Roles = "")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            service.userService.DeleteUser(id);
            return Ok();
        }

    }
}
