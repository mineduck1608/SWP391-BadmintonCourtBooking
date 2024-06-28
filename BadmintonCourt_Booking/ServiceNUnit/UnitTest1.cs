using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ServiceNUnit
{
	public class Tests
	{
		private RoleService _rService;
		private UserService _uService;
		private CourtService _cService;
		private PaymentService _pService;
		private SlotService _sService;
		private DiscountService _dService;


		[SetUp]
		public void Setup()
		{
			_rService = new RoleService();	
			_uService = new UserService();
			_cService = new CourtService();
			_pService = new PaymentService();
			_sService = new SlotService();
			_dService = new DiscountService();
		}

		[Test]
		public void TestNumRoles_Input3_ReturnsTrue()
		{
			
			Assert.That(_rService.GetAllRoles().Count(), Is.EqualTo(3), "Passed");
		}

		[Test]
		public void TestAdminRole_InputStaff_ReturnsFalse()
		{
			string roleId = _uService.GetUserById("U1").RoleId;
			string role = _rService.GetRoleById(roleId).RoleName;
			bool status = role == "Staff"; 
			Assert.That(status, Is.False, "Passed");
		}

		[Test]
		public void TestCustomerBranch_BranchNotEmpty_ReturnsFalse()
		{
			List<User> uList = _uService.GetUsersByRole("Customer").Where(x => !x.BranchId.IsNullOrEmpty()).ToList();
			Assert.That(uList.Count > 0, Is.False, "Passed");
		}

		[Test]
		public void CompareACourtPrice_InputNegative_ReturnsFalse()
		{
			int randIndex = new Random().Next(0, _cService.GetAllCourts().Count() - 1);
			Court court = _cService.GetAllCourts()[randIndex];
			bool status = court.Price < 0;
			Assert.That(status, Is.False, "Passed");
		}

		[Test]
		public void FindAnyPaymentAmountLittleThanZero_InputNegative_ReturnsFalse()
		{
			List<Payment> pList = _pService.GetAllPayments().Where(x => x.Amount < 0).ToList();
			Assert.That(pList.Count > 0, Is.False, "Passed");
		}

		[Test]
		public void FindAnyUsersWithSixFailsAndStatusTrue_ReturnsFalse()
		{
			List<User> uList = _uService.GetAllUsers().Where(x => x.AccessFail == 6 && x.ActiveStatus == true).ToList();
			Assert.That(uList.Count() > 0, Is.False, "Passed");
		}

		[Test]
		public void FindAnySlotWithNoneBookingId_ReturnsFalse()
		{
			List<BookedSlot> sList = _sService.GetAllSlots().Where(x => x.BookingId.IsNullOrEmpty()).ToList();
			Assert.That(sList.Count > 0, Is.False, "Passed");
		}

		[Test]
		public void FindAnyPaymentWithMethodOutOfSupporting_ReturnsFalse()
		{
			List<Payment> pList = _pService.GetAllPayments().Where(x => x.Method != 1 && x.Method != 2).ToList();
			Assert.That(pList.Count > 0, Is.False, "Passed");
		}

		[Test]
		public void FindAnyDiscountWithProportion_CheckNegative_ReturnsFalse()
		{
			List<Discount> dList = _dService.GetAllDiscounts().Where(x => x.Proportion <= 0).ToList();
			Assert.That(dList.Count > 0, Is.False, "Passed");
		}

		[Test]
		public void FindAnySlotsWithIntervalHoursLittleThanOne_InputLittleThanOne_ReturnsFalse()
		{
			List<BookedSlot> sList = _sService.GetAllSlots().Where(x => (x.EndTime - x.StartTime).TotalHours < 1).ToList();
			Assert.That(sList.Count > 0, Is.False, "Passed");
		}
	}
}