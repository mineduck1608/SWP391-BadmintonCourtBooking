using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace BadmintonCourtAPI.Controllers
{
	public class FeedbackController : Controller
	{
		private readonly BadmintonCourtService _service = null;

		public FeedbackController(IConfiguration config)
		{
			if (_service == null)
			{
				_service = new BadmintonCourtService(config);
			}
		}
	
		[HttpGet]
		[Route("Feedback/GetAll")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetAllFeedbacks() => Ok(_service.FeedbackService.GetAllFeedbacks());

		[HttpGet]
		[Route("Feedback/GetByBranch")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByBranch(string id) => Ok(_service.FeedbackService.GetBranchFeedbacks(id));

		[HttpGet]
		[Route("Feedback/GetBySearch")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksBySearch(string search) => Ok(_service.FeedbackService.GetFeedbacksByContent(search));

		[HttpGet]
		[Route("Feedback/GetByUser")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByUser(string id) => Ok(_service.FeedbackService.GetA_UserFeedbacks(id));

		[HttpGet]
		[Route("Feedback/GetByRate")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByRate(int rate) => Ok(_service.FeedbackService.GetFeedbacksByRate(rate));

		[HttpPost]
		[Route("Feedback/Post")]
		[Authorize]
		public async Task<IActionResult> AddFeedback(int rate, string content, string id, string branchId)
		{
			string roleId = _service.UserService.GetUserById(id).RoleId;
			if (roleId != "R001" || roleId != "R002")
			{
				List<Court> courtList = _service.CourtService.GetCourtsByBranchId(branchId);
				List<Booking> bookingList = _service.BookingService.GetBookingsByUserId(id).Where(x => x.IsDeleted == null).ToList();
				if (bookingList.Count == 0)
					return BadRequest(new { msg = "Can't post feedback" });
				bool status = false;
				foreach (var item in bookingList)
				{
					List<BookedSlot> slotList = _service.SlotService.GetSlotsByBookingId(item.BookingId).Where(x => x.IsDeleted == null).ToList();
					if (slotList.Count == 0)
						continue;
					else
					{
						foreach (var slot in slotList)
						{
							foreach (var court in courtList)
							{
								if (slot.CourtId == court.CourtId)
								{
									status = true;
									break;
								}
							}
						}
					}
				}
				if (!status)
					return BadRequest(new { msg = "Can't post feedback" });
			}	

			if (content.IsNullOrEmpty())
				return BadRequest(new { msg = "Full fill your comment" });
			_service.FeedbackService.AddFeedback(new Feedback { FeedbackId = "F" + (_service.FeedbackService.GetAllFeedbacks().Count + 1).ToString("D8"), UserId = id, BranchId = branchId, Content = content, Rating = rate, Period = DateTime.Now });
			return Ok(new { msg = "Success"});
		}

		[HttpPut]
		[Route("Feedback/Update")]
		[Authorize]
		public async Task<IActionResult> UpdateFeedback(int rate, string content, string id, string userId)
		{
			Feedback feedback = _service.FeedbackService.GetFeedbackByFeedbackId(id);
			if (feedback.UserId != userId)
				return BadRequest(new { msg = "Can't edit others' feedback" });
			feedback.Rating = int.Parse(rate.ToString());
			if (!content.IsNullOrEmpty())
				feedback.Content = content;
			feedback.Period = DateTime.Now;
			_service.FeedbackService.UpdateFeedback(feedback, id);
			return Ok(new { msg = "Success" });
		}

		[HttpDelete]
		[Route("Feedback/Delete")]
		[Authorize]
		public async Task<IActionResult> DeleteFeedback(string id)
		{
			_service.FeedbackService.DeleteFeedback(id);
			return Ok();
		}

	}
}
