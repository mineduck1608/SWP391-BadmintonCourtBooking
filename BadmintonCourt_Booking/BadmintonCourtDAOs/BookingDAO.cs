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

        public BookingDAO(BadmintonCourtContext context)
        {
            _dbContext = context;
        }

        public List<Booking> GetAllBookings() => _dbContext.Bookings.ToList();

        public Booking GetBookingByBookingId(string id) => _dbContext.Bookings.FirstOrDefault(x => x.BookingId == id);

        public Booking GetRecentAddedBooking() => _dbContext.Bookings.AsEnumerable().OrderBy(x => int.Parse(x.BookingId.Substring(1))).LastOrDefault();

        public List<Booking> GetBookingsByUserId(string id) => _dbContext.Bookings.Where(x => x.UserId == id).ToList();

        public List<Booking> GetBookingsByType(int id) => _dbContext.Bookings.Where(x => x.BookingType == id).ToList();

        public void UpdateBooking (Booking newBooking, string id)
        {
            Booking tmp = GetBookingByBookingId(id);
            if (tmp != null)
            {
                tmp.BookingType = newBooking.BookingType;
                tmp.Amount = newBooking.Amount;
                _dbContext.Bookings.Update(tmp);
                _dbContext.SaveChanges();
            }
        }

        public void AddBooking(Booking booking)
        {
            _dbContext.Bookings.Add(booking);
            _dbContext.SaveChanges();
        }

        public void DeleteBooking(string id) 
        {
            Booking booking = GetBookingByBookingId(id);
            if (booking != null)
            {
				_dbContext.Bookings.Remove(booking);
				_dbContext.SaveChanges();
			}
        }

    }
}
