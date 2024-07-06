using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BadmintonCourtAPI.Controllers
{
	public class DiscountController : Controller
	{
		private readonly IDiscountService _service;

		public DiscountController(IDiscountService service)
		{
			_service = service;
		}

		[HttpGet]
		[Route("Discount/GetAll")]
		public async Task<ActionResult<IEnumerable<Discount>>> GetAllDiscounts() => Ok(_service.GetAllDiscounts());


		[HttpGet]
		[Route("Discount/GetById")]
		public async Task<ActionResult<Discount>> GetDiscountById(string id) => Ok(_service.GetDiscountById(id));


		[HttpPost]
		[Route("Discount/Add")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> CreateDiscount(float? amount, float? proportion)
		{
			_service.AddDiscount(new Discount { DiscountId = "D" + (_service.GetAllDiscounts().Count + 1).ToString("D3"), Amount = amount == null || amount < 0 ? 0 : amount.Value, Proportion = proportion == null || proportion < 0 ? 0 : proportion.Value });
			return Ok(new { msg = "Success"});
		}


		[HttpPut]
		[Route("Discount/Update")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateDiescount(string id, float? amount, float? proportion)
		{
			amount = amount < 0 ? 0 : amount.Value;
			proportion = proportion < 0 ? 0 : proportion.Value;
			Discount discount = _service.GetDiscountById(id);
			if (amount.HasValue)
				discount.Amount = amount.Value;
			if (proportion.HasValue)
				discount.Proportion = proportion.Value;
			_service.UpdateDiscount(discount, id);
			return Ok(new { msg = "Success" });
		}


		[HttpDelete]
		[Route("Discount/Delete")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> RemoveDiscount(string id)
		{
			_service.DeleteDiscount(id);
			return Ok(new { msg = "Success" });
		}

	}
}
