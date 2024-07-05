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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BadmintonCourtNUnitTests.ControllerTests
{
	internal class RoleControllerTests
	{
		private DbContextOptions<BadmintonCourtContext> _options;
		public readonly int primitiveLength = new DataTestStorage().rolseStorage.Count;
		public RoleController roleController;
		public const string notExistedId = "sfsafsfs";
		public const string existedId = "R001";
		public const string existedRoleName = "Admin";



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

		[Test]
		public async Task GetRoleById_InputExisted_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			roleController = new RoleController(dao);
			var actual = await roleController.GetRoleById(existedId);
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			Assert.IsInstanceOf<Role>(ok.Value);
			Role role = ok.Value as Role;
			Assert.IsTrue(role.RoleId.Equals(existedId));
		}

		[Test]
		public async Task AddRole_RoleNameHasUniqueValue_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			roleController = new RoleController(dao);
			string name = Guid.NewGuid().ToString().Substring(0, 15);
			var actual = await roleController.AddRole(name);
			var ok = actual as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(ok.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual("Success", response["msg"]);
			Assert.AreEqual(primitiveLength + 1, dao.GetAllRoles().Count);
		}

		[Test]
		public async Task AddRole_DuplicatedName_ResultOfBadRequest_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			roleController = new RoleController(dao);
			var actual = await roleController.AddRole(existedRoleName);
			var bad = actual as BadRequestObjectResult;
			Assert.IsInstanceOf<BadRequestObjectResult>(bad);
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(bad.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual($"Role {existedRoleName} has already existed", response["msg"]);
		}


		[Test]
		public async Task AddRole_EmptyName_ResultOfBadRequest_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			roleController = new RoleController(dao);
			var actual = await roleController.AddRole("");
			var bad = actual as BadRequestObjectResult;
			Assert.IsInstanceOf<BadRequestObjectResult>(bad);
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(bad.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual("Name is not full filled yet", response["msg"]);
		}

		[Test]
		public async Task DeleteRole_ResultOfLengthDecreasedBy1_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			roleController = new RoleController(dao);
			var actual = roleController.DeleteRole(existedId);
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(ok.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual("Success", response["msg"]);
			Assert.AreEqual(primitiveLength - 1, dao.GetAllRoles().Count);
		}

		[Test]
		public async Task UpdateRole_NewNameHasUniqueValue_ReturnsWell()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			roleController = new RoleController(dao);
			string name = Guid.NewGuid().ToString().Substring(0, 15);
			var actual = await roleController.UpdateRole(name, existedId);
			Assert.IsFalse(!name.Equals(dao.GetRoleById(existedId).RoleName));
			var ok = actual as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(ok.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual("Success", response["msg"]);
			Assert.IsTrue(name.Equals(dao.GetRoleById(existedId).RoleName));
		}

		[Test]
		public async Task UpdateRole_NewNameHasDuplicatedValue_ResultOfBadRequest_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			roleController = new RoleController(dao);
			var actual = await roleController.UpdateRole(existedRoleName, existedId);
			Assert.IsTrue(existedRoleName.Equals(dao.GetRoleById(existedId).RoleName));
			var bad = actual as BadRequestObjectResult;
			Assert.IsInstanceOf<BadRequestObjectResult>(bad);
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(bad.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual($"Role {existedRoleName} has already existed", response["msg"]);
		}

		[Test]
		public async Task UpdateRole_EmptyNewName_No_ChangeData_ResultOfOk_ReturnsTrue()
		{
			var context = new BadmintonCourtContext(_options);
			RoleDAO dao = new RoleDAO(context);
			roleController = new RoleController(dao);
			var actual = await roleController.UpdateRole("", existedId);
			Assert.IsTrue(!"".Equals(dao.GetRoleById(existedId).RoleName));
			var ok = actual as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(ok.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual("Success", response["msg"]);
			Assert.AreEqual(existedRoleName, dao.GetRoleById(existedId).RoleName);
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
