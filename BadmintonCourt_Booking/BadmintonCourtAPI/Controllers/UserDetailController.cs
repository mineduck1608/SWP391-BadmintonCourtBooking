using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BadmintonCourtAPI.Controllers
{
    public class UserDetailController : Controller
    {
        private readonly BadmintonCourtService _service = null;

        public UserDetailController(IConfiguration config)
        {
            if (_service == null)
            {
                _service = new BadmintonCourtService(config);
            }
        }

        [HttpGet]
        [Route("UserDetail/GetAll")]
        public async Task<IEnumerable<UserDetail>> GetAllUserDetails() => _service.UserDetailService.GetAllUserDetails();

        [HttpGet]
        [Route("UserDetail/GetByName")]
        public async Task<IEnumerable<UserDetail>> GetUserDetailsByName(string name) =>
			_service.UserDetailService.GetUserDetailsByName(name);

        [HttpGet]
        [Route("UserDetail/GetBySearch")]
        public async Task<IEnumerable<UserDetail>> GetUserDetailsBySearch(string search) => search.IsNullOrEmpty() == true ? _service.UserDetailService.GetAllUserDetails().ToList() : _service.UserDetailService.GetUserDetailsBySearchResult(search).ToList();

       
        [HttpPost]
        [Route("UserDetail/Register")]
        public async Task<IActionResult> AddUserDetail(UserDetail userDetail)
        {
			_service.UserDetailService.AddUserDetail(userDetail);
            return Ok();
        }

        [HttpPut]
        [Route("UserDetail/Update")]
        public async Task<IActionResult> UpdateUserDetail(UserDetail userDetail)
        {
			_service.UserDetailService.UpdateUserDetail(userDetail, userDetail.UserId);
            return Ok();
        }

        // Xóa account
        // Xóa userdetail -> xóa user
        [HttpDelete]
        [Route("UserDetail/Delete")]
        public async Task<IActionResult> DeleteUserDetail(string id)
        {
            _service.UserDetailService.DeleteUserDetail(id);
            return RedirectToAction("DeleteUser", "User", id);
        }

    }
}
