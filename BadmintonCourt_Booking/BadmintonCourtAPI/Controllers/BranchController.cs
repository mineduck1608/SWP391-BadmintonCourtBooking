using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BadmintonCourtAPI.Controllers
{
    public class BranchController : Controller
    {

        private readonly BadmintonCourtService service = null;

        public BranchController() 
        {
            service = new BadmintonCourtService();
        }

        public bool IsPhoneFormatted(string phone) => new Regex(@"\d{9,11}").IsMatch(phone);


        [HttpPost]
        [Route("Branch/Add")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<CourtBranch>> AddBranch(CourtBranch courtBranch)
        {
            bool status = IsPhoneFormatted(courtBranch.BranchPhone);
            if (status)
            {
                service.courtBranchService.AddBranch(courtBranch);
                return Ok();
            }
            return BadRequest("Phone number is not properly formatted");
        }


        [HttpDelete]
        [Route("Branch/Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CourtBranch>> DeleteBranch(int id)
        {
            service.courtBranchService.DeleteBranch(id);
            return NoContent();
        }


        [HttpGet]
        [Route("Branch/GetAll")]
        [Authorize]
        public async Task<IEnumerable<CourtBranch>> GetAllBranches() => service.courtBranchService.GetAllCourtBranches().ToList();


        [HttpGet]
		[Route("Branch/GetBySearch")]
		[Authorize]
		public async Task<IEnumerable<CourtBranch>> GetBranchesBySearchResult(string search) => service.courtBranchService.GetBranchesBySearchResult(search);


		[HttpPut]
		[Route("Branch/Update")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CourtBranch>> UpdateBranch(CourtBranch newBranch, int id)
        {
            bool status = IsPhoneFormatted(newBranch.BranchPhone);
            if (status)
            {
                service.courtBranchService.UpdateBranch(newBranch, id);
                return Ok();
            }
            return BadRequest("Phone number is not properly formatted");
        }

    }
}
