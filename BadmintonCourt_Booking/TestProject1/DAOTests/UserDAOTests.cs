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

namespace BadmintonCourtXUnitTest.DAOTests
{
	public class UserDAOTests
	{
		private const int size = 10;
	
		

		private async Task<BadmintonCourtContext> GetDbContext()
		{
			var serviceProvider = new ServiceCollection()
		   .AddEntityFrameworkInMemoryDatabase()
		   .BuildServiceProvider();
			var options = new DbContextOptionsBuilder<BadmintonCourtContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			using var dbContext = new BadmintonCourtContext(options);
			dbContext.Database.EnsureCreated();
			if (await dbContext.Users.CountAsync() <= 0 )
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
					await dbContext.SaveChangesAsync();
                }
            }
			return dbContext;
		}


		[Fact]
		public async void UserDAO_GetAllUsers_ReturnsCorrectLengh()
		{
			var dbContext = await GetDbContext();
			UserDAO dao = new UserDAO(dbContext);
			int actual = dao.GetAllUsers().Count();
			int expected = size;
			Assert.Equal(expected, actual);
		}

	}
}
