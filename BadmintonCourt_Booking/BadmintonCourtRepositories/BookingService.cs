using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtDAOs;
using BadmintonCourtServices.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices
{
    public class BookingService : IBookingService
    {
        private readonly BookingDAO bookingDAO = null;

        public BookingService() 
        {
            if (bookingDAO == null)
            {
                bookingDAO = new BookingDAO();
            }
        }

        public List<Booking> GetAllBookings() => bookingDAO.GetAllBookings();

        public Booking GetBookingByBookingId(string id) => bookingDAO.GetBookingByBookingId(id);
        
        public Booking GetRecentAddedBooking() => bookingDAO.GetRecentAddedBooking();

        public List<Booking> GetBookingsByUserId(string id) => bookingDAO.GetBookingsByUserId(id);

        public List<Booking> GetBookingsByType(int id) => bookingDAO.GetBookingsByType(id);

        public void UpdatBooking(Booking newBooking, string id) => bookingDAO.UpdateBooking(newBooking, id);

        public void AddBooking(Booking booking) => bookingDAO.AddBooking(booking);

        public void DeleteBooking(string id) => bookingDAO.DeleteBooking(id);
 
    }
}
