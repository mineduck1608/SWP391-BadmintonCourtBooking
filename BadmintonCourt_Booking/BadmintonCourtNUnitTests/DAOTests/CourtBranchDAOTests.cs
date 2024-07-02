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
	internal class CourtBranchDAOTests
	{
		private DbContextOptions<BadmintonCourtContext> _options;
		public readonly int primitiveLength = new DataTestStorage().branchStorage.Count;
		public const string existedId = "B001";
		public const string maintainedBranch = "B003";
		public const string notExistedId = "sffwwr2";


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
			if (dbContext.CourtBranches.Count() <= 0)
			{
				List<CourtBranch> branchStorage = new DataTestStorage().branchStorage;
				foreach (var item in branchStorage)
				{
					dbContext.CourtBranches.Add(item);
					dbContext.SaveChanges();
				}
			}
			if (dbContext.Courts.Count() <= 0)
			{
				List<Court> courtStorage = new DataTestStorage().courtStorage.Where(x => x.BranchId == existedId).ToList();
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
			CourtBranchDAO dao = new CourtBranchDAO(context);
			Assert.AreEqual(primitiveLength, dao.GetAllCourtBranches().Count());
		}

		[Test]
		public void GetById_InputNotExistedId_ResultOfNull_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtBranchDAO dao = new CourtBranchDAO(context);
			Assert.IsTrue(dao.GetBranchById(notExistedId) == null);
		}

		[Test]
		public void GetById_InputExistedId_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtBranchDAO dao = new CourtBranchDAO(context);
			Assert.IsTrue(dao.GetBranchById(existedId) != null);
		}

		[Test]
		public void Add_ResultOfLengthAddedBy1_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtBranchDAO dao = new CourtBranchDAO(context);
			dao.AddBranch(new CourtBranch() { BranchId = $"{new Random().Next(1, 1000)}", BranchName = "Nam Nguyen", BranchPhone = "033211655", BranchStatus = 1, Location = "Quan 1" });
			Assert.AreEqual(primitiveLength + 1, dao.GetAllCourtBranches().Count());
		}

		[Test]
		public void Remove_InputExistedId_ResultOfSuccess_AllCourtStatusBecomeFalse_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			CourtBranchDAO branchDAO = new CourtBranchDAO(context);
			CourtDAO courtDAO = new CourtDAO(context);
			Assert.IsFalse(courtDAO.GetCourtsByBranchId(existedId).Where(x => x.CourtStatus == true).Count() == 0);
			branchDAO.DeleteBranch(existedId);
			Assert.AreEqual(0, courtDAO.GetCourtsByBranchId(existedId).Where(x => x.CourtStatus == true).Count());
		}

		[TearDown]
		public void TearDown()
		{
			using var context = new BadmintonCourtContext(_options);
			context.Database.EnsureDeleted();
		}
	}
}
