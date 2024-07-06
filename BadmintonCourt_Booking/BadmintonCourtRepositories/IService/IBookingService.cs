using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
    public interface IBookingService
    {
        public List<Booking> GetAllBookings();

        public Booking GetBookingByBookingId(string id);

        public Booking GetRecentAddedBooking();


		public List<Booking> GetBookingsByUserId(string id);

        public List<Booking> GetBookingsByType(int id);

        public void UpdatBooking(Booking newBooking, string id);

        public void AddBooking(Booking booking);

        public void DeleteBooking(string id);

    }
}
