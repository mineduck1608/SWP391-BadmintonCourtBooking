using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtDAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtNUnitTests.DAOTests
{
	internal class BookingDAOTests
	{
		private DbContextOptions<BadmintonCourtContext> _options;
		public readonly int primitiveLength = new DataTestStorage().bookingStorage.Count;
		public const string existedId = "BK001";
		public const string existedUserId = "U1";
		public const string notExistedId = "sffwwr2";
		public readonly List<int> existedTypes = new List<int>() { 1, 2 };


		[SetUp]
		public void Setup()
		{
			var serviceProvider = new ServiceCollection()
			.AddEntityFrameworkInMemoryDatabase()
			.BuildServiceProvider();

			// Configure DbContextOptions to use in-memory database
			_options = new DbContextOptionsBuilder<BadmintonCourtContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.UseInternalServiceProvider(serviceProvider)
				.Options;
			using var context = new BadmintonCourtContext(_options);
			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();
			SeedDatabase(context);
		}

		private void SeedDatabase(BadmintonCourtContext dbContext)
		{
			// Code to seed your database with test data
			if (dbContext.Bookings.Count() <= 0)
			{
				List<Booking> bookingStorage = new DataTestStorage().bookingStorage;
				foreach (var item in bookingStorage)
				{
					dbContext.Bookings.Add(item);
					dbContext.SaveChanges();
				}
			}
		}

		[Test]
		public void GetAll_ExpectedAsPrimitiveLength_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			BookingDAO dao = new BookingDAO(context);
			Assert.AreEqual(primitiveLength, dao.GetAllBookings().Count());
		}

		[Test]
		public void GetById_InputNotExistedId_ResultOfNull_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			BookingDAO dao = new BookingDAO(context);
			Assert.IsTrue(dao.GetBookingByBookingId(notExistedId) == null);
		}

		[Test]
		public void GetById_InputExistedId_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			BookingDAO dao = new BookingDAO(context);
			Assert.IsTrue(dao.GetBookingByBookingId(existedId) != null);
		}

		[Test]
 		public void GetByUser_InputNotExisted_ResultOfEnpty_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			BookingDAO dao = new BookingDAO(context);
			Assert.AreEqual(0, dao.GetBookingsByUserId(notExistedId).Count());
		}

		[Test]
		public void GetByUser_InputExisted_ResultOfAList_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			BookingDAO dao = new BookingDAO(context);
			Assert.IsTrue(dao.GetBookingsByUserId(existedUserId).Count() > 0);
		}

		[Test]
		public void GetByType_InputNotExistedTypeAs999_ResultOfEmpty_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			BookingDAO dao = new BookingDAO(context);
			Assert.AreEqual(0, dao.GetBookingsByType(999).Count());
		}

		[Test]
		public void GetByType_InputExisted_ResultOf2Lists_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			BookingDAO dao = new BookingDAO(context);
            for (int i = 1; i <= existedTypes.Count; i++)
				Assert.IsTrue(dao.GetBookingsByType(i).Count() > 0);        
        }

		[Test]
		public void Add_ResultOfLengthAddedBy1_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			BookingDAO dao = new BookingDAO(context);
			string id = Guid.NewGuid().ToString();
			dao.AddBooking(new Booking() { BookingId = id, Amount = 20000, BookingDate = DateTime.Now, BookingType = new Random().Next(1, 2), ChangeLog = 2, UserId = id});
			Assert.AreEqual(primitiveLength + 1, dao.GetAllBookings().Count());
		}

		[Test]
		public void Update_ChangeAmountAs100000_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			BookingDAO dao = new BookingDAO(context);
			Booking booking = dao.GetBookingByBookingId(existedId);
			Assert.AreNotEqual(100000, booking.Amount);
			booking.Amount = 100000;
			dao.UpdateBooking(booking, existedId);
			Assert.AreEqual(100000, dao.GetBookingByBookingId(existedId).Amount);
		}

		[Test]
		public void Remove_DeleteExisted_ResultOfLengthDecreasedBy1_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			BookingDAO dao = new BookingDAO(context);
			dao.DeleteBooking(existedId);
			Assert.AreEqual(primitiveLength - 1, dao.GetAllBookings().Count);
		}


	}
}
