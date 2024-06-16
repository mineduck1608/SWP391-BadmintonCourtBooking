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
        private readonly BookingDAO _bookingDAO = null;

        public BookingService() 
        {
            if (_bookingDAO == null)
            {
                _bookingDAO = new BookingDAO();
            }
        }

        public List<Booking> GetAllBookings() => _bookingDAO.GetAllBookings();

        public Booking GetBookingByBookingId(string id) => _bookingDAO.GetBookingByBookingId(id);
        
        public Booking GetRecentAddedBooking() => _bookingDAO.GetRecentAddedBooking();

        public List<Booking> GetBookingsByUserId(string id) => _bookingDAO.GetBookingsByUserId(id);

        public List<Booking> GetBookingsByType(int id) => _bookingDAO.GetBookingsByType(id);

        public void UpdatBooking(Booking newBooking, string id) => _bookingDAO.UpdateBooking(newBooking, id);

        public void AddBooking(Booking booking) => _bookingDAO.AddBooking(booking);

        public void DeleteBooking(string id) => _bookingDAO.DeleteBooking(id);
 
    }
}
