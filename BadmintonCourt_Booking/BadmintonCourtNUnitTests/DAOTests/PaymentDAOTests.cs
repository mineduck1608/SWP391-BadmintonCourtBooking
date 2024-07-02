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
	internal class PaymentDAOTests
	{
		private DbContextOptions<BadmintonCourtContext> _options;
		public readonly int primitiveLength = new DataTestStorage().paymentStorage.Count;
		public const string existedId = "P001";
		public const string existedUserId = "U1";
		public const string notExistedId = "sfawg323r21";
		public readonly List<string> existedBookings = new List<string>() { "BK001", "BK002", "BK003", "BK004" };


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
			if (dbContext.Payments.Count() <= 0)
			{
				List<Payment> paymentStorage = new DataTestStorage().paymentStorage;
				foreach (var item in paymentStorage)
				{
					dbContext.Payments.Add(item);
					dbContext.SaveChanges();
				}
			}
			if (dbContext.Users.Count() <= 0)
			{
				List<User> userStorage = new DataTestStorage().userStorage;
				foreach (var item in userStorage)
				{
					dbContext.Users.Add(item);
					dbContext.SaveChanges();
				}
			}
			if (dbContext.Bookings.Count() <= 0)
			{
				List<Booking> bookingStorage = new DataTestStorage().bookingStorage;
                for (int i = 0; i < existedBookings.Count(); i++)
                {
					List<Booking> tmpStorage = bookingStorage.Where(x => x.BookingId == existedBookings[i]).ToList();
                    foreach (var item in tmpStorage)
                    {
                        dbContext.Bookings.Add(item);
						dbContext.SaveChanges();
                    }
                }
            }
		}

		[Test]
		public void GetAll_ExpectedAsPrimitiveLength_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			PaymentDAO dao = new PaymentDAO(context);
			Assert.AreEqual(primitiveLength, dao.GetAllPayments().Count());
		}

		[Test]
		public void GetById_InputNotExistedId_ResultOfNull_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			PaymentDAO dao = new PaymentDAO(context);
			Assert.IsTrue(dao.GetPaymentByPaymentId(notExistedId) == null);
		}


		[Test]
		public void GetById_InputExistedId_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			PaymentDAO dao = new PaymentDAO(context);
			Assert.IsTrue(dao.GetPaymentByPaymentId(existedId) != null);
		}

		[Test]
		public void GetByUser_InputNotExisted_ResultOfNull_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			PaymentDAO dao = new PaymentDAO(context);
			Assert.AreEqual(0, dao.GetPaymentsByUserId(notExistedId).Count());
		}

		[Test]
		public void GetByUser_InputExisted_ResultOfAList_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			PaymentDAO dao = new PaymentDAO(context);
			Assert.IsTrue(dao.GetPaymentsByUserId(existedUserId).Count() > 0);
		}

		[Test]
		public void GetByBooking_InputNotExisted_ResultOfNull_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			PaymentDAO dao = new PaymentDAO(context);
			Assert.IsTrue(dao.GetPaymentByBookingId(notExistedId) == null);
		}

		[Test]
		public void GetByBooking_InputExisted_ResultOfAList_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			PaymentDAO dao = new PaymentDAO(context);
			Assert.IsTrue(dao.GetPaymentByBookingId(existedBookings[new Random().Next(0, existedBookings.Count - 1)]) != null);
		}

		[Test]
		public void Add_ResultOfLengthAddedBy1_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			PaymentDAO dao = new PaymentDAO(context);
			dao.AddPayment(new Payment() { PaymentId = $"{new Random().Next(1000, 10000)}", Amount = 50000, BookingId = "BK999", Date = DateTime.Now, UserId = existedUserId, Method = 1, TransactionId = $"{new Random().Next(1000, 10000)}" });
			Assert.AreEqual(primitiveLength + 1, dao.GetAllPayments().Count);
		}

		[Test]
		public void Update_ChangeMethodAs999_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			PaymentDAO dao = new PaymentDAO(context);
			Payment payment = dao.GetPaymentByPaymentId(existedId);
			Assert.AreNotEqual(payment.Method, 999);
			payment.Method = 999;
			dao.UpdatePayment(payment, existedId);
			Assert.AreEqual(999, dao.GetPaymentByPaymentId(existedId).Method);
		}

		[Test]
		public void Remove_InputExisted_ResultOfLengthDecreaseBy1_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			PaymentDAO dao = new PaymentDAO(context);
			dao.DeletePayment(existedId);
			Assert.AreEqual(primitiveLength - 1, dao.GetAllPayments().Count);
		}

		[TearDown]
		public void TearDown()
		{
			using var context = new BadmintonCourtContext(_options);
			context.Database.EnsureDeleted();
		}
	}
}
