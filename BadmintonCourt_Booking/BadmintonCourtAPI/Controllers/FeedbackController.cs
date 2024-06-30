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
		//[Authorize]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByUser(string id) => Ok(_service.FeedbackService.GetA_UserFeedbacks(id));

		[HttpGet]
		[Route("Feedback/GetByRate")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByRate(int rate) => Ok(_service.FeedbackService.GetFeedbacksByRate(rate));

		[HttpPost]
		[Route("Feedback/Post")]
		//[Authorize]
		public async Task<IActionResult> AddFeedback(int rate, string content, string id, string branchId)
		{
			if (content.IsNullOrEmpty())
				return BadRequest("Full fill your comment");
			_service.FeedbackService.AddFeedback(new Feedback { FeedbackId = "F" + (_service.FeedbackService.GetAllFeedbacks().Count + 1).ToString("D8"), UserId = id, BranchId = branchId, Content = content, Rating = rate, Period = DateTime.Now });
			return Ok();
		}

		[HttpPut]
		[Route("Feedback/Update")]
		//[Authorize]
		public async Task<IActionResult> UpdateFeedback(int rate, string content, string id)
		{
			Feedback feedback = _service.FeedbackService.GetFeedbackByFeedbackId(id);
			feedback.Rating = int.Parse(rate.ToString());
			if (!content.IsNullOrEmpty())
				feedback.Content = content;
			feedback.Period = DateTime.Now;
			_service.FeedbackService.UpdateFeedback(feedback, id);
			return Ok();
		}

		[HttpDelete]
		[Route("Feedback/Delete")]
		//[Authorize]
		public async Task<IActionResult> DeleteFeedback(string id)
		{
			_service.FeedbackService.DeleteFeedback(id);
			return Ok();
		}

	}
}
