using BadmintonCourtAPI.Controllers;
using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;


namespace BadmintonCourtNUnitTests.ControllerTests
{
	internal class CourtControllerTests
	{
		private Mock<IRoleService> _roleServiceMock;
		private RoleController _controller;
		private List<Role> _list;
		private readonly int primitiveLength = new DataTestStorage().rolseStorage.Count;
		private const string existedId = "R001";
		private const string notExisted = "gegwqrw";


		[SetUp]
		public void Setup()
		{
			_roleServiceMock = new Mock<IRoleService>();
			_list = new DataTestStorage().rolseStorage;
			_roleServiceMock.Setup(s => s.GetAllRoles()).Returns(_list);

			_roleServiceMock.Setup(s => s.AddRole(It.IsAny<Role>())).Callback<Role>(role => _list.Add(role));
			_controller = new RoleController(_roleServiceMock.Object);
		}

		[Test]
		public async Task AddRole_NewRoleHasUniqueValue_ResultOfOk_ReturnsTrue()
		{
			//string roleName = $"{Guid.NewGuid().ToString().Substring(0, 10)}";			
			//var controller = new RoleController(_roleServiceMock.Object);
			//var result = await controller.AddRole(roleName);
			//var ok = result as OkObjectResult;
			//Assert.IsInstanceOf<OkObjectResult>(ok);
			//var jsonOptions = new JsonSerializerOptions
			//{
			//	PropertyNameCaseInsensitive = true
			//};
			//var jsonString = System.Text.Json.JsonSerializer.Serialize(ok.Value);
			//var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			//Assert.IsTrue(response.ContainsKey("msg"));
			//Assert.AreEqual("Success", response["msg"]);
			//Assert.AreEqual(new DataTestStorage().rolseStorage.Count + 1, _list.Count);

			//var searchResult = await controller.GetRolesByName(roleName);
			//Assert.IsNull(searchResult);
			//var searchOk = searchResult.Result as OkObjectResult;
			//Assert.IsInstanceOf<OkObjectResult>(searchOk);
			//List<Role> tmp = searchOk.Value as List<Role>;
			//Assert.AreEqual(roleName, tmp[0].RoleName);

		}

		[TearDown]
		public void TearDown()
		{
			_roleServiceMock.Reset();
			_controller.Dispose();
		}
	}
}
