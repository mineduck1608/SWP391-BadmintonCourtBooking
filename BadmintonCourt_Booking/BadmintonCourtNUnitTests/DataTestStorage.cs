using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtNUnitTests
{
	public class DataTestStorage
	{
		public List<User> userStorage = new List<User>() {
			// Admin
			new User() { UserId = "U1", AccessFail = 0, ActiveStatus = true, Balance = 0, LastFail = new DateTime(1900, 1, 1, 0, 0, 0), RoleId = "R001"},
			// Branch B001
			// Staff
			new User() { UserId = "U2", AccessFail = 0, ActiveStatus = true, Balance = 0, LastFail = new DateTime(1900, 1, 1, 0, 0, 0), RoleId = "R002", BranchId = "B001"},
			new User() { UserId = "U3", AccessFail = 0, ActiveStatus = true, Balance = 0, LastFail = new DateTime(1900, 1, 1, 0, 0, 0), RoleId = "R002", BranchId = "B002"},
			new User() { UserId = "U4", AccessFail = 0, ActiveStatus = true, Balance = 0, LastFail = new DateTime(1900, 1, 1, 0, 0, 0), RoleId = "R002", BranchId = "B003"},
			// Staff
			new User() { UserId = "U5", AccessFail = 1, ActiveStatus = true, Balance = 0, LastFail = new DateTime(2024, 7, 1, 15, 23, 49), RoleId = "R002", BranchId = "B002"},
			new User() { UserId = "U6", AccessFail = 0, ActiveStatus = true, Balance = 0, LastFail = new DateTime(1900, 1, 1, 0, 0, 0), RoleId = "R002", BranchId = "B003"},
			// Customer
			// Normal case
			new User() { UserId = "U7", AccessFail = 1, ActiveStatus = true, Balance = 0, LastFail = new DateTime(2024, 7, 1, 15, 23, 49), RoleId = "R003"},
			new User() { UserId = "U8", AccessFail = 0, ActiveStatus = true, Balance = 50, LastFail = new DateTime(1900, 1, 1, 0, 0, 0), RoleId = "R003"},
			// Customer
			// Temporary-locked case
			new User() { UserId = "U9", AccessFail = 3, ActiveStatus = false, Balance = 20000, LastFail = new DateTime(2024, 7, 1, 15, 23, 49), RoleId = "R003"},
			// Customer
			// Banned case
			new User() { UserId = "U10", AccessFail = 6, ActiveStatus = false, Balance = 20000, LastFail = new DateTime(2024, 7, 1, 15, 23, 49), RoleId = "R003"},
		};

		public List<UserDetail> userDetailStorage = new List<UserDetail>()
		{
			new UserDetail() { UserId = "U1", FirstName = "Nam", LastName = "Nguyen", Phone = "0932211699", Email = "externalauthdemo1234@gmail.com"},
			new UserDetail() { UserId = "U2", FirstName = "Luu", LastName = "Dang", Phone = "0932211599", Email = "dangl311@gmail.com"},
			new UserDetail() { UserId = "U3", FirstName = "Dark", LastName = "Bu", Phone = "0910283399", Email = "dBuk2@gmail.com"},
			new UserDetail() { UserId = "U4", FirstName = "Iem", LastName = "Diep", Phone = "0977211699", Email = "sonkk23@gmail.com"},
			new UserDetail() { UserId = "U5", FirstName = "Oanh", LastName = "Oem", Phone = "0932211699", Email = "kapba255@gmail.com"},
			new UserDetail() { UserId = "U6", FirstName = "Cuc", LastName = "Suc", Phone = "0932215499", Email = "m4uch13n900@gmail.com"},
			new UserDetail() { UserId = "U7", FirstName = "Gia", LastName = "Bao", Email = "j1j13m3m@gmail.com"},
			new UserDetail() { UserId = "U8", Email = "h0inh4m4y@gmail.com"},
			new UserDetail() { UserId = "U9", Phone = "0939211699", Email = "j13nd0@gmail.com"},
			new UserDetail() { UserId = "U10", FirstName = "Johny", LastName = "Dark", Phone = "0835251799", Email = "jydrk0527@gmail.com"}
		};

		public List<Role> rolseStorage = new List<Role>() {
			new Role() {RoleId = "R001", RoleName = "Admin"},
			new Role() {RoleId = "R002", RoleName = "Staff"},
			new Role() {RoleId = "R003", RoleName = "Customer"},
		};

		public List<CourtBranch> branchStorage = new List<CourtBranch>()
		{
			// Active
			new CourtBranch() { BranchId = "B001", BranchName = "Nha van hoa", BranchStatus = 1, BranchPhone = "0831255640", Location = "Binh Duong" },
			// Closed
			new CourtBranch() { BranchId = "B002", BranchName = "Khu Cong Nghe Cao", BranchStatus = 0, BranchPhone = "0831255622", Location = "Thu Duc" },
			// Maintained
			new CourtBranch() { BranchId = "B003", BranchName = "Hoa Lac", BranchStatus = -1, BranchPhone = "0846250640", Location = "Ha Noi" }
		};

		public List<Court> courtStorage = new List<Court>()
		{ 
			// Branch B001 - active branch
			new Court() { CourtId = "C1", CourtName = "Court 1", BranchId = "B001", CourtStatus = true, Price = 20000, Description = "VIP"}, // active court
			new Court() { CourtId = "C2", CourtName = "Court 2", BranchId = "B001", CourtStatus = false, Price = 25000, Description = "GHevatsao"}, // maintained court
			// Branch B002 - closed branch -> All courts of B002 closed
			new Court() { CourtId = "C3", CourtName = "Court A", BranchId = "B002", CourtStatus = false, Price = 30000, Description = "VIP"},
			new Court() { CourtId = "C4", CourtName = "Court B", BranchId = "B002", CourtStatus = false, Price = 30000, Description = "VIP"},
			// Branch B003 - maintained branch -> All courts temorarily closed for maintainence
			new Court() { CourtId = "C5", CourtName = "Court C", BranchId = "B003", CourtStatus = false, Price = 40000, Description = "VIP"},
			new Court() { CourtId = "C6", CourtName = "Court D", BranchId = "B003", CourtStatus = false, Price = 30000, Description = "VIP"}
		};

		public List<Payment> paymentStorage = new List<Payment>()
		{
			// Once 
			new Payment() { PaymentId = "P001", Amount = 20000, BookingId = "BK001", Date = new DateTime(2024, 7, 1, 15, 23, 49), Method = 1, TransactionId = "311566", UserId = "U1"},  // VnPay
			new Payment() { PaymentId = "P002", Amount = 20000, BookingId = "BK002", Date = new DateTime(2024, 7, 1, 15, 23, 49), Method = 2, TransactionId = "318166", UserId = "U2"},  // Momo
			// Balance
			new Payment() { PaymentId = "P003", Amount = 30000, Date = new DateTime(2024, 7, 1, 15, 23, 49), Method = 1, TransactionId = "311566", UserId = "U3"}, // VnPay
			new Payment() { PaymentId = "P004", Amount = 20000, Date = new DateTime(2024, 7, 1, 15, 23, 49), Method = 2, TransactionId = "311266", UserId = "U4"},  // Momo
			// Fixed
			new Payment() { PaymentId = "P005", Amount = 50000, BookingId = "BK003" ,Date = new DateTime(2024, 7, 1, 15, 23, 49), Method = 1, TransactionId = "31501566", UserId = "U5"}, // VnPay
			new Payment() { PaymentId = "P006", Amount = 100000, BookingId = "BK004", Date = new DateTime(2024, 7, 1, 15, 23, 49), Method = 2, TransactionId = "31121166", UserId = "U6"}  // Momo
		};

		public List<Booking> bookingStorage = new List<Booking>()
		{ 
			// Primitive booking - contribute to create office hours
			new Booking() { BookingId = "BK001", Amount = 0, BookingDate = new DateTime(1900, 1, 1, 0, 0, 0), BookingType = 1, ChangeLog = 0, UserId = "U1" },
			// Once - normal case:
			new Booking() { BookingId = "BK002", Amount = 20000, BookingDate = new DateTime(2024, 7, 1, 15, 23, 49), BookingType = 1, ChangeLog = 0, UserId = "U2" },
			new Booking() { BookingId = "BK003", Amount = 20000, BookingDate = new DateTime(2024, 7, 1, 15, 23, 49), BookingType = 1, ChangeLog = 2, UserId = "U5" },
			// Fixed - normal case:
			new Booking() { BookingId = "BK004", Amount = 50000, BookingDate = new DateTime(2024, 7, 1, 15, 23, 49), BookingType = 2, ChangeLog = 1, UserId = "U6" },
			new Booking() { BookingId = "BK005", Amount = 120000, BookingDate = new DateTime(2024, 6, 1, 15, 23, 49), BookingType = 2, ChangeLog = 1, UserId = "U7" },
			// Delete case:
			new Booking() { BookingId = "BK006", Amount = 50000, BookingDate = new DateTime(2024, 7, 1, 15, 23, 49), BookingType = 1, ChangeLog = 2, UserId = "U8", IsDeleted = true },
			new Booking() { BookingId = "BK007", Amount = 50000, BookingDate = new DateTime(2024, 7, 1, 15, 23, 49), BookingType = 2, ChangeLog = 1, UserId = "U6", IsDeleted = true },
			// Out of attempts to change:
			new Booking() { BookingId = "BK008", Amount = 50000, BookingDate = new DateTime(2024, 7, 1, 15, 23, 49), BookingType = 1, ChangeLog = 2, UserId = "U3" },
			new Booking() { BookingId = "BK009", Amount = 50000, BookingDate = new DateTime(2024, 7, 1, 15, 23, 49), BookingType = 2, ChangeLog = 2, UserId = "U4" }
		};

		public List<BookedSlot> slotStorage = new List<BookedSlot>()
		{
			// Primitive slot - represent office hours - defaults as 7am to 22pm
			new BookedSlot() { SlotId = "S1", BookingId = "BK001", CourtId = "C1", StartTime = new DateTime(1900, 1, 1, 7, 0, 0), EndTime = new DateTime(1900, 1, 1, 22, 0, 0)},
			// Normal case - Playonce-booking type slot
			new BookedSlot() { SlotId = "S2", BookingId = "BK002", CourtId = "C1", StartTime = new DateTime(2024, 7, 7, 7, 0, 0), EndTime = new DateTime(2024, 7, 7, 8, 0, 0)},
			// Normal case - Fixed-bboking type slot
			new BookedSlot() { SlotId = "S3", BookingId = "BK005", CourtId = "C3", StartTime = new DateTime(2024, 6, 3, 7, 0, 0), EndTime = new DateTime(2024, 6, 3, 10, 0, 0)},
			new BookedSlot() { SlotId = "S4", BookingId = "BK005", CourtId = "C3", StartTime = new DateTime(2024, 6, 10, 7, 0, 0), EndTime = new DateTime(2024, 6, 10, 10, 0, 0)},
			// Cancel case:
			new BookedSlot() { SlotId = "S5", BookingId = "BK006", CourtId = "C3", StartTime = new DateTime(2024, 6, 10, 7, 0, 0), EndTime = new DateTime(2024, 6, 10, 10, 0, 0)}
		};


		public List<Feedback> feedbackStorage = new List<Feedback>
		{
			// Admin feedback
			new Feedback() { FeedbackId = "F1", BranchId = "B001", Content = "First comment", Period = new DateTime(2023, 11, 17, 14, 23, 41), Rating = 5, UserId = "U1" }, 
			// Staff feedback
			new Feedback() { FeedbackId = "F2", BranchId = "B003", Content = "We are maintaining this branch. Sorry for our inconvenience", Period = new DateTime(2024, 7, 21, 9, 12, 37), Rating = 5, UserId = "U6" },
			// Customer
			new Feedback() { FeedbackId = "F3", BranchId = "B003", Content = "Good service :D", Period = new DateTime(2024, 2, 6, 19, 32, 37), Rating = 4, UserId = "U2" },
		};

		public List<Discount> discountStorage = new List<Discount>()
		{
			new Discount() { DiscountId = "D1", Amount = 200000, Proportion = 3},
			new Discount() { DiscountId = "D2", Amount = 500000, Proportion = 5}
		};
	}
}
