﻿using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
		public async Task<IActionResult> UpdateDiscount(string id, float? amount, float? proportion, string? isDelete)
		{
			Discount discount = _service.GetDiscountById(id);
			if (amount.HasValue && amount > 0)
				discount.Amount = amount.Value;
			if (proportion.HasValue && proportion > 0)
				discount.Proportion = proportion.Value;
			if (!isDelete.IsNullOrEmpty()) 
				discount.IsDelete = isDelete == "null" ? null : true;
			_service.UpdateDiscount(discount, id);
			return Ok(new { msg = "Success" });
		}

		[HttpPut]
		[Route("Discount/Recover")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> RecoverDiscount(string id)
		{
			Discount discount = _service.GetDiscountById(id);
			discount.IsDelete = null;
			_service.UpdateDiscount(discount, id);
			return Ok(new { msg = "Sucess" });
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
