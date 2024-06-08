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

        public CourtController()
        {
            if (service == null)
            {
                service = new BadmintonCourtService();
            }
        }


        public bool ArePricesValid(float? min, float? max) => min < max;

		public void AddDefaultSlot(Slot slot) => service.slotService.AddSlot(slot);

		[HttpGet]
        [Route("Court/GetAll")]
        public async Task<ActionResult<IEnumerable<Court>>> GettAllCourts() => Ok(service.courtService.GetAllCourts().ToList());

        [HttpGet]
        [Route("Court/GetDeleted")]
        //[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<Court>>> GetDeletedCourts() => Ok();

		[HttpGet]
        [Route("Court/GetById")]
        public async Task<ActionResult<Court>> GetCourtById(int id) => Ok(service.courtService.GetCourtByCourtId(id));

        [HttpGet]
        [Route("Court/GetByBranch")]
        public async Task<ActionResult<IEnumerable<Court>>> GetCourtsByBranch(int id) => Ok(service.courtService.GetCourtsByBranchId(id));

        [HttpGet]
        [Route("Court/GetByPrice")]
        public async Task<ActionResult<IEnumerable<Court>>> GetCourtsByPriceInterval(string txtMin, string txtMax)
        {
            try
            {
                float? min = float.Parse(txtMin);
                float? max = float.Parse(txtMax);
                bool status = ArePricesValid(min, max);
                if (status || min == null || max == null)
                    return Ok(service.courtService.GetCourtsByPriceInterval(min, max).ToList());             
            } catch (Exception ex)
            {
            }
            return Ok();
        }

        [HttpGet]
        [Route("Court/GetBySearch")]
        public async Task<ActionResult<IEnumerable<Court>>> GetCourtsBySearch(string txtSearch) => Ok(service.courtService.GetCourtsBySearchResult(txtSearch).ToList());

        [HttpPost]
        [Route("Court/GetByBranch")]
        public async Task<ActionResult<Court>> AddCourt(string courtImg, int branchId, double price, string description)
        {
            service.courtService.AddCourt(new Court(courtImg, branchId, price, description));

            // Tao san mac dinh tu tao slot 
            AddDefaultSlot(new Slot(service.slotService.GetSlotById(1).StartTime, service.slotService.GetSlotById(1).EndTime, true, service.courtService.GetRecentAddedCourt().CourtId, null));
            return Ok();
        }

        //[HttpDelete]
        //[Route("Court/Delete")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<Court>> DeleteCourt(int id) // Xóa sân -> Xóa luôn slot 
        {
            // Xóa toàn bộ các slot trống của sân
            // Chỉ tạm giữ lại sự hiện diện của các slot mà khách đa đặt cho khách chơi xong thời hạn đặt 
            List<Slot> slotsInCourt = service.slotService.GetSlotsByCourt(id).Where(x => x.Status == true).ToList();
            foreach (Slot slot in slotsInCourt)
				// int slotId = slot.slotId;
				service.slotService.DeleteSlot(slot.SlotId);
            service.courtService.DeleteCourt(id);
            return Ok();
        }

        //[HttpPut]
        //[Route("Court/Update")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<Court>> UpdateCourt(string courtImg, int? price, string description, int id)
        {
            Court tmp = service.courtService.GetCourtByCourtId(id);
            if (!courtImg.IsNullOrEmpty())
                tmp.CourtImg = courtImg;
            if (price !=  null) 
                tmp.Price = int.Parse(price.ToString());
            if (!description.IsNullOrEmpty())
                tmp.Description = description;
            service.courtService.UpdateCourt(tmp, id);
            return Ok();
        }
    }
}
