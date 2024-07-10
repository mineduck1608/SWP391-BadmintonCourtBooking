using BadmintonCourtAPI.Utils;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtBusinessObjects.SupportEntities.Court;
using BadmintonCourtDAOs;
using BadmintonCourtServices;
using BadmintonCourtServices.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BadmintonCourtAPI.Controllers
{
	public class CourtController : Controller
	{
		private readonly ICourtService _service = null;

		public CourtController(ICourtService service)
		{
			_service = service;
		}

		[HttpGet]
		[Route("Court/GetAll")]
		public async Task<ActionResult<IEnumerable<Court>>> GettAllCourts() => Ok(_service.GetAllCourts());


		//[HttpGet]
		//[Route("Court/GetDeleted")]
		////[Authorize(Roles = "Admin")]
		//public async Task<ActionResult<IEnumerable<CourtDTO>>> GetDeletedCourts() => Ok();

		//[HttpGet]
		//[Route("Court/GetById")]
		//public async Task<ActionResult<CourtDTO>> GetCourtById(string id)
		//{
		//	Court court = _service.GetCourtByCourtId(id);
		//	string[] components = court.CourtImg.Split('|');
		//	List<string> courtImg = new List<string>();
		//	for (int i = 0; i < components.Length; i++)
		//		courtImg.Add($"Image {i + 1}:{components[i]}");
		//	return Ok(new CourtDTO
		//	{
		//		CourtId = court.CourtId,
		//		CourtImg = courtImg,
		//		BranchId = court.BranchId,
		//		CourtName = court.CourtName,
		//		CourtStatus = court.CourtStatus,
		//		Description = court.Description,
		//		Price = court.Price
		//	});
		//}


		[HttpGet]
		[Route("Court/GetById")]
		public async Task<ActionResult<Court>> GetCourtById(string id) => Ok(_service.GetCourtByCourtId(id));

		[HttpGet]
		[Route("Court/GetByStatus")]
		public async Task<ActionResult<IEnumerable<Court>>> GetCourtsByStatus(bool status) => Ok(_service.GetCourtsByStatus(status));

		[HttpGet]
		[Route("Court/GetByBranch")]
		public async Task<ActionResult<IEnumerable<Court>>> GetCourtsByBranch(string id) => Ok(_service.GetCourtsByBranchId(id));

		[HttpGet]
		[Route("Court/GetByPrice")]
		public async Task<ActionResult<IEnumerable<Court>>> GetCourtsByPriceInterval(string? txtMin, string? txtMax)
		{
			try
			{
				double min = string.IsNullOrEmpty(txtMin) ? 0 : Double.Parse(txtMin);
				double max = string.IsNullOrEmpty(txtMax) ? float.MaxValue : Double.Parse(txtMax);
				bool status = Util.ArePricesValid(min, max);
				if (status)
					return Ok(_service.GetCourtsByPriceInterval(min, max));
				return BadRequest(new { msg = "Max must be larger than min"});
			}
			catch (Exception ex)
			{
				return BadRequest("Invalid price format | " + ex.Message);
			}
		}

		[HttpGet]
		[Route("Court/GetBySearch")]
		public async Task<ActionResult<IEnumerable<Court>>> GetCourtsBySearch(string txtSearch) => Ok(Util.FormatCourtList(_service.GetCourtsBySearchResult(txtSearch).ToList()));

		[HttpPost]
		[Route("Court/Add")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> AddCourt(string? courtImg, string branchId, string? description, float? price)
		{
			_service.AddCourt(new Court { CourtId = "C" + (_service.GetAllCourts().Count + 1).ToString("D3"), BranchId = branchId, CourtImg = courtImg.IsNullOrEmpty() ? "" : courtImg, Price = price == null || price <= 0 ? 20000 : price.Value, CourtStatus = true, Description = description.IsNullOrEmpty() ? "" : description, CourtName = $"Court {_service.GetCourtsByBranchId(branchId).Count + 1}" });
			return Ok(new { msg = "Success"});
		}

		[HttpPut]
		[Route("Court/Update")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<IActionResult> UpdateCourt(string courtImg, string description, string id, bool activeStatus, float? price)
		{
			try
			{
				Court tmp = _service.GetCourtByCourtId(id);
				tmp.CourtStatus = activeStatus;
				if (!courtImg.IsNullOrEmpty())
					tmp.CourtImg = courtImg;
				if (!description.IsNullOrEmpty())
					tmp.Description = description;
				if (price != null)
					tmp.Price = price < 0 ? tmp.Price : price.Value;
				
				tmp.CourtStatus = activeStatus;
				_service.UpdateCourt(tmp, id);
				return Ok(new { msg = "Success" });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
