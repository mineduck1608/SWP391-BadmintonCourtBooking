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

        public Booking GetBookingByBookingId(int id) => bookingDAO.GetBookingByBookingId(id);

        public List<Booking> GetBookingsByUserId(int id) => bookingDAO.GetBookingsByUserId(id);

        public List<Booking> GetBookingsByType(int id) => bookingDAO.GetBookingsByType(id);

        public List<Booking> GetBookingsByStatus(bool status) => bookingDAO.GetBookingsByStatus(status);

        public void UpdatBooking(Booking newBooking, int bId) => bookingDAO.UpdateBooking(newBooking, bId);

        public void AddBooking(Booking booking) => bookingDAO.AddBooking(booking);

        public void DeleteBooking(int id) => bookingDAO.DeleteBooking(id);
 
    }
}
