using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BadmintonCourtAPI.Controllers
{
    public class BookingController : Controller
    {

        private readonly BadmintonCourtService _service = null;

        public BookingController(IConfiguration config)
        {
            if (_service == null)
            {
                _service = new BadmintonCourtService(config);
            }
        }


        [HttpGet]
        [Route("Booking/GetAll")]
		//[Authorize(Roles = "Admin,Staff")]
		public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings() => Ok(_service.BookingService.GetAllBookings());

        [HttpGet]
        [Route("Booking/GetByPaymentId")]
        //[Authorize]
        public async Task<ActionResult<Booking>> GetBookingByPaymentId(string id) => Ok(_service.BookingService.GetBookingByBookingId(id));

        [HttpGet]
        [Route("Booking/GetByUser")]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByUserId(string id) => Ok(_service.BookingService.GetBookingsByUserId(id).ToList());

        [HttpGet]
        [Route("Bookinng/GetByType")]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByType(int id) => Ok(_service.BookingService.GetBookingsByType(id).ToList());


        [HttpDelete]
        [Route("Booking/Delete")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBooking(string id)
        {
            _service.BookingService.DeleteBooking(id);
            return Ok(new { msg = "Success"});
        }

        [HttpPut]
        [Route("Booking/Update")]
        //[Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateBooking(int? type, float? amount, string id)
        {
            Booking booking = _service.BookingService.GetBookingByBookingId(id);
            if (type.HasValue)
                booking.BookingType = type.Value;
            if (amount.HasValue)
                if (amount.Value >= _service.CourtService.GetCourtByCourtId("C1").Price)
                    booking.Amount = amount.Value;
            _service.BookingService.UpdatBooking(booking, id);
            return Ok();
        }

    }
}
