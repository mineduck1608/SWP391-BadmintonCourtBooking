using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BadmintonCourtAPI.Utils;

namespace BadmintonCourtAPI.Controllers
{
    public class RoleController : Controller
    {
        private readonly BadmintonCourtService service = null;

        public RoleController(IConfiguration config)
        {
            if (service == null)
            {
                service = new BadmintonCourtService(config);
            }
        }


		[HttpGet]
        [Route("Role/GetAll")]
        //[Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<Role>>> GetAllRoles() =>
            Ok(service.roleService.GetAllRoles());

        [HttpGet]
        [Route("Role/GetByName")]
        public async Task<IEnumerable<Role>> GetRolesByName(string name) => service.roleService.GetRolesByName(name).ToList();

		[HttpGet]
		[Route("Role/GetById")]
		public async Task<ActionResult<Role>> GetRoleById(string id) => service.roleService.GetRoleById(id);

		[HttpPost]
		[Route("Role/AddRole")]
		public async Task<IActionResult> AddRole(string roleName)
        {
            if (roleName.IsNullOrEmpty())
            {
				service.roleService.AddRole(new Role { RoleId = Util.GenerateRoleId(service), RoleName = roleName });
				return Ok();
			}
            return BadRequest("Name is not full filled yet");
        }

		[HttpDelete]
		[Route("Role/Delete")]
		public async Task<IActionResult> DeleteRole(string id)
        {
            service.roleService.DeleteRole(id);
            return Ok();    
        }

        [HttpPut]
		[Route("Role/Update")]
		public async Task<IActionResult> UpdateRole(string roleName, string id)
        {
            Role tmp = service.roleService.GetRoleById(id);       
            if (!roleName.IsNullOrEmpty())
                tmp.RoleName = roleName;
            service.roleService.UpdateRole(tmp, id);
            return Ok();
        }

    }
}
