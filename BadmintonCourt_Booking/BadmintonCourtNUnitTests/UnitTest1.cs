using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtDAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;

namespace BadmintonCourtNUnitTests
{
	public class Tests
	{
		private DbContextOptions<BadmintonCourtContext> _options;
		private const int size = 10;
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
			if (dbContext.Users.Count() <= 0)
			{
				for (int i = 0; i < size; i++)
				{
					string role = $"{new Random().Next(1, 3)}";
					dbContext.Users.Add(new User
					{
						UserId = $"{i}",
						Balance = new Random().Next(0, 200000),
						ActiveStatus = true,
						AccessFail = 0,
						LastFail = new DateTime(1900, 1, 1, 0, 0, 0),
						RoleId = role,
						BranchId = role == "2" ? $"{new Random().Next(1, 10)}" : null,
						UserName = "U" + i,
						Password = "Pw" + i
					});
					dbContext.SaveChanges();
				}
			}
		}


		[Test]
		public void Test1()
		{
			var context = new BadmintonCourtContext(_options);
			UserDAO dao = new UserDAO(context);
			int actual = dao.GetAllUsers().Count();
			int expected = size;
			bool check = actual == expected;
			Assert.That(check, Is.True, "Passed");
		}

	}
}