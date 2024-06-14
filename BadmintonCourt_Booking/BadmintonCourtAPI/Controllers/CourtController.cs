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
		private readonly BadmintonCourtService service = null;

		public CourtController(IConfiguration config)
		{
			if (service == null)
			{
				service = new BadmintonCourtService(config);
			}
		}

		//public void AddDefaultSlot(Slot slot) => service.slotService.AddSlot(slot);

		[HttpGet]
		[Route("Court/GetAll")]
		public async Task<ActionResult<IEnumerable<Court>>> GettAllCourts() => Ok(service.courtService.GetAllCourts().ToList());

		[HttpGet]
		[Route("Court/GetDeleted")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<Court>>> GetDeletedCourts() => Ok();

		[HttpGet]
		[Route("Court/GetById")]
		public async Task<ActionResult<Court>> GetCourtById(string id) => Ok(service.courtService.GetCourtByCourtId(id));

		[HttpGet]
		[Route("Court/GetByBranch")]
		public async Task<ActionResult<IEnumerable<Court>>> GetCourtsByBranch(string id) => Ok(service.courtService.GetCourtsByBranchId(id));

		[HttpGet]
		[Route("Court/GetByPrice")]
		public async Task<ActionResult<IEnumerable<Court>>> GetCourtsByPriceInterval(string txtMin, string txtMax)
		{
			try
			{
				float min = string.IsNullOrEmpty(txtMin) ? 0 : float.Parse(txtMin);
				float max = string.IsNullOrEmpty(txtMax) ? float.MaxValue : float.Parse(txtMax);
				bool status = Util.ArePricesValid(min, max);
				if (status)
					return Ok(service.courtService.GetCourtsByPriceInterval(min, max).ToList());
				return BadRequest("Max must be larger than min");
			}
			catch (Exception ex)
			{
				return BadRequest("Invalid price format | " + ex.Message);
			}
		}

		[HttpGet]
		[Route("Court/GetBySearch")]
		public async Task<ActionResult<IEnumerable<Court>>> GetCourtsBySearch(string txtSearch) => Ok(service.courtService.GetCourtsBySearchResult(txtSearch).ToList());

		[HttpPost]
		[Route("Court/Add")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult<Court>> AddCourt(string courtImg, string branchId, float price, string description)
		{
			service.courtService.AddCourt(new Court { CourtId = Util.GenerateCourtId(service), BranchId = branchId, CourtImg = courtImg, Price = price, CourtStatus = true, Description = description });
			Slot primitive = service.slotService.GetSlotById("S1");
			return Ok();
		}

		[HttpDelete]
		[Route("Court/Delete")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult<Court>> DeleteCourt(string id) // Xóa sân -> Xóa luôn slot 
		{
			// Xóa toàn bộ các slot trống của sân
			// Chỉ tạm giữ lại sự hiện diện của các slot mà khách đa đặt cho khách chơi xong thời hạn đặt 
			//List<Slot> slotsInCourt = service.slotService.GetSlotsByCourt(id).Where(x => x.Status == true).ToList();
			//foreach (Slot slot in slotsInCourt)
				// int slotId = slot.slotId;
			//	service.slotService.DeleteSlot(slot.SlotId);
			//service.courtService.DeleteCourt(id);
			return Ok();
		}

		[HttpPut]
		[Route("Court/Update")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult<Court>> UpdateCourt(string courtImg, float price, string description, string id, bool activeStatus)
		{
			try
			{
				Court tmp = service.courtService.GetCourtByCourtId(id);
				tmp.CourtStatus = activeStatus;
				if (!courtImg.IsNullOrEmpty())
					tmp.CourtImg = courtImg;
				if (!description.IsNullOrEmpty())
					tmp.Description = description;
				if (price != null)
					if (Util.ArePricesValid(0, price))
						tmp.Price = price;
				service.courtService.UpdateCourt(tmp, id);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
