using BadmintonCourtAPI.Controllers;
using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices.IService;
using Microsoft.AspNetCore.Http.HttpResults;
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
	internal class BookingControllerTests
	{
		private Mock<IBookingService> _bookingServiceMock;
		private BookingController _controller;
		private List<Booking> _list;
		private readonly int primitiveLength = new DataTestStorage().bookingStorage.Count;
		private const string existedId = "BK001";
		private const string existedUserId = "U1";
		private const int type1Num = 5;
		private const int type2Num = 4;



		[SetUp]
		public void Setup()
		{
			_bookingServiceMock = new Mock<IBookingService>();
			_list = new DataTestStorage().bookingStorage;
			_bookingServiceMock.Setup(s => s.GetAllBookings()).Returns(_list);

			_bookingServiceMock.Setup(s => s.GetBookingByBookingId(It.IsAny<string>()))
					   .Returns<string>(id => _list.FirstOrDefault(x => x.BookingId == id));

			_bookingServiceMock.Setup(s => s.DeleteBooking(It.IsAny<string>()))
					   .Callback<string>(id =>
					   {
						   Booking deleted = _list.FirstOrDefault(c => c.BookingId == id);
						   if (deleted != null)
							   _list.Remove(deleted);
					   });
			_bookingServiceMock.Setup(s => s.UpdatBooking(It.IsAny<Booking>(), It.IsAny<string>()))
				   .Callback<Booking, string>((newBooking, id) =>
				   {
					   Booking previous = _list.FirstOrDefault(x => x.BookingId == id);
					   if (previous != null)
					   {
						   if (newBooking.BookingType != null)
							   previous.BookingType = newBooking.BookingType;
						   if (newBooking.Amount != null)
							   previous.Amount = newBooking.Amount;
					   }
				   });

			_bookingServiceMock.Setup(s => s.GetBookingsByUserId(It.IsAny<string>()))
				.Returns<string>(id => _list.Where(x => x.UserId == id).ToList());

			_bookingServiceMock.Setup(s => s.GetBookingsByType(It.IsAny<int>()))
				.Returns<int>(type => _list.Where(x => x.BookingType == type).ToList());

			_controller = new BookingController(_bookingServiceMock.Object);
		}

		[Test]
		public async Task GetAllDiscounts_ResultAsPrimitiveLength_ReturnsTrue()
		{
			var actual = await _controller.GetAllBookings();
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<Booking> list = ok.Value as List<Booking>;
			Assert.AreEqual(primitiveLength, list.Count);
		}

		[Test]
		public async Task GetBookingById_InputExistedId_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.GetBookingById(existedId);
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			Booking tmp = ok.Value as Booking;
			Assert.IsNotNull(tmp);
		}

		[Test]
		public async Task GetBookingsByUserId_InputExistedId_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.GetBookingsByUserId(existedUserId);
			var ok = actual.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(ok);
			List<Booking> tmp = ok.Value as List<Booking>;
			Assert.IsNotNull(tmp);
			Assert.IsTrue(tmp.Count > 0);
		}

		[Test]
		public async Task GetBookingsByType_ResultOf2Lists_ResultOfOk_ReturnsTrue()
		{
			var first = await _controller.GetBookingsByType(1);
			var firstOk = first.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(firstOk);
			List<Booking> firstTmp = firstOk.Value as List<Booking>;
			Assert.IsTrue(firstTmp.Count == type1Num);
			//--------------------------------
			var second = await _controller.GetBookingsByType(2);
			var secondOk = second.Result as OkObjectResult;
			Assert.IsInstanceOf<OkObjectResult>(secondOk);
			List<Booking> secondTmp = secondOk.Value as List<Booking>;
			Assert.IsTrue(secondTmp.Count == type2Num);
		}

		[Test]
		public async Task DeleteBooking_ResultOfLenghDecreasedBy1_ResultOfOk_ReturnsTrue()
		{
			var actual = await _controller.DeleteBooking(existedId);
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
		
		[Test]
		public async Task UpdateBooking_InputValidFields_ResultOfOk_ReturnsTrue()
		{
			int type = 2;
			float amount = 20000;
			Booking previous = _list.FirstOrDefault(x => x.BookingId == existedId);
			Assert.AreNotEqual(previous.BookingType, type);
			Assert.AreNotEqual(previous.Amount, amount);
			var actual = await _controller.UpdateBooking(type, amount, existedId);
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
			//-------------------------------------------
			Booking afterupdated = _list.FirstOrDefault(x => x.BookingId == existedId);
			Assert.AreEqual(afterupdated.BookingType, type);
			Assert.AreEqual(afterupdated.Amount, amount);
		}


		[Test]
		public async Task UpdateBooking_InputEmpty_ResultOfOk_ReturnsTrue()
		{
			Booking previous = _list.FirstOrDefault(x => x.BookingId == existedId);
			var actual = await _controller.UpdateBooking(null, null, existedId);
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
			//-------------------------------------------
			Booking afterupdated = _list.FirstOrDefault(x => x.BookingId == existedId);
			Assert.AreEqual(afterupdated.BookingType, previous.BookingType);
			Assert.AreEqual(afterupdated.Amount, previous.Amount);
		}


		[TearDown]
		public void TearDown()
		{
			_bookingServiceMock.Reset();
			_controller.Dispose();
		}
	}
}
