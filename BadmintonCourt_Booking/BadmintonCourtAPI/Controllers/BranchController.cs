using BadmintonCourtAPI.Utils;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtBusinessObjects.SupportEntities.Court;
using BadmintonCourtServices;
using BadmintonCourtServices.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace BadmintonCourtAPI.Controllers
{
	public class BranchController : Controller
	{

		private readonly ICourtBranchService _service = null;
		private readonly ICourtService _courtService = new CourtService();

		public BranchController(ICourtBranchService service)
		{
			_service = service;
		}

		[HttpPost]
		[Route("Branch/Add")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CourtBranch>> AddBranch(string location, string img, string name, string phone, string mapUrl)
		{
			if (location.IsNullOrEmpty())
				return BadRequest();
			if (name.IsNullOrEmpty())
				return BadRequest();
			if (Util.IsPasswordSecure(phone))
			{
				_service.AddBranch(new CourtBranch { BranchId = "B" + (_service.GetAllCourtBranches().Count + 1).ToString("D3"), BranchImg = img, BranchName = name, Location = location, BranchPhone = phone, BranchStatus = 1, MapUrl = mapUrl });
				// 1: hoạt động
				// 0: bỏ
				// -1: bảo trì
				return Ok(new { msg = "Success" });
			}
			return BadRequest();
		}


		[HttpDelete]
		[Route("Branch/Delete")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CourtBranch>> DeleteBranch(string id)
		{
			_service.DeleteBranch(id);
			return Ok(new { msg = "Success" });
		}


		[HttpGet]
		[Route("Branch/GetAll")]
		public async Task<ActionResult<IEnumerable<BranchDTO>>> GetAllBranches() => Ok(Util.FormatBranchList(_service.GetAllCourtBranches()));

		[HttpGet]
		[Route("Branch/GetBySearch")]
		public async Task<ActionResult<IEnumerable<BranchDTO>>> GetBranchesBySearchResult(string search) => Ok(Util.FormatBranchList(_service.GetBranchesBySearchResult(search)));


		[HttpPut]
		[Route("Branch/Update")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CourtBranch>> UpdateBranch(string location, string img, string name, string phone, int status, string id, string mapUrl)
		{
			CourtBranch branch = _service.GetBranchById(id);
			if (!location.IsNullOrEmpty())
				branch.Location = location;
			if (!name.IsNullOrEmpty())
				branch.BranchName = name;
			if (!img.IsNullOrEmpty())
				branch.BranchImg = img;
			if (!mapUrl.IsNullOrEmpty())
				branch.MapUrl = mapUrl;
			if (Util.IsPhoneFormatted(phone))
				branch.BranchPhone = phone;
			branch.BranchStatus = status;
			if (status == -1 || status == 0)
			{
				List<Court> courtList = _courtService.GetCourtsByBranchId(id);
				if (courtList.Count > 0 || courtList != null)
				{
					foreach (var item in courtList)
					{
						item.CourtStatus = false;
						_courtService.UpdateCourt(item, item.CourtId);
					}
				}
			}
			_service.UpdateBranch(branch, id);
			return Ok(new { msg = "Success" });
		}

	}
}
