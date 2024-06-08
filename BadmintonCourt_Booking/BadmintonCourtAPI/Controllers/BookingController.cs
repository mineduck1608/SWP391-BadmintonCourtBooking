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

        public BookingController()
        {
            service = new BadmintonCourtService();
        }


        [HttpGet]
        [Route("Booking/GetAll")]
        //[Authorize(Roles = "Admin,Staff")]
        public async Task<IEnumerable<Booking>> GetAllBookings() => service.bookingService.GetAllBookings().ToList();

        [HttpGet]
        [Route("Booking/GetByPaymentId")]
        //[Authorize]
        public async Task<ActionResult<Booking>> GetBookingByPaymentId(int id) => service.bookingService.GetBookingByBookingId(id);

        [HttpGet]
        [Route("Booking/GetByUser")]
        //[Authorize]
        public async Task<IEnumerable<Booking>> GetBookingsByUserId(int id) => service.bookingService.GetBookingsByUserId(id).ToList();

        [HttpGet]
        [Route("Bookinng/GetByType")]
        //[Authorize]
        public async Task<IEnumerable<Booking>> GetBookingsByType(int id) => service.bookingService.GetBookingsByType(id).ToList();

        [HttpGet]
        [Route("Booking/GetByStatus")]
        //[Authorize(Roles = "Admin,Staff")]
        public async Task<IEnumerable<Booking>> GetBookingsByStatus(string txtStatus) => txtStatus.Equals("1") ? service.bookingService.GetBookingsByStatus(true).ToList() : service.bookingService.GetBookingsByStatus(false).ToList();

        [HttpDelete]
        [Route("Booking/Delete")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            service.bookingService.DeleteBooking(id);
            return NoContent();
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
        public async Task<IActionResult> UpdateBooking(Booking newBooking, int id)
        {
            service.bookingService.UpdatBooking(newBooking, id);
            return NoContent();
        }

    }
}
