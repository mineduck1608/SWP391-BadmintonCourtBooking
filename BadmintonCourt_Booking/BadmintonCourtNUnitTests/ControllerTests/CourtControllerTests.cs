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
using Microsoft.IdentityModel.Tokens;
using BadmintonCourtBusinessObjects.SupportEntities.Court;


namespace BadmintonCourtNUnitTests.ControllerTests
{
	internal class CourtControllerTests
	{
		private Mock<ICourtService> _courtServiceMock;
		private CourtController _controller;
		private List<Court> _list;
		private readonly int primitiveLength = new DataTestStorage().courtStorage.Count;
		private const string existedId = "C1";
		private const string existedBranchId = "B001";
		private const string notExisted = "wietweit131@";

		private const int active = 1;
		private const int inactive = 5;
		private const double existedMax = 40000;
		private const double existedMin = 20000;


		[SetUp]
		public void Setup()
		{
			_courtServiceMock = new Mock<ICourtService>();
			_list = new DataTestStorage().courtStorage;
			_courtServiceMock.Setup(s => s.GetAllCourts()).Returns(_list);

			_courtServiceMock.Setup(s => s.AddCourt(It.IsAny<Court>())).Callback<Court>(court => _list.Add(court));
			_courtServiceMock.Setup(s => s.GetCourtByCourtId(It.IsAny<string>()))
					   .Returns<string>(id => _list.FirstOrDefault(c => c.CourtId == id));
			_courtServiceMock.Setup(s => s.DeleteCourt(It.IsAny<string>()))
					   .Callback<string>(roleId =>
					   {
						   Court deleted = _list.FirstOrDefault(c => c.CourtId == roleId);
						   if (deleted != null)
							   _list.Remove(deleted);
					   });
			_courtServiceMock.Setup(s => s.UpdateCourt(It.IsAny<Court>(), It.IsAny<string>()))
				   .Callback<Court, string>((newCourt, id) =>
				   {
					   Court previous = _list.FirstOrDefault(x => x.CourtId == id);
					   if (previous != null)
					   {
						   if (!newCourt.BranchId.IsNullOrEmpty())
							   previous.BranchId = newCourt.BranchId;
						   if (!newCourt.CourtImg.IsNullOrEmpty())
							   previous.CourtImg = newCourt.CourtImg;
						   if (!newCourt.Description.IsNullOrEmpty())
							   previous.Description = newCourt.Description;
						   previous.CourtStatus = newCourt.CourtStatus;
					   }

				   });

			_courtServiceMock.Setup(s => s.GetCourtsByStatus(It.IsAny<bool>()))
				.Returns<bool>(status => _list.Where(x => x.CourtStatus == status).ToList());

			_courtServiceMock.Setup(s => s.GetCourtsByBranchId(It.IsAny<string>()))
			.Returns<string>(id => _list.Where(x => x.BranchId == id).ToList());

			_courtServiceMock.Setup(s => s.GetCourtsByPriceInterval(It.IsAny<double>(), It.IsAny<double>()))
			.Returns<double, double>((min, max) => _list.Where(x => x.Price >= min && x.Price <= max).ToList());

			_controller = new CourtController(_courtServiceMock.Object);
		}

		[Test]
		public async Task GetAllCourts_ResultAsPrimitiveLength_ReturnsTrue()
		{
			var actual = await _controller.GettAllCourts();
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<Court> list = ok.Value as List<Court>;
			Assert.AreEqual(primitiveLength, list.Count);
		}

		[Test]
		public async Task GetCourtById_InputExisted_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.GetCourtById(existedId);
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			Court tmp = ok.Value as Court;
			Assert.IsNotNull(tmp);
			Assert.AreEqual(tmp.CourtId, existedId);
		}

