using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using BadmintonCourtServices.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BadmintonCourtAPI.Controllers
{
	public class BookingController : Controller
	{

		private readonly IBookingService _service = null;
		private readonly ICourtService _courtService = new CourtService();

		public BookingController(IBookingService service)
		{
			_service = service;
		}


		[HttpGet]
		[Route("Booking/GetAll")]
		[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings() => Ok(_service.GetAllBookings());

		[HttpGet]
		[Route("Booking/GetById")]
		[Authorize]
		public async Task<ActionResult<Booking>> GetBookingById(string id) => Ok(_service.GetBookingByBookingId(id));

		[HttpGet]
		[Route("Booking/GetByUser")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByUserId(string id) => Ok(_service.GetBookingsByUserId(id).ToList());

		[HttpGet]
		[Route("Bookinng/GetByType")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByType(int id) => Ok(_service.GetBookingsByType(id).ToList());


		[HttpDelete]
		[Route("Booking/Delete")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteBooking(string id)
		{
			_service.DeleteBooking(id);
			return Ok(new { msg = "Success" });
		}



		[HttpPut]
		[Route("Booking/Update")]
		[Authorize]
		public async Task<IActionResult> UpdateBooking(int? type, float? amount, string id)
		{
			Booking booking = _service.GetBookingByBookingId(id);
			if (type.HasValue)
				booking.BookingType = type.Value;
			if (amount.HasValue && amount > 0)
				booking.Amount = amount.Value;
			_service.UpdatBooking(booking, id);
			return Ok(new { msg = "Success"});
		}


	}
}
