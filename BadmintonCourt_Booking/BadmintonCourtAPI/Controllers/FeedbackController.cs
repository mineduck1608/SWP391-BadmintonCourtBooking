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
using BadmintonCourtServices.IService;

namespace BadmintonCourtAPI.Controllers
{
	public class FeedbackController : Controller
	{
		private readonly IFeedbackService _service = null;
		private readonly IUserService _userService = new UserService();
		private readonly ISlotService _slotService = new SlotService();
		private readonly IBookingService _bookingService = new BookingService();
		private readonly ICourtService _courtService = new CourtService();


		public FeedbackController(IFeedbackService service)
		{
			_service = service;
		}

		[HttpGet]
		[Route("Feedback/GetAll")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetAllFeedbacks() => Ok(_service.GetAllFeedbacks());

		[HttpGet]
		[Route("Feedback/GetByBranch")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByBranch(string id) => Ok(_service.GetBranchFeedbacks(id));

		[HttpGet]
		[Route("Feedback/GetBySearch")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksBySearch(string search) => Ok(_service.GetFeedbacksByContent(search));

		[HttpGet]
		[Route("Feedback/GetByUser")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByUser(string id) => Ok(_service.GetA_UserFeedbacks(id));

		[HttpGet]
		[Route("Feedback/GetByRate")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByRate(int rate) => Ok(_service.GetFeedbacksByRate(rate));

		[HttpPost]
		[Route("Feedback/Post")]
		[Authorize]
		public async Task<IActionResult> AddFeedback(int rate, string content, string id, string branchId)
		{
			string roleId = _userService.GetUserById(id).RoleId;
			if (roleId != "R001" || roleId != "R002")
			{
				List<Court> courtList = _courtService.GetCourtsByBranchId(branchId);
				List<Booking> bookingList = _bookingService.GetBookingsByUserId(id).Where(x => x.IsDeleted == null).ToList();
				if (bookingList.Count == 0)
					return BadRequest(new { msg = "Can't post feedback" });
				bool status = false;
				foreach (var item in bookingList)
				{
					List<BookedSlot> slotList = _slotService.GetSlotsByBookingId(item.BookingId).Where(x => x.IsDeleted == null).ToList();
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
						}BB
					}
				}
				if (!status)
					return BadRequest(new { msg = "Can't post feedback" });
			}

			if (content.IsNullOrEmpty())
				return BadRequest(new { msg = "Full fill your comment" });
			_service.AddFeedback(new Feedback { FeedbackId = "F" + (_service.GetAllFeedbacks().Count + 1).ToString("D8"), UserId = id, BranchId = branchId, Content = content, Rating = rate, Period = DateTime.Now });
			return Ok(new { msg = "Success" });
		}

		[HttpPut]
		[Route("Feedback/Update")]
		[Authorize]
		public async Task<IActionResult> UpdateFeedback(int? rate, string content, string id, string userId)
		{
			Feedback feedback = _service.GetFeedbackByFeedbackId(id);
			if (feedback.UserId != userId)
				return BadRequest(new { msg = "Can't edit others' feedback" });
			if (rate != null)
				feedback.Rating = rate.Value;
			if (!content.IsNullOrEmpty())
				feedback.Content = content;
			feedback.Period = DateTime.Now;
			_service.UpdateFeedback(feedback, id);
			return Ok(new { msg = "Success" });
		}


		[HttpDelete]
		[Route("Feedback/Delete")]
		[Authorize]
		public async Task<IActionResult> DeleteFeedback(string id, string userID)
		{
			Feedback feedback = _service.GetFeedbackByFeedbackId(id);
			User user = _userService.GetUserById(userID);
			if (user.RoleId == "R001" || feedback.UserId == userID)
			{
				_service.DeleteFeedback(id);
				return Ok(new { msg = "Success" });
			}
			return BadRequest(new { msg = "Can't delete other's feedback" });
		}

		//[HttpDelete]
		//[Route("Feedback/Delete")]
		//[Authorize]
		//public async Task<IActionResult> DeleteFeedback(string id)
		//{
		//	_service.DeleteFeedback(id);
		//	return Ok(new { msg = "Success" });
		//}

	}
}
