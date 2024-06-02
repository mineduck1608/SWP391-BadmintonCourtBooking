using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace BadmintonCourtAPI.Controllers
{
    public class UserController : Controller
    {
        private readonly BadmintonCourtService service = null;

        public UserController()
        {
            if (service == null)
            {
                service = new BadmintonCourtService();
            }
        }

        public bool IsPasswordSecure(string password) => new Regex(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=<>?])[A-Za-z\d!@#$%^&*()_\-+=<>?]{12,}").IsMatch(password);
        public bool IsPhoneFormatted(string phone) => new Regex(@"\d{9,11}").IsMatch(phone);

        [HttpGet]
        [Route("User/GetAll")]
        public async Task<IEnumerable<User>> GetAllUsers() => service.userService.GetAllUsers().ToList();

        [HttpGet]
        [Route("User/GetStaffsInBranch")]
        public async Task<IEnumerable<User>> GetStaffsByBranch(int id) => service.userService.GetStaffsByBranch(id).ToList();

        [HttpGet]
        [Route("User/GetByRole")]
        public async Task<IEnumerable<User>> GetUsersByRole(int id) => service.userService.GetUsersByRole(id).ToList();

        [HttpGet]
        [Route("User/GetById")]
        public async Task<ActionResult<User>> GetUserByUserId(int id) => service.userService.GetUserById(id);


        [HttpPost]
        [Route("User/ExternalLogAuth")]
        public async Task<ActionResult<string>> ExternalAuth(string email) => service.userDetailService.GetUserDetailByMail(email) != null ? $"{service.userService.GetUserById(service.userDetailService.GetUserDetailByMail(email).UserId).RoleId}" : "";


        /*service.userService.GetUserById(service.userDetailService.GetUserDetailByMail(email).UserId).RoleId.ToString();*/

        [HttpPost]
        [Route("User/LoginAuth")]
        public IActionResult LoginAuth(string id, string password) => service.userService.GetAllUsers().FirstOrDefault(x => x.UserName.Equals(id) && x.Password.Equals(password)) != null ? Json(new
        {
            service.userService.GetAllUsers().FirstOrDefault(x => x.UserName.Equals(id) && x.Password.Equals(password)).RoleId,
            id,
            password
        }) : Json("0");

        /* $"{service.userService.GetAllUsers().FirstOrDefault(x => x.UserName.Equals(userName) && x.Password.Equals(userName) && x.UserDetail.Email.Equals(email))}";*/



        [HttpPut]
        [Route("User/Update")]
        public async Task<IActionResult> UpdateUser(int id, string username, string password, int? branchId, int? roleId, int? balence, string firstName, string lastName, string phone, string email)
        {
            User tmp = service.userService.GetUserById(id);
            if (!username.IsNullOrEmpty())
                tmp.UserName = username;
            if (!password.IsNullOrEmpty())
                tmp.Password = password;

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
            service.userDetailService.UpdateUserDetail(new UserDetail(id, firstName, lastName, email, phone), id);
            return Ok();


            //return RedirectToAction("UpdateUserDetail", "UserDetail", new UserDetail(id, firstName, lastName, email, phone));
        }


        [HttpPost]
        [Route("User/Register")]
        public async Task<IActionResult> AddUser(string username, string password, string firstName, string lastName, string email, string phone)
        {
            if (IsPhoneFormatted(phone))
            {
                UserDetail infoStorage = service.userDetailService.GetAllUserDetails().FirstOrDefault(x => x.Email.Equals(email) && x.Phone.Equals(phone));
                if (infoStorage == null)
                {
                    service.userService.AddUser(new User(username, password, null, 3, 0));
                    service.userDetailService.AddUserDetail(new UserDetail(service.userService.GetRecentAddedUser().UserId, firstName, lastName, email, phone));
                    return Json(new
                    {
                        service.userService.GetRecentAddedUser().RoleId
                    });

                }
                return Json("0");
            }
            return Json("The phone number is not properly formatted");
        }


        // Xóa account
        // Xóa userdetail -> xóa user
        [HttpDelete]
        [Route("User/Delete")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            service.userService.DeleteUser(id);
            return Ok();
        }

        [HttpPost]
        [Route("User/ForgetPassword")]
        public IActionResult ResetPass(string newPass, string mail)
        {
            User user = service.userService.GetUserById(service.userDetailService.GetUserDetailByMail(mail).UserId);
            if (!newPass.Equals(user.Password))
            {
                bool status = IsPasswordSecure(newPass);
                if (status)
                {
                    user.Password = newPass;
                    service.userService.UpdateUser(user, user.UserId);
                    return Json("");
                }
                return Json("");
            }
            return Json("");

        }
    }
}
