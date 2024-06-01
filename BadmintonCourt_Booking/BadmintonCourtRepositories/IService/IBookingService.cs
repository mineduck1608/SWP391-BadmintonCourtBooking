using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
    internal interface IBookingService
    {
        public List<Booking> GetAllBookings();

        public Booking GetBookingByBookingId(int id);

        public List<Booking> GetBookingsByUserId(int id);

        public List<Booking> GetBookingsByType(int id);

        public List<Booking> GetBookingsByStatus(bool status);

        public void UpdatBooking(Booking newBooking, int bId);

        public void AddBooking(Booking booking);

        public void DeleteBooking(int id);

    }
}
