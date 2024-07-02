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
	internal class RoleDAOTests
	{
		private DbContextOptions<BadmintonCourtContext> _options;
		public readonly int primitiveLength = DataTestStorage.rolseStorage.Count;
		public const string notExistedId = "sfsafsfs";
		public const string existedId = "R001";

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
			if (dbContext.Roles.Count() <= 0)
			{
				List<Role> roleStorage = DataTestStorage.rolseStorage;
				foreach (var item in roleStorage)
				{
					dbContext.Roles.Add(item);
					dbContext.SaveChanges();
				}
			}
		}


		[Test]
		public void GetAll_ExpectedAsPrimitiveLength_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			Assert.AreEqual(primitiveLength, dao.GetAllRoles().Count());
		}


		[Test]
		public void GetRoleById_InputNotExistedId_ResultOfNull_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			Assert.AreEqual(null, dao.GetRoleById(notExistedId));
		}

		[Test]
		public void GetRoleById_InputExistedId_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			Assert.AreEqual(true, dao.GetRoleById(existedId) != null);
		}

		[Test]
		public void AddRole_ResultOfLengthAddedBy1_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			dao.AddRole(new Role() { RoleId = "R004", RoleName = "Anonymous" });
			Assert.AreEqual(primitiveLength + 1, dao.GetAllRoles().Count());
		}

		[Test]
		public void RemoveRole_InputNotExistedId_ResultOfFail_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			dao.DeleteRole(notExistedId);
			Assert.AreEqual(primitiveLength, dao.GetAllRoles().Count());
		}

		[Test]
		public void RemoveRole_InputExistedId_ResultOfSuccess_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			dao.DeleteRole(existedId);
			Assert.AreEqual(primitiveLength - 1, dao.GetAllRoles().Count());
		}

		[Test]
		public void UpdateRole_InputExistedID_UpdateNameAsOwner_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			Role role = dao.GetRoleById(existedId);
			role.RoleName = "Owner";
			dao.UpdateRole(role, existedId);
			string actual = dao.GetRoleById(existedId).RoleName;
			Assert.AreEqual("Owner", actual);
		}

	}
}
