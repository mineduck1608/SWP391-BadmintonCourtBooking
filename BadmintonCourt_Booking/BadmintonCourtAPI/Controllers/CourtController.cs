using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
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

        [HttpGet]
        [Route("Court/GetAll")]
        public async Task<IEnumerable<Court>> GettAllCourts() => service.courtService.GetAllCourts().ToList();

        [HttpGet]
        [Route("Court/GetById")]
        public async Task <ActionResult<Court>> GetCourtById(int id)
        {
            service.courtService.GetCourtByCourtId(id);
            return NoContent(); 
        }

        [HttpGet]
        [Route("Court/GetByBranch")]
        public async Task<IEnumerable<Court>> GetCourtsByBranch(int id) => service.courtService.GetCourtsByBranchId(id).ToList();

        [HttpGet]
        [Route("Court/GetByPrice")]
        public async Task<IEnumerable<Court>> GetCourtsByPriceInterval(string txtMin, string txtMax)
        {
            try
            {
                float? min = float.Parse(txtMin);
                float? max = float.Parse(txtMax);
                bool status = ArePricesValid(min, max);
                if (status || min == null || max == null)
                    return service.courtService.GetCourtsByPriceInterval(min, max).ToList();             
            } catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        [HttpGet]
        [Route("Court/GetBySearch")]
        public async Task<IEnumerable<Court>> GetCourtsBySearch(string txtSearch) => service.courtService.GetCourtsBySearchResult(txtSearch).ToList();

        [HttpPost]
        [Route("Court/GetByBranch")]
        public async Task<ActionResult<Court>> AddCourt(string courtImg, int branchId, double price, string description)
        {
            service.courtService.AddCourt(new Court(1, courtImg, branchId, price, description));

            // Tao san mac dinh tu tao slot 
            return RedirectToAction("AddDSlot", "Slot", new Slot(1, service.slotService.GetSlotById(1).StartTime, service.slotService.GetSlotById(1).EndTime, true, service.courtService.GetRecentAddedCourt().CourtId, null));
        }

        //[Route("Court/Delete/id")]
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
