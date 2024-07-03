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
	internal class SloDAOTests
	{
		private DbContextOptions<BadmintonCourtContext> _options;
		public readonly int primitiveLength = new DataTestStorage().slotStorage.Count;
		public const string existedId = "S1";
		public const string notExistedId = "Swqrih382r*";
		public const string existedBookingId = "BK001";
		public const string existedCourtId = "C1";
		public readonly DateTime existedDate = new DateTime(1900, 1, 1, 0, 0, 0);
		public readonly DateTime notExistedDate = new DateTime(1800, 1, 1, 0, 0, 0);
		public readonly DateTime existedEndDate = new DateTime(2024, 7, 10, 10, 0, 0);




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
			if (dbContext.BookedSlots.Count() <= 0)
			{
				List<BookedSlot> slotStorage = new DataTestStorage().slotStorage;
				foreach (var item in slotStorage)
				{
					dbContext.BookedSlots.Add(item);
					dbContext.SaveChanges();
				}
			}
		}

		[Test]
		public void GetAll_ExpectedAsPrimitiveLength_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			SlotDAO dao = new SlotDAO(context);
			Assert.AreEqual(primitiveLength, dao.GetAllSlots().Count());
		}

		[Test]
		public void GetById_InputNotExistedId_ResultOfNull_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			SlotDAO dao = new SlotDAO(context);
			Assert.IsTrue(dao.GetSlotById(notExistedId) == null);
		}

		[Test]
		public void GetById_InputExistedId_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			SlotDAO dao = new SlotDAO(context);
			Assert.IsTrue(dao.GetSlotById(existedId) != null);
		}


		[Test]
		public void GetByBooking_InputNotExistedId_ResultOfEmpty_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			SlotDAO dao = new SlotDAO(context);
			Assert.AreEqual(0, dao.GetSlotsByBookingId(notExistedId).Count);
		}

		[Test]
		public void GetByBooking_InputExistedId_ResultOfAList_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			SlotDAO dao = new SlotDAO(context);
			Assert.IsTrue(dao.GetSlotsByBookingId(existedBookingId).Count > 0);
		}

		[Test]
		public void GetByDate_InputNotExisted_ResultOfEmpty_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			SlotDAO dao = new SlotDAO(context);
			Assert.AreEqual(0, dao.GetSLotsByDate(notExistedDate).Count);
		}

		[Test]
		public void GetByDate_InputExisted_ResultOfAList_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			SlotDAO dao = new SlotDAO(context);
			Assert.IsTrue(dao.GetSLotsByDate(existedDate).Count > 0);
		}

		[Test]
		public void GetByCourt_InputNotExisted_ResultOfEmpty_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			SlotDAO dao = new SlotDAO(context);
			Assert.AreEqual(0, dao.GetSlotsByCourt(notExistedId).Count);
		}

		[Test]
		public void GetByCourt_InputExisted_ResultOfAList_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			SlotDAO dao = new SlotDAO(context);
			Assert.IsTrue(dao.GetSlotsByCourt(existedCourtId).Count > 0);
		}

		[Test]
		public void GetByInterval_InputExistedInterval_ResultOfAList_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			SlotDAO dao = new SlotDAO(context);
			Assert.IsTrue(dao.GetA_CourtSlotsInTimeInterval(existedDate, existedEndDate, existedCourtId).Count > 0);
		}


		[Test]
		public void GetByInterval_SwapExistedInterval_ResultOfEmpty_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			SlotDAO dao = new SlotDAO(context);
			Assert.IsTrue(dao.GetA_CourtSlotsInTimeInterval(existedEndDate, existedDate, existedCourtId).Count == 0);
		}

		[Test]
		public void GetByInterval_InputNotExistedInterval_ResultOfEmpty_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			SlotDAO dao = new SlotDAO(context);
			DateTime startDate;
			DateTime endDate;
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
				{
					endDate = notExistedDate;
					startDate = endDate.AddMonths(-2);
				}
				else
				{
					startDate = existedEndDate.AddMonths(2);
					endDate = startDate.AddMonths(2);
				}
				Assert.AreEqual(0, dao.GetA_CourtSlotsInTimeInterval(startDate, endDate, existedCourtId).Count);
            }
        }

		[Test]
		public void GetByInterval_InputNotExistedCourt_ResultOfEmpty_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			SlotDAO dao = new SlotDAO(context);
			Assert.AreEqual(0, dao.GetA_CourtSlotsInTimeInterval(existedDate, existedEndDate, notExistedId).Count);
		}

		[Test]
		public void GetByFixed_InputNumberOfMonthAs1WithDurationAs2Hours_ResultOfAListWithLengthAs4_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			SlotDAO dao = new SlotDAO(context);
			int numMonth = 1;
			int expected = 4;
			DateTime startDate = existedDate.AddMonths(1);
			DateTime endDate = startDate.AddHours(2);
			Assert.AreEqual(expected, dao.GetSlotsByFixedBooking(1, startDate, endDate, existedCourtId).Count);

		}

		[TearDown]
		public void TearDown()
		{
			using var context = new BadmintonCourtContext(_options);
			context.Database.EnsureDeleted();
		}

	}
}
