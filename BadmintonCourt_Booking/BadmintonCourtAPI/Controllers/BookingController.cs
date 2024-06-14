using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BadmintonCourtAPI.Controllers
{
    public class BookingController : Controller
    {

        private readonly BadmintonCourtService service = null;

        public BookingController(IConfiguration config)
        {
            if (service == null)
            {
                service = new BadmintonCourtService(config);
            }
        }

		string GenerateId()
		{
			string number = $"{service.bookingService.GetAllBookings().Count()}";
			int length = number.Length;
			while (length < 17)
			{
				string tmp = $"0{number}";
				number = tmp;
				length++;
			}
			return $"BK{number}";
		}


        [HttpGet]
        [Route("Booking/GetAll")]
		//[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings() => Ok(service.bookingService.GetAllBookings());

        [HttpGet]
        [Route("Booking/GetByPaymentId")]
        //[Authorize]
        public async Task<ActionResult<Booking>> GetBookingByPaymentId(string id) => Ok(service.bookingService.GetBookingByBookingId(id));

        [HttpGet]
        [Route("Booking/GetByUser")]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByUserId(string id) => Ok(service.bookingService.GetBookingsByUserId(id).ToList());

        [HttpGet]
        [Route("Bookinng/GetByType")]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByType(int id) => Ok(service.bookingService.GetBookingsByType(id).ToList());

        [HttpDelete]
        [Route("Booking/Delete")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBooking(string id)
        {
            service.bookingService.DeleteBooking(id);
            return Ok();
        }

        [HttpPost]
        [Route("Booking/Add")]
        //[Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> AddBooking(Booking booking)
        {
            service.bookingService.AddBooking(booking);
            return NoContent();
        }

        [HttpPut]
        [Route("Booking/Update")]
        //[Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateBooking(int? type, float? amount, string id)
        {
            Booking booking = service.bookingService.GetBookingByBookingId(id);
            if (type.HasValue)
                booking.BookingType = type.Value;
            if (amount.HasValue)
                if (amount.Value >= service.courtService.GetCourtByCourtId("C1").Price)
                    booking.Amount = amount.Value;
            service.bookingService.UpdatBooking(booking, id);
            return Ok();
        }

    }
}
