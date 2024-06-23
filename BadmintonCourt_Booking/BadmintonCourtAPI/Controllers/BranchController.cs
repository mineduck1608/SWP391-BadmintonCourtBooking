using BadmintonCourtAPI.Utils;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace BadmintonCourtAPI.Controllers
{
	public class BranchController : Controller
	{

		private readonly BadmintonCourtService _service = null;

		public BranchController(IConfiguration config)
		{
			_service = new BadmintonCourtService(config);
		}

		[HttpPost]
		[Route("Branch/Add")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CourtBranch>> AddBranch(string location, string img, string name, string phone)
		{
			if (location.IsNullOrEmpty())
				return BadRequest();
			if (name.IsNullOrEmpty())
				return BadRequest();
			if (Util.IsPasswordSecure(phone))
			{
				_service.BranchService.AddBranch(new CourtBranch { BranchId = "B" + (_service.BranchService.GetAllCourtBranches().Count + 1).ToString("D3"), BranchImg = img, BranchName = name, Location = location, BranchPhone = phone, BranchStatus = 1 });
				// 1: hoạt động
				// 0: bỏ
				// -1: bảo trì
				return Ok(new { msg = "Success" });
			}
			return BadRequest();
		}


		[HttpDelete]
		[Route("Branch/Delete")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CourtBranch>> DeleteBranch(string id)
		{
			_service.BranchService.DeleteBranch(id);
			return NoContent();
		}


		[HttpGet]
		[Route("Branch/GetAll")]
		//[Authorize(Roles = "Admin,Staff")]
		public async Task<IEnumerable<CourtBranch>> GetAllBranches() => _service.BranchService.GetAllCourtBranches().ToList();

		[HttpGet]
		[Route("Branch/GetBySearch")]
		public async Task<IEnumerable<CourtBranch>> GetBranchesBySearchResult(string search) => _service.BranchService.GetBranchesBySearchResult(search);


		[HttpPut]
		[Route("Branch/Update")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CourtBranch>> UpdateBranch(string location, string img, string name, string phone, int status, string id)
		{
			CourtBranch branch = _service.BranchService.GetBranchById(id);
			if (!location.IsNullOrEmpty())
				branch.Location = location;
			if (!name.IsNullOrEmpty())
				branch.BranchName = name;
			if (!img.IsNullOrEmpty())
				branch.BranchImg = img;
			if (Util.IsPhoneFormatted(phone))
				branch.BranchPhone = phone;
			branch.BranchStatus = status;
			if (status == -1 || status == 0)
			{
				List<Court> courtList = _service.CourtService.GetCourtsByBranchId(id);
				if (courtList.Count > 0 || courtList != null)
				{
					foreach (var item in courtList)
					{
						item.CourtStatus = false;
						_service.CourtService.UpdateCourt(item, item.CourtId);
					}
				}
			}
			_service.BranchService.UpdateBranch(branch, id);
			return Ok(new { msg = "Success" });
		}

	}
}
