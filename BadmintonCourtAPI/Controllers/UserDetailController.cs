using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BadmintonCourtAPI.Controllers
{
    public class UserDetailController : Controller
    {
        private readonly BadmintonCourtService service = null;

        public UserDetailController()
        {
            if (service == null)
            {
                service = new BadmintonCourtService();
            }
        }

        [HttpGet]
        [Route("UserDetail/GetAll")]
        public async Task<IEnumerable<UserDetail>> GetAllUserDetails() => service.userDetailService.GetAllUserDetails();

        [HttpGet]
        [Route("UserDetail/GetByName")]
        public async Task<IEnumerable<UserDetail>> GetUserDetailsByName(string name) =>
			service.userDetailService.GetUserDetailsByName(name);

        [HttpGet]
        [Route("UserDetail/GetBySearch")]
        public async Task<IEnumerable<UserDetail>> GetUserDetailsBySearch(string search) => search.IsNullOrEmpty() == true ? service.userDetailService.GetAllUserDetails().ToList() : service.userDetailService.GetUserDetailsBySearchResult(search).ToList();

       
        [HttpPost]
        [Route("UserDetail/Register")]
        public async Task<IActionResult> AddUserDetail(UserDetail userDetail)
        {
			service.userDetailService.AddUserDetail(userDetail);
            return Ok();
        }

        [HttpPut]
        [Route("UserDetail/Update")]
        public async Task<IActionResult> UpdateUserDetail(UserDetail userDetail)
        {
			service.userDetailService.UpdateUserDetail(userDetail, userDetail.UserId);
            return Ok();
        }

        // Xóa account
        // Xóa userdetail -> xóa user
        [HttpDelete]
        [Route("UserDetail/Delete")]
        public async Task<IActionResult> DeleteUserDetail(int id)
        {
            service.userDetailService.DeleteUserDetail(id);
            return RedirectToAction("DeleteUser", "User", id);
        }

    }
}
