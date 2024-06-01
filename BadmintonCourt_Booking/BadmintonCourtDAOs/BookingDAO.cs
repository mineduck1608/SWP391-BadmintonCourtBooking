using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtDAOs
{
    public class BookingDAO
    {
        private readonly BadmintonCourtContext _dbContext = null;

        public BookingDAO()
        {
            if (_dbContext == null) 
            { 
                _dbContext = new BadmintonCourtContext();
            }
        }

        public List<Booking> GetAllBookings() => _dbContext.Bookings.ToList();

        public Booking GetBookingByBookingId(int id) => _dbContext.Bookings.FirstOrDefault(x => x.BookingId == id);

        public List<Booking> GetBookingsByUserId(int id) => _dbContext.Bookings.Where(x => x.UserId == id).ToList();

        public List<Booking> GetBookingsByType(int id) => _dbContext.Bookings.Where(x => x.BookingType == id).ToList();

        public List<Booking> GetBookingsByStatus(bool status) => _dbContext.Bookings.Where(x => x.BookingStatus == status).ToList();

        public void UpdateBooking (Booking newBooking, int id)
        {
            Booking tmp = GetBookingByBookingId(id);
            if (tmp != null)
            {
                tmp.BookingType = newBooking.BookingType;
                tmp.BookingStatus = newBooking.BookingStatus;
                tmp.TotalPrice = newBooking.TotalPrice;
                _dbContext.Bookings.Update(tmp);
                _dbContext.SaveChanges();
            }
        }

        public void AddBooking(Booking booking)
        {
            _dbContext.Bookings.Add(booking);
            _dbContext.SaveChanges();
        }

        public void DeleteBooking(int bId) 
        {
            _dbContext.Bookings.Remove(GetBookingByBookingId((bId)));
            _dbContext.SaveChanges();
        }

    }
}
