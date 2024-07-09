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
	internal class DiscountDAOTests
	{
		private DbContextOptions<BadmintonCourtContext> _options;
		public readonly int primitiveLength = new DataTestStorage().discountStorage.Count;
		public const string existedId = "D1";
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
			if (dbContext.Feedbacks.Count() <= 0)
			{
				List<Discount> discountStorage = new DataTestStorage().discountStorage;
				foreach (var item in discountStorage)
				{
					dbContext.Discounts.Add(item);
					dbContext.SaveChanges();
				}
			}
		}

		[Test]
		public void GetAll_ExpectedAsPrimitiveLength_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			DiscountDAO dao = new DiscountDAO(context);
			Assert.AreEqual(primitiveLength, dao.GetAllDiscounts().Count());
		}

		[Test]
		public void GetById_InputNotExistedId_ResultOfNull_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			DiscountDAO dao = new DiscountDAO(context);
			Assert.IsTrue(dao.GetDiscountById(notExistedId) == null);
		}

		[Test]
		public void GetById_InputExistedId_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			DiscountDAO dao = new DiscountDAO(context);
			Assert.IsTrue(dao.GetDiscountById(existedId) != null);
		}

		[Test]
		public void Add_ResultOfLengthAddedBy1_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			DiscountDAO dao = new DiscountDAO(context);
			dao.AddDiscount(new Discount() { DiscountId = $"{new Random().Next(1, 100)}", Amount = 20000, Proportion = 1 });
			Assert.AreEqual(primitiveLength + 1, dao.GetAllDiscounts().Count());
		}

		[Test]
		public void Remove_InputNotExistedId_ResultOfFail_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			DiscountDAO dao = new DiscountDAO(context);
			dao.DeleteDiscount(notExistedId);
			Assert.AreEqual(primitiveLength, dao.GetAllDiscounts().Count());
		}

		[Test]
		public void Remove_InputExistedId_ResultOfSuccess_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			DiscountDAO dao = new DiscountDAO(context);
			Discount previous = dao.GetDiscountById(existedId);
			Assert.IsTrue(previous.IsDelete == null);
			dao.DeleteDiscount(existedId);
			Discount afterUpdated = dao.GetDiscountById(existedId);
			Assert.IsTrue(afterUpdated.IsDelete == true);

		}

		[Test]
		public void UpdateFeedbacm_InputExistedID_UpdateAmountAs1000000_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			DiscountDAO dao = new DiscountDAO(context);
			Discount discount = dao.GetDiscountById(existedId);
			discount.Amount = 1000000;
			dao.UpdateDiscount(discount, existedId);
			double actual = dao.GetDiscountById(existedId).Amount;
			Assert.AreEqual(1000000, actual);
		}

		[TearDown]
		public void TearDown()
		{
			using var context = new BadmintonCourtContext(_options);
			context.Database.EnsureDeleted();
		}

	}
}
