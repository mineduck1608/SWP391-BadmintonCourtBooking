using BadmintonCourtAPI.Utils;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BadmintonCourtAPI.Controllers
{
	public class CourtController : Controller
	{
		private readonly BadmintonCourtService _service = null;

		public CourtController(IConfiguration config)
		{
			if (_service == null)
			{
				_service = new BadmintonCourtService(config);
			}
		}

		[HttpGet]
		[Route("Court/GetAll")]
		//[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<IEnumerable<Court>>> GettAllCourts() => Ok(_service.CourtService.GetAllCourts().ToList());


		[HttpGet]
		[Route("Court/GetDeleted")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<Court>>> GetDeletedCourts() => Ok();

		[HttpGet]
		[Route("Court/GetById")]
		public async Task<ActionResult<Court>> GetCourtById(string id) => Ok(_service.CourtService.GetCourtByCourtId(id));

		[HttpGet]
		[Route("Court/GetActive")]
		public async Task<ActionResult<IEnumerable<Court>>> GetActiveCourts() => Ok(_service.CourtService.GetCourtsByStatus(true));

		[HttpGet]
		[Route("Court/GetByStatus")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<Court>>> GetCourtsByStatus(bool status) => Ok(_service.CourtService.GetCourtsByStatus(status));

		[HttpGet]
		[Route("Court/GetByBranch")]
		public async Task<ActionResult<IEnumerable<Court>>> GetCourtsByBranch(string id) => Ok(_service.CourtService.GetCourtsByBranchId(id));

		[HttpGet]
		[Route("Court/GetByPrice")]
		public async Task<ActionResult<IEnumerable<Court>>> GetCourtsByPriceInterval(string txtMin, string txtMax)
		{
			try
			{
				double min = string.IsNullOrEmpty(txtMin) ? 0 : Double.Parse(txtMin);
				double max = string.IsNullOrEmpty(txtMax) ? float.MaxValue : Double.Parse(txtMax);
				bool status = Util.ArePricesValid(min, max);
				if (status)
					return Ok(_service.CourtService.GetCourtsByPriceInterval(min, max).ToList());
				return BadRequest("Max must be larger than min");
			}
			catch (Exception ex)
			{
				return BadRequest("Invalid price format | " + ex.Message);
			}
		}

		[HttpGet]
		[Route("Court/GetBySearch")]
		public async Task<ActionResult<IEnumerable<Court>>> GetCourtsBySearch(string txtSearch) => Ok(_service.CourtService.GetCourtsBySearchResult(txtSearch).ToList());

		[HttpPost]
		[Route("Court/Add")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> AddCourt(string courtImg, string branchId, string description)
		{
			
			_service.CourtService.AddCourt(new Court { CourtId = "C" + (_service.CourtService.GetAllCourts().Count + 1).ToString("D3"), BranchId = branchId, CourtImg = courtImg, Price = _service.CourtService.GetCourtByCourtId("C1").Price, CourtStatus = true, Description = description, CourtName = $"Court {_service.CourtService.GetCourtsByBranchId(branchId).Count + 1}"  });
			return Ok();
		}

		[HttpPut]
		[Route("Court/Update")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateCourt(string courtImg, string description, string id, bool activeStatus)
		{
			try
			{
				Court tmp = _service.CourtService.GetCourtByCourtId(id);
				tmp.CourtStatus = activeStatus;
				if (!courtImg.IsNullOrEmpty())
					tmp.CourtImg = courtImg;
				if (!description.IsNullOrEmpty())
					tmp.Description = description;
				_service.CourtService.UpdateCourt(tmp, id);
				return Ok(new { msg = "Success" });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
