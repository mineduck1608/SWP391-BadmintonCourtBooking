using BadmintonCourtAPI.Controllers;
using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtDAOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Azure;

namespace BadmintonCourtNUnitTests.ControllerTests
{
	internal class RoleControllerTests
	{
		private DbContextOptions<BadmintonCourtContext> _options;
		public readonly int primitiveLength = new DataTestStorage().rolseStorage.Count;
		public RoleController roleController;
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
				List<Role> roleStorage = new DataTestStorage().rolseStorage;
				foreach (var item in roleStorage)
				{
					dbContext.Roles.Add(item);
					dbContext.SaveChanges();
				}
			}
		}

		[Test]
		public async Task GetAllRoles_ResultAsPrimitiveLength_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			roleController = new RoleController(dao);
			var actual = await roleController.GetAllRoles();
			var ok = actual.Result as OkObjectResult;
			//Assert.IsNotNull(actual);
			Assert.IsInstanceOf<OkObjectResult>(ok);
			Assert.IsInstanceOf<List<Role>>(ok.Value);
			List<Role> roleStorage = ok.Value as List<Role>;
			Assert.AreEqual(roleStorage.Count, primitiveLength);
		}

		[TearDown]
		public void TearDown()
		{
			roleController.Dispose();
			using var context = new BadmintonCourtContext(_options);
			context.Database.EnsureDeleted();
		}
	}
}
