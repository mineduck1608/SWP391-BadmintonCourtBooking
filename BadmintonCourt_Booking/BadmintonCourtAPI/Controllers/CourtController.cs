using BadmintonCourtAPI.Utils;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtBusinessObjects.SupportEntities.Court;
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
		public async Task<ActionResult<IEnumerable<CourtDTO>>> GettAllCourts() => Ok(Util.FormatCourtList(_service.CourtService.GetAllCourts().ToList()));


		[HttpGet]
		[Route("Court/GetDeleted")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<CourtDTO>>> GetDeletedCourts() => Ok();

		[HttpGet]
		[Route("Court/GetById")]
		public async Task<ActionResult<CourtDTO>> GetCourtById(string id)
		{
			Court court = _service.CourtService.GetCourtByCourtId(id);
			string[] components = court.CourtImg.Split('|');
			List<string> courtImg = new List<string>();
			for (int i = 0; i < components.Length; i++)
				courtImg.Add($"Image {i + 1}:{components[i]}");
			return Ok(new CourtDTO
			{
				CourtId = court.CourtId,
				CourtImg = courtImg,
				BranchId = court.BranchId,
				CourtName = court.CourtName,
				CourtStatus = court.CourtStatus,
				Description = court.Description,
				Price = court.Price
			});
		}


		[HttpGet]
		[Route("Court/GetByStatus")]
		public async Task<ActionResult<IEnumerable<Court>>> GetCourtsByStatus(bool status) => Ok(Util.FormatCourtList(_service.CourtService.GetCourtsByStatus(status)));

		[HttpGet]
		[Route("Court/GetByBranch")]
		public async Task<ActionResult<IEnumerable<CourtDTO>>> GetCourtsByBranch(string id) => Ok(Util.FormatCourtList(_service.CourtService.GetCourtsByBranchId(id)));

		[HttpGet]
		[Route("Court/GetByPrice")]
		public async Task<ActionResult<IEnumerable<CourtDTO>>> GetCourtsByPriceInterval(string txtMin, string txtMax)
		{
			try
			{
				double min = string.IsNullOrEmpty(txtMin) ? 0 : Double.Parse(txtMin);
				double max = string.IsNullOrEmpty(txtMax) ? float.MaxValue : Double.Parse(txtMax);
				bool status = Util.ArePricesValid(min, max);
				if (status)
					return Ok(Util.FormatCourtList(_service.CourtService.GetCourtsByPriceInterval(min, max).ToList()));
				return BadRequest("Max must be larger than min");
			}
			catch (Exception ex)
			{
				return BadRequest("Invalid price format | " + ex.Message);
			}
		}

		[HttpGet]
		[Route("Court/GetBySearch")]
		public async Task<ActionResult<IEnumerable<CourtDTO>>> GetCourtsBySearch(string txtSearch) => Ok(Util.FormatCourtList(_service.CourtService.GetCourtsBySearchResult(txtSearch).ToList()));

		[HttpPost]
		[Route("Court/Add")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> AddCourt(string courtImg, string branchId, string? description, float? price)
		{

			_service.CourtService.AddCourt(new Court { CourtId = "C" + (_service.CourtService.GetAllCourts().Count + 1).ToString("D3"), BranchId = branchId, CourtImg = courtImg, Price = price == null ? 20000 : price.Value, CourtStatus = true, Description = description, CourtName = $"Court {_service.CourtService.GetCourtsByBranchId(branchId).Count + 1}" });
			return Ok();
		}

		[HttpPut]
		[Route("Court/Update")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateCourt(string courtImg, string description, string id, bool activeStatus, float? price)
		{
			try
			{
				Court tmp = _service.CourtService.GetCourtByCourtId(id);
				tmp.CourtStatus = activeStatus;
				if (!courtImg.IsNullOrEmpty())
					tmp.CourtImg = courtImg;
				if (!description.IsNullOrEmpty())
					tmp.Description = description;
				if (price != null)
					tmp.Price = price.Value;
				tmp.CourtStatus = activeStatus;
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