		[Test]
		public async Task GetCourtsByStatus_InputTrue_ResultOfAListWithCountAs1_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.GetCourtsByStatus(true);
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<Court> tmpStorage = ok.Value as List<Court>;
			Assert.AreEqual(active, tmpStorage.Count);
		}

		[Test]
		public async Task GetCourtsByStatus_InputFalse_ResultOfAListWithCountAs5_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.GetCourtsByStatus(false);
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<Court> tmpStorage = ok.Value as List<Court>;
			Assert.AreEqual(inactive, tmpStorage.Count);
		}

		[Test]
		public async Task GetCourtsByBranch_InputExistedBranch_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.GetCourtsByBranch(existedBranchId);
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<Court> tmpStorage = ok.Value as List<Court>;
			Assert.IsTrue(tmpStorage.Count > 0);
		}

		[Test]
		public async Task GetCourtsByPriceInterval_InputExistedInterval_ResultOfAListWithLengthAsPrimitive_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.GetCourtsByPriceInterval(existedMin.ToString(), existedMax.ToString());
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<Court> tmpStorage = ok.Value as List<Court>;
			Assert.IsTrue(tmpStorage.Count == primitiveLength);
		}


		[Test]
		public async Task GetCourtsByPriceInterval_InputMaxSmallerThanMin_ResultOfEmpty_ResultOfBadRequest_ReturnsTrue()
		{
			var actual = await _controller.GetCourtsByPriceInterval(existedMax.ToString(), existedMin.ToString());
			var bad = actual.Result as BadRequestObjectResult;
			Assert.IsInstanceOf<BadRequestObjectResult>(bad);
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(bad.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual("Max must be larger than min", response["msg"]);
		}

		[Test]
		public async Task GetCourtsByPriceInterval_InputEmpty_ResultOfAListWithLengthAsPrimitive_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.GetCourtsByPriceInterval("", "");
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<Court> tmpStorage = ok.Value as List<Court>;
			Assert.IsTrue(tmpStorage.Count == primitiveLength);
		}

		[Test]
		public async Task GetCourtsByPriceInterval_InputOutOfExistedInterval_ResultOfEmpty_ResultOfOk_ReturnsTrue()
		{
			double max = existedMin - 100;
			double min = max - 100;
			var actual = await _controller.GetCourtsByPriceInterval(min.ToString(), max.ToString());
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<Court> tmpStorage = ok.Value as List<Court>;
			Assert.IsTrue(tmpStorage.Count == 0);
		}

		[Test]
		public async Task AddCourt_ResultOfLengthAddedBy1_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.AddCourt("", existedBranchId, "", 20000);
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
		public async Task UpdateCourt_InputValidDifferentPriceAs100000_ResultOfOk_ReturnsTrue()
		{
			var before = await _controller.GetCourtById(existedId);
			var beforeOk = before.Result as OkObjectResult;
			Court tmpBefore = beforeOk.Value as Court;
			Assert.AreNotEqual(tmpBefore.Price, 1000000);
			
			var actual = await _controller.UpdateCourt("", "", existedId, true, 100000);
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

			//--------------------------------------------------------------
			var search = await _controller.GetCourtById(existedId);
			var searchOk = search.Result as OkObjectResult;
			Court tmpAfter = searchOk.Value as Court;
			Assert.AreEqual(tmpAfter.Price, 100000);
		}

		[Test]
		public async Task UpdateCourt_InputNegativePrice_ResultOfOkWithDataNoChange_ReturnsTrue()
		{
			var before = await _controller.GetCourtById(existedId);
			var beforeOk = before.Result as OkObjectResult;
			Court tmpBefore = beforeOk.Value as Court;
			Assert.AreNotEqual(tmpBefore.Price, -1);

			var actual = await _controller.UpdateCourt("", "", existedId, true, -1);
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

			//--------------------------------------------------------------
			var search = await _controller.GetCourtById(existedId);
			var searchOk = search.Result as OkObjectResult;
			Court tmpAfter = searchOk.Value as Court;
			Assert.AreEqual(tmpAfter.Price, tmpBefore.Price);
		}

		[Test]
		public async Task UpdateCourt_NewPriceHasNoValue_ResultOfOkWithDataNoChange_ReturnsTrue()
		{
			var before = await _controller.GetCourtById(existedId);
			var beforeOk = before.Result as OkObjectResult;
			Court tmpBefore = beforeOk.Value as Court;

			var actual = await _controller.UpdateCourt("", "", existedId, true, null);
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

			//--------------------------------------------------------------
			var search = await _controller.GetCourtById(existedId);
			var searchOk = search.Result as OkObjectResult;
			Court tmpAfter = searchOk.Value as Court;
			Assert.AreEqual(tmpAfter.Price, tmpBefore.Price);
		}

		[TearDown]
		public void TearDown()
		{
			_courtServiceMock.Reset();
			_controller.Dispose();
		}
	}
}
