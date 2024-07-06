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
using BadmintonCourtServices.IService;
using Moq;
using Microsoft.IdentityModel.Tokens;

namespace BadmintonCourtNUnitTests.ControllerTests
{
	internal class RoleControllerTests
	{
		private Mock<IRoleService> _roleServiceMock;
		private RoleController _controller;
		private List<Role> _list;
		private readonly int primitiveLength = new DataTestStorage().rolseStorage.Count;
		private const string existedId = "R001";
		private const string notExisted = "gegwqrw";
		private const string existedRoleName = "Admin";
 


		[SetUp]
		public void Setup()
		{
			_roleServiceMock = new Mock<IRoleService>();
			_list = new DataTestStorage().rolseStorage;
			_roleServiceMock.Setup(s => s.GetAllRoles()).Returns(_list);

			_roleServiceMock.Setup(s => s.AddRole(It.IsAny<Role>())).Callback<Role>(role => _list.Add(role));
			_roleServiceMock.Setup(s => s.GetRoleById(It.IsAny<string>()))
					   .Returns<string>(id => _list.FirstOrDefault(r => r.RoleId == id));
			_roleServiceMock.Setup(s => s.DeleteRole(It.IsAny<string>()))
					   .Callback<string>(roleId =>
					   {
						   Role deleted = _list.FirstOrDefault(r => r.RoleId == roleId);
						   if (deleted != null)
							   _list.Remove(deleted);
					   });
			_roleServiceMock.Setup(s => s.UpdateRole(It.IsAny<Role>(), It.IsAny<string>()))
				   .Callback<Role, string>((newRole, id) =>
				   {
					   Role previous = _list.FirstOrDefault(x => x.RoleId == id);
					   if (previous != null)
						   if (!newRole.RoleName.IsNullOrEmpty())
							   previous.RoleName = newRole.RoleName;
				   });
			_controller = new RoleController(_roleServiceMock.Object);
		}

		[Test]
		public async Task GetAllRoles_ResultAsPrimitiveLength_ReturnsTrue()
		{
			var actual = await _controller.GetAllRoles();
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<Role> list = ok.Value as List<Role>;
			Assert.AreEqual(primitiveLength, list.Count);
		}

		[Test]
		public async Task GetRoleById_InputExisted_ResultOfOk_ReturnsTrue()
		{
			Role existed = new Role() { RoleId = existedId, RoleName = "Admin"};
			_roleServiceMock.Setup(s => s.GetRoleById(existedId)).Returns(existed);
			_controller = new RoleController(_roleServiceMock.Object);
			var actual = await _controller.GetRoleById(existedId);
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			Role tmp = ok.Value as Role;
			Assert.AreEqual(tmp.RoleId, existedId);
			Assert.AreEqual(tmp.RoleName, existed.RoleName);
		}

		[Test]
		public async Task AddRole_RoleNameHasUniqueValue_ResultOfOk_ReturnsTrue()
		{
			string name = Guid.NewGuid().ToString().Substring(0, 15);
			var actual = await _controller.AddRole(name);
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
			Assert.AreEqual(primitiveLength + 1, _list.Count);
		}

		[Test]
		public async Task AddRole_DuplicatedName_ResultOfBadRequest_ReturnsTrue()
		{
			var actual = await _controller.AddRole(existedRoleName);
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
			Assert.AreEqual(primitiveLength, _list.Count);
		}


		[Test]
		public async Task AddRole_EmptyName_ResultOfBadRequest_ReturnsTrue()
		{
			var actual = await _controller.AddRole("");
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
			Assert.AreEqual(primitiveLength, _list.Count);
		}

		[Test]
		public async Task DeleteRole_ResultOfLengthDecreasedBy1_ReturnsTrue()
		{
			Role deleted = new Role() { RoleId = existedId, RoleName = "Admin"};
			var actual = _controller.DeleteRole(existedId);
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
			Assert.AreEqual(primitiveLength - 1, _list.Count);
		}

		[Test]
		public async Task UpdateRole_NewNameHasUniqueValue_ResultOfOk_ReturnsWell()
		{
			string name = Guid.NewGuid().ToString().Substring(0, 15);
			var actual = await _controller.UpdateRole(name, existedId);
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
			
			var search = await _controller.GetRoleById(existedId);
			var searchOk = search.Result as OkObjectResult;
			Role check = searchOk.Value as Role;
			Assert.AreEqual(name, check.RoleName);
		}

		[Test]
		public async Task UpdateRole_NewNameHasDuplicatedValue_ResultOfBadRequest_ReturnsTrue()
		{
			var actual = await _controller.UpdateRole(existedRoleName, existedId);
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
			var before = await _controller.GetRoleById(existedId);
			var beforeOk = before.Result as OkObjectResult;
			Role tmp = beforeOk.Value as Role;
			Assert.AreNotEqual("", tmp.RoleName);
			var actual = await _controller.UpdateRole("", existedId);
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

			var search = await _controller.GetRoleById(existedId);
			var searchOk = search.Result as OkObjectResult;
			Role check = searchOk.Value as Role;
			Assert.AreEqual(tmp.RoleName, check.RoleName);
		}

		[TearDown]
		public void TearDown()
		{
			_roleServiceMock.Reset();
			_controller.Dispose();
		}
	}
}
