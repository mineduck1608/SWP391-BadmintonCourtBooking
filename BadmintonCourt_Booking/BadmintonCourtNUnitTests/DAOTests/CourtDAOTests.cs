using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtBusinessObjects.SupportEntities.Court;
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
	internal class CourtDAOTests
	{
		private DbContextOptions<BadmintonCourtContext> _options;
		public readonly int primitiveLength = new DataTestStorage().courtStorage.Count;
		public readonly List<string> existedBranches = new List<string>() { "B001", "B002", "B003" };
		public const string existedId = "C1";
		public const string notExistedId = "safqwrq69449";
		public const double existedMaxPrice = 40000;
		public const double existedMinPrice = 20000;
		public readonly int offCourtNum = new DataTestStorage().courtStorage.Where(x => x.CourtStatus == false).Count();


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
			if (dbContext.Courts.Count() <= 0)
			{
				List<Court> courtStorage = new DataTestStorage().courtStorage;
				foreach (var item in courtStorage)
				{
					dbContext.Courts.Add(item);
					dbContext.SaveChanges();
				}
			}
		}

		[Test]
		public void GetAll_ExpectedAsPrimitiveLength_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtDAO dao = new CourtDAO(context);
			Assert.AreEqual(primitiveLength, dao.GetAllCourts().Count());
		}

		[Test]
		public void GetById_InputNotExistedId_ResultOfNull_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtDAO dao = new CourtDAO(context);
			Assert.IsTrue(dao.GetCourtByCourtId(notExistedId) == null);
		}

		[Test]
		public void GetById_InputExistedId_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtDAO dao = new CourtDAO(context);
			Assert.IsTrue(dao.GetCourtByCourtId(existedId) != null);
		}

		[Test]
		public void Add_ResultOfLengthAddedBy1_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtDAO dao = new CourtDAO(context);
			dao.AddCourt(new Court() { CourtId = $"{new Random().Next(1, 100)}", BranchId = "B010", CourtName = "Court 1", CourtStatus = true, Description = "V1p pr0", Price = 20000 });
			Assert.AreEqual(primitiveLength + 1, dao.GetAllCourts().Count());
		}

		[Test]
		public void GetByStatus_ResultOf2ListWithLenghLargerThan0_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtDAO dao = new CourtDAO(context);
			Assert.IsTrue(dao.GetCourtsByStatus(true).Count() > 0);
			Assert.IsTrue(dao.GetCourtsByStatus(false).Count() > 0);
		}

		[Test]
		public void GetByBranch_InputExistedBranches_ResultOf3ListWithLengthLargerThan0_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtDAO dao = new CourtDAO(context);
			foreach (var item in existedBranches)
				Assert.IsTrue(dao.GetCourtsByBranchId(item).Count > 0);      
        }

		[Test]
		public void GetByPrices_InputExistedInterval_ResultOfCountAsPrimitive_ReturnsTue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtDAO dao = new CourtDAO(context);
			Assert.IsTrue(dao.GetCourtsByPriceInterval(existedMinPrice, existedMaxPrice).Count == primitiveLength);
		}

		[Test]
		public void GetByPrices_SwapExistedInterval_ResultOfCountAs0_ReturnsTue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtDAO dao = new CourtDAO(context);
			Assert.IsTrue(dao.GetCourtsByPriceInterval(existedMaxPrice, existedMinPrice).Count == 0);
		}

		[Test]
		public void GetByPrices_InputIntervalOutOufExisted_ResultOfCountAs0_ReturnsTue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtDAO dao = new CourtDAO(context);
			double min;
			double max;
			for (int i = 1; i <= 2; i++)
            {
				if (i == 1)
				{
					max = existedMinPrice / 1000;
					min = max / 100;
				}
				else
				{
					min = existedMaxPrice * 1000;
					max = min * 100;
				}
				Assert.IsTrue(dao.GetCourtsByPriceInterval(min, max).Count == 0);
			}
        }

		[Test]
		public void Update_ChangeExistedPriceAs100000_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtDAO dao = new CourtDAO(context);
			Court court = dao.GetCourtByCourtId(existedId);
			Assert.AreNotEqual(100000, court.Price);
			court.Price = 100000;
			dao.UpdateCourt(court, existedId);
			Assert.AreEqual(100000, dao.GetCourtByCourtId(existedId).Price);
		}

		[Test]
		public void Delete_InputExisted_ReturnsCountAsPrimitiveLength_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtDAO dao = new CourtDAO(context);
			dao.DeleteCourt(existedId);
			Assert.AreEqual(offCourtNum + 1, dao.GetAllCourts().Where(x => x.CourtStatus == false).Count());
		}

		[TearDown]
		public void TearDown()
		{
			using var context = new BadmintonCourtContext(_options);
			context.Database.EnsureDeleted();
		}

	}
}
