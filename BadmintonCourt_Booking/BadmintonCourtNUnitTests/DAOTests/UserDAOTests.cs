using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtDAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;
using System.Security.Policy;

namespace BadmintonCourtNUnitTests.DAOTests
{
	public class UserDAOTests
	{
		private DbContextOptions<BadmintonCourtContext> _options;

		public readonly int primitiveLength = DataTestStorage.userStorage.Count;
		public readonly int activeCase = DataTestStorage.userStorage.Count - 2;
		public readonly List<CourtBranch> branchStorage = DataTestStorage.branchStorage;
		public const string notExistedId = "sfsafsfs";
		public const string existedId = "U1";


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
				List<User> userStorage = DataTestStorage.userStorage;
                foreach (var item in userStorage)
				{
					dbContext.Users.Add(item);
					dbContext.SaveChanges();
				}
            }
		}
		private List<User> ExtractUserBasedOnRoleFromTestStorage(string roldId) => DataTestStorage.userStorage.Where(x => x.RoleId == roldId).ToList();

		[Test]
		public void GetAll_ExpectedAsPrimitiveLength_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			UserDAO dao = new UserDAO(context);
			int actual = dao.GetAllUsers().Count();
			Assert.AreEqual(primitiveLength, dao.GetAllUsers().Count());
		}

		[Test]
		public void GetUserById_InputNotExistedId_ResultOfNull_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			UserDAO dao = new UserDAO(context);
			Assert.AreEqual(null, dao.GetUserById(notExistedId));
		}

		[Test]
		public void GetUserById_InputExistedId_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			UserDAO dao = new UserDAO(context);
			Assert.AreEqual(true, dao.GetUserById(existedId) != null);
		}

		[Test]
		public void GetStaffsByBranch_InputExistedBranch_ReturnsList()
		{
			var context = new BadmintonCourtContext(_options);
			UserDAO dao = new UserDAO(context);
			List<List<User>> actual = new List<List<User>>();
            foreach (var item in branchStorage)
            {
				actual.Add(dao.GetStaffsByBranch(item.BranchId));
            }
			Assert.AreEqual(true, actual != null);
        }

		[Test] 
		public void AddUser_ResultOfLengthAddedBy1_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			UserDAO dao = new UserDAO(context);
			dao.AddUser(new User() { UserId = "999", RoleId = "R003", Balance = 0, AccessFail = 0, ActiveStatus = false});
			Assert.AreEqual(primitiveLength + 1, dao.GetAllUsers().Count());
		}

		[Test]
		public void RemoveUser_InputNotExistedId_ResultOfFail_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			UserDAO dao = new UserDAO(context);
			dao.DeleteUser(notExistedId);
			Assert.AreEqual(primitiveLength, dao.GetAllUsers().Count());
		}

		[Test]
		public void RemoveUser_InputExistedId_ResultOfSuccess_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			UserDAO dao = new UserDAO(context);
			dao.DeleteUser(existedId);
			Assert.AreEqual(activeCase - 1, dao.GetAllUsers().Where(x => x.ActiveStatus == true).ToList().Count());
		}

		[Test]
		public void UpdateUser_InputExistedID_UpdateBalanceAs100000_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			UserDAO dao = new UserDAO(context);
			User user = dao.GetUserById(existedId);
			user.Balance = 100000;
			dao.UpdateUser(user, existedId);
			double actual = dao.GetUserById(existedId).Balance.Value;
			Assert.AreEqual(100000, actual);
		}

		[Test]
		public void GetByRoles_InputAllRoles_ReturnAllUserOfEachRole_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			UserDAO dao = new UserDAO(context);
			List<Role> roleStorage = DataTestStorage.rolseStorage;
			//List<List<User>> expected = new List<List<User>>();
			//List<List<User>> actual = new List<List<User>>();

            foreach (var item in roleStorage)
            {
				List<User> tmp1 = ExtractUserBasedOnRoleFromTestStorage(item.RoleId);
				List<User> tmp2 = dao.GetUsersByRole(item.RoleId);
				Assert.AreEqual(tmp1.Count, tmp2.Count);
			}
        }

	}
}
