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
		private readonly BadmintonCourtService service = null;

		public FeedbackController(IConfiguration config)
		{
			if (service == null)
			{
				service = new BadmintonCourtService(config);
			}
		}
	
		[HttpGet]
		[Route("Feedback/GetAll")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetAllFeedbacks() => Ok(service.feedbackService.GetAllFeedbacks());

		[HttpGet]
		[Route("Feedback/GetByBranch")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByBranch(string id) => Ok(service.feedbackService.GetBranchFeedbacks(id));

		[HttpGet]
		[Route("Feedback/GetBySearch")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksBySearch(string search) => Ok(service.feedbackService.GetFeedbacksByContent(search));

		[HttpGet]
		[Route("Feedback/GetByUser")]
		//[Authorize]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByUser(string id) => Ok(service.feedbackService.GetA_UserFeedbacks(id));

		[HttpGet]
		[Route("Feedback/GetByRate")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByRate(int rate) => Ok(service.feedbackService.GetFeedbacksByRate(rate));

		[HttpPost]
		[Route("Feedback/Post")]
		//[Authorize]
		public async Task<IActionResult> AddFeedback(int rate, string content, string id, string branchId)
		{
			if (content.IsNullOrEmpty())
				return BadRequest("Full fill your comment");
			service.feedbackService.AddFeedback(new Feedback { FeedbackId = "F" + (service.feedbackService.GetAllFeedbacks().Count + 1).ToString("D8"), UserId = id, BranchId = branchId, Content = content, Rate = rate });
			return Ok();
		}

		[HttpPut]
		[Route("Feedback/Update")]
		//[Authorize]
		public async Task<IActionResult> UpdateFeedback(int rate, string content, string id)
		{
			Feedback feedback = service.feedbackService.GetFeedbackByFeedbackId(id);
			feedback.Rate = int.Parse(rate.ToString());
			if (!content.IsNullOrEmpty())
				feedback.Content = content;
			service.feedbackService.UpdateFeedback(feedback, id);
			return Ok();
		}

		[HttpDelete]
		[Route("Feedback/Delete")]
		//[Authorize]
		public async Task<IActionResult> DeleteFeedback(string id)
		{
			service.feedbackService.DeleteFeedback(id);
			return Ok();
		}

	}
}
