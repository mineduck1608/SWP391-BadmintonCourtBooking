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
		private readonly BadmintonCourtService _service = null;

		public RoleController(IConfiguration config)
		{
			if (_service == null)
			{
				_service = new BadmintonCourtService(config);
			}
		}


		[HttpGet]
		[Route("Role/GetAll")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<IEnumerable<Role>>> GetAllRoles() =>
			Ok(_service.RoleService.GetAllRoles());

		[HttpGet]
		[Route("Role/GetByName")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<IEnumerable<Role>> GetRolesByName(string name) => _service.RoleService.GetRolesByName(name).ToList();

		[HttpGet]
		[Route("Role/GetById")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<Role>> GetRoleById(string id) => _service.RoleService.GetRoleById(id);

		[HttpPost]
		[Route("Role/Add")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> AddRole(string roleName)
		{
			if (!roleName.IsNullOrEmpty())
			{
				if (_service.RoleService.GetAllRoles().FirstOrDefault(x => x.RoleName == roleName) != null)
					return BadRequest(new { msg = $"Role {roleName} has already existed" });
				_service.RoleService.AddRole(new Role { RoleId = "R" + (_service.RoleService.GetAllRoles().Count + 1).ToString("D3"), RoleName = roleName });
				return Ok(new { msg = "Success" });
			}
			return BadRequest(new { msg = "Name is not full filled yet" });
		}

		[HttpDelete]
		[Route("Role/Delete")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteRole(string id)
		{
			_service.RoleService.DeleteRole(id);
			return Ok(new { msg = "Success" });
		}

		[HttpPut]
		[Route("Role/Update")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateRole(string roleName, string id)
		{
			Role tmp = _service.RoleService.GetRoleById(id);
			if (!roleName.IsNullOrEmpty())
			{
				if (_service.RoleService.GetAllRoles().FirstOrDefault(x => x.RoleName == roleName) != null)
					return BadRequest(new { msg = $"Role {roleName} has already existed" });
				tmp.RoleName = roleName;
			}
			_service.RoleService.UpdateRole(tmp, id);
			return Ok(new { msg = "Success" });
		}

	}
}
