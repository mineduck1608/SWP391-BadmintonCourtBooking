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
		private readonly ICoordinateService _coordinateService = new CoordinateService();
		private readonly ICourtService _courtService = new CourtService();
		private readonly IFeedbackService _feedbackService = new FeedbackService();

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
				return BadRequest(new { msg = "Location can't be empty"});
			if (name.IsNullOrEmpty())
				return BadRequest(new { msg = "Name can't be empty" });
			if (Util.IsPhoneFormatted(phone))
			{
				_service.AddBranch(new CourtBranch { BranchId = "B" + (_service.GetAllCourtBranches().Count + 1).ToString("D3"), BranchImg = img, BranchName = name, Location = location, BranchPhone = phone, BranchStatus = 1, MapUrl = mapUrl });
				// 1: hoạt động
				// 0: bỏ
				// -1: bảo trì
				return Ok(new { msg = "Success" });
			}
			return BadRequest(new { msg = "Phone number is not properly formatted"});
		}

		[HttpGet]
		[Route("Branch/GetNearby")]
		public async Task<ActionResult<IEnumerable<CourtBranch>>> GetBranchesByGPS(double latitude, double longitude)
		{
			List<CourtBranch> branchList = _service.GetAllCourtBranches();
			List<CourtBranch> tmpStorage = new List<CourtBranch>();
			Dictionary<CourtBranch, double> rateStorage = new Dictionary<CourtBranch, double>();
			List<CourtBranch> result = new List<CourtBranch>();
            foreach (var item in branchList)
            {
				if (!item.MapUrl.IsNullOrEmpty())
				{
					Dictionary<string, double> tmp = _coordinateService.ExtractCoordinates(item.MapUrl);
					if (tmp != null)
					{
						double distance = _coordinateService.CaculateDistance(latitude, longitude, tmp.ElementAt(0).Value, tmp.ElementAt(1).Value);
						if (distance <= 5000) 
							tmpStorage.Add(item);
					}
				}
            }
			foreach (var item in tmpStorage)
			{
				List<Feedback> feedbackStorage = _feedbackService.GetBranchFeedbacks(item.BranchId);
				double rating = 0;
				foreach (var feedback in feedbackStorage)
					rating += feedback.Rating;
				rating = rating / feedbackStorage.Count;
				rateStorage.Add(item, rating);
			}
			rateStorage = rateStorage.OrderByDescending(x => x.Value).ToDictionary();
			foreach (var item in rateStorage)
				result.Add(item.Key);
            return Ok(result);
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
		public async Task<ActionResult<IEnumerable<CourtBranch>>> GetAllBranches() => Ok(_service.GetAllCourtBranches());

		[HttpGet]
		[Route("Branch/GetBySearch")]
		public async Task<ActionResult<IEnumerable<CourtBranch>>> GetBranchesBySearchResult(string search) => Ok(_service.GetBranchesBySearchResult(search));


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
			if (!phone.IsNullOrEmpty())
			{
				if (Util.IsPhoneFormatted(phone))
					branch.BranchPhone = phone;
				else return BadRequest(new { msg = "Phone number is not properly formatted" });
			}
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
