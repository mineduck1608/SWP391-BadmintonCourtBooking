using BadmintonCourtAPI.Controllers;
using BadmintonCourtBusinessObjects.Entities;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BadmintonCourtNUnitTests.ControllerTests
{
	internal class FeedbackControllerTests
	{
		private Mock<IFeedbackService> _feedbackServiceMock;
		private FeedbackController _controller;
		private List<Feedback> _list;
		private readonly int primitiveLength = new DataTestStorage().feedbackStorage.Count;
		private const string existedId = "F1";
		private const string existedUserId = "U1";
		private const string existedBranchId = "B001";
		private const string notExisted = "ewgewgiwj";

		private readonly List<int> existedRates = new List<int>() { 4, 5 };

		[SetUp]
		public void Setup()
		{
			_feedbackServiceMock = new Mock<IFeedbackService>();
			_list = new DataTestStorage().feedbackStorage;

			_feedbackServiceMock.Setup(s => s.GetAllFeedbacks()).Returns(_list);

			_feedbackServiceMock.Setup(s => s.AddFeedback(It.IsAny<Feedback>())).Callback<Feedback>(feedback => _list.Add(feedback));

			_feedbackServiceMock.Setup(s => s.GetFeedbackByFeedbackId(It.IsAny<string>()))
				   .Returns<string>(id => _list.FirstOrDefault(x => x.FeedbackId == id));

			_feedbackServiceMock.Setup(s => s.GetA_UserFeedbacks(It.IsAny<string>()))
					   .Returns<string>(id => _list.Where(x => x.UserId == id).ToList());

			_feedbackServiceMock.Setup(s => s.GetFeedbacksByRate(It.IsAny<int>()))
					   .Returns<int>(Rate => _list.Where(x => x.Rating == Rate).ToList());

			_feedbackServiceMock.Setup(s => s.GetBranchFeedbacks(It.IsAny<string>()))
					   .Returns<string>(id => _list.Where(x => x.BranchId == id).ToList());

			_feedbackServiceMock.Setup(s => s.DeleteFeedback(It.IsAny<string>()))
					   .Callback<string>(id =>
					   {
						   Feedback deleted = _list.FirstOrDefault(c => c.FeedbackId == id);
						   if (deleted != null)
							   _list.Remove(deleted);
					   });
			_feedbackServiceMock.Setup(s => s.UpdateFeedback(It.IsAny<Feedback>(), It.IsAny<string>()))
				   .Callback<Feedback, string>((newFeedback, id) =>
				   {
					   Feedback previous = _list.FirstOrDefault(x => x.FeedbackId == id);
					   if (previous != null)
					   {
						   if (!newFeedback.Content.IsNullOrEmpty())
							   previous.Content = newFeedback.Content;
						   if (newFeedback.Rating != null)
							   previous.Rating = newFeedback.Rating;
						   previous.Period = newFeedback.Period;
					   }
				   });

			_controller = new FeedbackController(_feedbackServiceMock.Object);
		}

		[Test]
		public async Task GetAllDiscounts_ResultAsPrimitiveLength_ReturnsTrue()
		{
			var actual = await _controller.GetAllFeedbacks();
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<Feedback> list = ok.Value as List<Feedback>;
			Assert.AreEqual(primitiveLength, list.Count);
		}

		[Test]
		public async Task GetUserFeedbacks_InputedExistedUser_ResultOfAList_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.GetFeedbacksByUser(existedUserId);
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<Feedback> tmp = ok.Value as List<Feedback>; 
			Assert.IsNotNull(tmp);
			Assert.IsTrue(tmp.Count > 0);
		}

		[Test]
		public async Task GetBranchFeedbacks_InputedExistedBranch_ResultOfAList_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.GetFeedbacksByBranch(existedBranchId);
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<Feedback> tmp = ok.Value as List<Feedback>;
			Assert.IsNotNull(tmp);
			Assert.IsTrue(tmp.Count > 0);
		}

		[Test]
		public async Task GetFeedbacksByRate_InputedExistedFeedback_ResultOfOk_ReturnsTrue()
		{
            foreach (var item in existedRates)
            {
				var actual = await _controller.GetFeedbacksByRate(item);
				var ok = actual.Result as OkObjectResult;
				Assert.IsInstanceOf<OkObjectResult>(ok);
				List<Feedback> tmp = ok.Value as List<Feedback>;
				Assert.IsNotNull(tmp);
				Assert.IsTrue(tmp.Count > 0);
			}
        }

		[Test]
		public async Task UpdateFeedback_ExecutedByOwn_InputValidData_ResultOfOk_ReturnsTrue()
		{
			int rate = 4;
			string content = "Good";
			Feedback previous = _list.FirstOrDefault(x => x.FeedbackId == existedId);
			Assert.AreNotEqual(previous.Rating, rate);
			Assert.AreNotEqual(previous.Content, content);
			var actual = await _controller.UpdateFeedback(4, "Good", existedId, existedUserId);
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
			Feedback afterUpdated = _list.FirstOrDefault(x => x.FeedbackId == existedId);
			Assert.AreEqual(afterUpdated.Rating, rate);
			Assert.AreEqual(afterUpdated.Content, content);
		}

		[Test]
		public async Task UpdateFeedback_ExecutedByOther_ResultOfBad_ReturnsTrue()
		{
			var actual = await _controller.UpdateFeedback(4, "Good", existedId, notExisted);
			var bad = actual as BadRequestObjectResult;
			Assert.IsInstanceOf<BadRequestObjectResult>(bad);
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(bad.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual("Can't edit others' feedback", response["msg"]);
		}


		[Test]
		public async Task UpdateFeedback_ExecutedByOwn_InputEmpty_DataNoChange_ResultOfOk_ReturnsTrue()
		{
			Feedback previous = _list.FirstOrDefault(x => x.FeedbackId == existedId);
			var actual = await _controller.UpdateFeedback(null, "", existedId, existedUserId);
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
			Feedback afterUpdated = _list.FirstOrDefault(x => x.FeedbackId == existedId);
			Assert.AreEqual(afterUpdated.Rating, previous.Rating);
			Assert.AreEqual(afterUpdated.Content, previous.Content);
		}

		[TearDown]
		public void TearDown()
		{
			_feedbackServiceMock.Reset();
			_controller.Dispose();
		}

	}
}
