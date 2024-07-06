using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BadmintonCourtAPI.Utils;
using BadmintonCourtDAOs;
using BadmintonCourtServices.IService;

namespace BadmintonCourtAPI.Controllers
{
	public class RoleController : Controller
	{
		private readonly IRoleService _service = null;

		//public RoleController(IConfiguration config)
		//{
		//	if (_service == null)
		//	{
		//		_service = new BadmintonCourtService(config);
		//	}
		//}


		public RoleController(IRoleService service)
		{
			_service = service;
		}


		[HttpGet]
		[Route("Role/GetAll")]
		//[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<IEnumerable<Role>>> GetAllRoles() =>
			Ok(_service.GetAllRoles());

		[HttpGet]
		[Route("Role/GetByName")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<IEnumerable<Role>>> GetRolesByName(string name) => Ok(_service.GetRolesByName(name).ToList());

		[HttpGet]
		[Route("Role/GetById")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<Role>> GetRoleById(string id) => Ok(_service.GetRoleById(id));

		[HttpPost]
		[Route("Role/Add")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> AddRole(string roleName)
		{
			if (!roleName.IsNullOrEmpty())
			{
				if (_service.GetAllRoles().FirstOrDefault(x => x.RoleName == roleName) != null)
					return BadRequest(new { msg = $"Role {roleName} has already existed" });
				_service.AddRole(new Role { RoleId = "R" + (_service.GetAllRoles().Count + 1).ToString("D3"), RoleName = roleName });
				return Ok(new { msg = "Success" });
			}
			return BadRequest(new { msg = "Name is not full filled yet" });
		}

		[HttpDelete]
		[Route("Role/Delete")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteRole(string id)
		{
			_service.DeleteRole(id);
			return Ok(new { msg = "Success" });
		}

		[HttpPut]
		[Route("Role/Update")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateRole(string roleName, string id)
		{
			Role tmp = _service.GetRoleById(id);
			if (!roleName.IsNullOrEmpty())
			{
				if (_service.GetAllRoles().FirstOrDefault(x => x.RoleName == roleName) != null)
					return BadRequest(new { msg = $"Role {roleName} has already existed" });
				tmp.RoleName = roleName;
			}
			_service.UpdateRole(tmp, id);
			return Ok(new { msg = "Success" });
		}

	}
}
