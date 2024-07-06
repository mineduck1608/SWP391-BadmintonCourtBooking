using BadmintonCourtAPI.Controllers;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtBusinessObjects.SupportEntities.Court;
using BadmintonCourtServices.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BadmintonCourtNUnitTests.ControllerTests
{
	internal class BranchControllerTests
	{
		private Mock<ICourtBranchService> _branchServiceMock;
		private BranchController _controller;
		private List<CourtBranch> _list;
		private readonly int primitiveLength = new DataTestStorage().branchStorage.Count;
		private const string existedId = "B001";
		private const string formattedPhoneNumber = "0933111566";


		[SetUp]
		public void Setup()
		{
			_branchServiceMock = new Mock<ICourtBranchService>();
			_list = new DataTestStorage().branchStorage;
			_branchServiceMock.Setup(s => s.GetAllCourtBranches()).Returns(_list);

			_branchServiceMock.Setup(s => s.AddBranch(It.IsAny<CourtBranch>())).Callback<CourtBranch>(branch => _list.Add(branch));
			_branchServiceMock.Setup(s => s.GetBranchById(It.IsAny<string>()))
					   .Returns<string>(id => _list.FirstOrDefault(c => c.BranchId == id));
			_branchServiceMock.Setup(s => s.DeleteBranch(It.IsAny<string>()))
					   .Callback<string>(branchId =>
					   {
						   CourtBranch deleted = _list.FirstOrDefault(c => c.BranchId == branchId);
						   if (deleted != null)
							   _list.Remove(deleted);
					   });
			_branchServiceMock.Setup(s => s.UpdateBranch(It.IsAny<CourtBranch>(), It.IsAny<string>()))
				   .Callback<CourtBranch, string>((newBranch, id) =>
				   {
					   CourtBranch previous = _list.FirstOrDefault(x => x.BranchId == id);
					   if (previous != null)
					   {
						   if (!newBranch.BranchId.IsNullOrEmpty())
							   previous.BranchId = newBranch.BranchId;
						   if (!newBranch.BranchImg.IsNullOrEmpty())
							   previous.BranchImg = newBranch.BranchImg;
						   if (!newBranch.BranchPhone.IsNullOrEmpty())
							   previous.BranchPhone = newBranch.BranchPhone;
						   if (!newBranch.BranchName.IsNullOrEmpty())
							   previous.BranchName = newBranch.BranchName;
						   previous.BranchStatus = newBranch.BranchStatus;
					   }

				   });

			_controller = new BranchController(_branchServiceMock.Object);
		}

		[Test]
		public async Task GetAllBranches_ResultAsPrimitiveLength_ReturnsTrue()
		{
			var actual = await _controller.GetAllBranches();
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<BranchDTO> list = ok.Value as List<BranchDTO>;
			Assert.AreEqual(primitiveLength, list.Count);
		}

		[Test]
		public async Task AddBranch_InputEmptyLocation_ResultOfBadRequest_ReturnsTrue()
		{
			var actual = await _controller.AddBranch("", "", "name", formattedPhoneNumber, "");
			var bad = actual.Result as BadRequestObjectResult;
			Assert.IsInstanceOf<BadRequestObjectResult>(bad);
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(bad.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual("Location can't be empty", response["msg"]);
			Assert.AreEqual(primitiveLength, _list.Count);
		}

		[Test]
		public async Task AddBranch_InputEmptyName_ResultOfBadRequest_ReturnsTrue()
		{
			var actual = await _controller.AddBranch("location", "", "", formattedPhoneNumber, "");
			var bad = actual.Result as BadRequestObjectResult;
			Assert.IsInstanceOf<BadRequestObjectResult>(bad);
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(bad.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual("Name can't be empty", response["msg"]);
			Assert.AreEqual(primitiveLength, _list.Count);
		}

		[Test]
		public async Task AddBranch_InputInvalidPhoneNumber_ResultOfBadRequest_ReturnsTrue()
		{
			var actual = await _controller.AddBranch("location", "", "name", "sdfjnsfn", "");
			var bad = actual.Result as BadRequestObjectResult;
			Assert.IsInstanceOf<BadRequestObjectResult>(bad);
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(bad.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual("Phone number is not properly formatted", response["msg"]);
			Assert.AreEqual(primitiveLength, _list.Count);
		}

		[Test]
		public async Task AddBranch_InputInvalidData_ResultOfLengthAddedBy1_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.AddBranch("location", "", "name", formattedPhoneNumber, "");
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
			Assert.AreEqual(primitiveLength + 1, _list.Count);
		}

		[Test]
		public async Task DeleteBranch_ResultOfLengthDecreasedBy1_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.DeleteBranch(existedId);
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
		public async Task UpdateBranch_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.UpdateBranch("", "", "", "", 1, existedId, "");
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
		}

		[Test]
		public async Task UpdateBranch__InputInvalidPhoneNumber_ResultOfBadRequest_ReturnsTrue()
		{
			var actual = await _controller.UpdateBranch("", "", "", "saffewg", 1, existedId, "");
			var bad = actual.Result as BadRequestObjectResult;
			Assert.IsInstanceOf<BadRequestObjectResult>(bad);
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(bad.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual("Phone number is not properly formatted", response["msg"]);
		}

		[TearDown]
		public void TearDown()
		{
			_branchServiceMock.Reset();
			_controller.Dispose();
		}
	}
}
