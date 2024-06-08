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

        public FeedbackController()
        {
            if (service == null)
            {
                service = new BadmintonCourtService();
            }
        }

        [HttpGet]
        [Route("Feedback/GetAll")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetAllFeedbacks() => Ok(service.feedbackService.GetAllFeedbacks());

        [HttpGet]
		[Route("Feedback/GetByBranch")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByBranch(int id) => Ok(service.feedbackService.GetBranchFeedbacks(id));

        [HttpGet]
        [Route("Feedback/GetBySearch")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksBySearch(string search) => Ok(service.feedbackService.GetFeedbacksByContent(search));

        [HttpGet]
        [Route("Feedback/GetByUser")]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByUser(int id) => Ok(service.feedbackService.GetA_UserFeedbacks(id));

        [HttpGet]
        [Route("Feedback/GetByRate")]
		public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByRate(int rate) => Ok(service.feedbackService.GetFeedbacksByRate(rate));

        [HttpPost]
        [Route("Feedback/Post")]
        //[Authorize]
        public async Task<IActionResult> AddFeedback(int rate, string content, int id, int branchId)
        {
            service.feedbackService.AddFeedback(new Feedback(rate, content, id, branchId));
            return Ok();
        }

        [HttpPut]
        [Route("Feedback/Update")]
		//[Authorize]
		public async Task<IActionResult> UpdateFeedback(int? rate, string content, int id)
		{
            Feedback feedback = service.feedbackService.GetFeedbackByFeedbackId(id);
            if (rate != null)
                feedback.Rate = int.Parse(rate.ToString());
             if (!content.IsNullOrEmpty())
                feedback.Content = content;
             service.feedbackService.UpdateFeedback(feedback, id);
			return Ok();
		}

	}
}
