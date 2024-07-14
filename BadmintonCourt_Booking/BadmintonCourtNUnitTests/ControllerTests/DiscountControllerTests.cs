using BadmintonCourtAPI.Controllers;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtBusinessObjects.SupportEntities.Court;
using BadmintonCourtServices.IService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BadmintonCourtNUnitTests.ControllerTests
{
	internal class DiscountControllerTests
	{

		private Mock<IDiscountService> _discountServiceMock;
		private DiscountController _controller;
		private List<Discount> _list;
		private readonly int primitiveLength = new DataTestStorage().discountStorage.Count;
		private const string existedId = "D1";
		private const float existedAmount = 200000;
		private const float existedProportion = 3;

		[SetUp]
		public void Setup()
		{
			_discountServiceMock = new Mock<IDiscountService>();
			_list = new DataTestStorage().discountStorage;
			_discountServiceMock.Setup(s => s.GetAllDiscounts()).Returns(_list);

			_discountServiceMock.Setup(s => s.AddDiscount(It.IsAny<Discount>())).Callback<Discount>(discount => _list.Add(discount));

			_discountServiceMock.Setup(s => s.GetDiscountById(It.IsAny<string>()))
					   .Returns<string>(id => _list.FirstOrDefault(x => x.DiscountId == id));

			_discountServiceMock.Setup(s => s.DeleteDiscount(It.IsAny<string>()))
					   .Callback<string>(id =>
					   {
						   Discount deleted = _list.FirstOrDefault(c => c.DiscountId == id);
						   if (deleted != null)
							   _list.Remove(deleted);
					   });
			_discountServiceMock.Setup(s => s.UpdateDiscount(It.IsAny<Discount>(), It.IsAny<string>()))
				   .Callback<Discount, string>((newDiscount, id) =>
				   {
					   Discount previous = _list.FirstOrDefault(x => x.DiscountId == id);
					   if (previous != null)
					   {
							previous.Amount = newDiscount.Amount;
						   previous.Proportion = newDiscount.Proportion;
					   }
				   });

			_controller = new DiscountController(_discountServiceMock.Object);
		}

		[Test]
		public async Task GetAllDiscounts_ResultAsPrimitiveLength_ReturnsTrue()
		{
			var actual = await _controller.GetAllDiscounts();
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<Discount> list = ok.Value as List<Discount>;
			Assert.AreEqual(primitiveLength, list.Count);
		}

		[Test]
		public async Task GetDiscountById_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.GetDiscountById(existedId);
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			Discount tmp = ok.Value as Discount;
			Assert.IsNotNull(tmp);
			Assert.AreEqual(existedAmount, tmp.Amount);
			Assert.AreEqual(tmp.Proportion, existedProportion);
		}

		[Test]
		public async Task CreateDiscount_InputValidFields_ResultOfLengthAddedBy1_ResultOfOk_ReturnsTrue()
		{
			float amount = new Random().Next(10000, 100000);
			float proportion = new Random().Next(0, 100);
			var actual = await _controller.CreateDiscount(amount, proportion);
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
		public async Task CreateDiscount_InputEmpty_ResultOfLengthAddedBy1_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.CreateDiscount(null, null);
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
			Assert.AreEqual(0, _list[primitiveLength].Amount);
			Assert.AreEqual(0, _list[primitiveLength].Proportion);
		}

		[Test]
		public async Task CreateDiscount_InputNegative_ResultOfLengthAddedBy1_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.CreateDiscount(-1, -1);
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
			Assert.AreEqual(0, _list[primitiveLength].Amount);
			Assert.AreEqual(0, _list[primitiveLength].Proportion);
		}

		[Test]
		public async Task UpdateDiscount_InputValidFields_ResultOfOk_ReturnsTrue()
		{
			float amount = new Random().Next(10000, 100000);
			float proportion = new Random().Next(0, 100);
			var actual = await _controller.UpdateDiscount(existedId, amount, proportion, null);
			var ok = actual as OkObjectResult;
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(ok.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual("Success", response["msg"]);
			Discount tmp = _list.FirstOrDefault(x => x.DiscountId == existedId);
			Assert.IsNotNull(tmp);
			Assert.AreEqual(tmp.Amount, amount);
			Assert.AreEqual(tmp.Proportion, proportion);
		}

		[Test]
		public async Task UpdateDiscount_InputEmptyAndNegative_ResultOfNoChangeInData_ResultOfOk_ReturnsTrue()
		{
			Discount origin = _list.FirstOrDefault(x => x.DiscountId == existedId);
			var actual = await _controller.UpdateDiscount(existedId, null, -1, null);
			var ok = actual as OkObjectResult;
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var jsonString = System.Text.Json.JsonSerializer.Serialize(ok.Value);
			var response = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
			Assert.IsTrue(response.ContainsKey("msg"));
			Assert.AreEqual("Success", response["msg"]);
			Discount tmp = _list.FirstOrDefault(x => x.DiscountId == existedId);
			Assert.IsNotNull(tmp);
			Assert.AreEqual(tmp.Amount, origin.Amount);
			Assert.AreEqual(tmp.Proportion, origin.Proportion);
		}

		[Test]
		public async Task RemoveDiscount_ResultOfLenghDecreasedBy1_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.RemoveDiscount(existedId);
			var ok = actual as OkObjectResult;
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

		[TearDown]
        public void TearDown()
		{
			_discountServiceMock.Reset();
			_controller.Dispose();
		}
	}
}
