using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtBusinessObjects.SupportEntities.Court;
using BadmintonCourtBusinessObjects.SupportEntities.Slot;
using BadmintonCourtBusinessObjects.SupportEntities.Statistic;
using BadmintonCourtServices;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using NuGet.Versioning;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BadmintonCourtAPI.Utils
{
	public class Util
	{

		private static IConfiguration _config = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", true, true).Build();
		static readonly HttpClient client = new HttpClient();

		public static bool IsPhoneFormatted(string phone) => !phone.IsNullOrEmpty() ? new Regex(@"^0[9832]\d{8}$").IsMatch(phone) : false;

		public static bool IsPasswordSecure(string password) => !password.IsNullOrEmpty() ? new Regex(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=<>?])[A-Za-z\d!@#$%^&*()_\-+=<>?]{12,}$").IsMatch(password) : false;

		public static bool IsMailFormatted(string mail) => !mail.IsNullOrEmpty() ? new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").IsMatch(mail) : false;

		public static string GenerateToken(string id, bool status, string username, string roleName, int type)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			if (username.IsNullOrEmpty())
				username = "";
			var claims = new[]
			{
				new Claim("UserId", id.ToString()),
				//new Claim(ClaimTypes.NameIdentifier, username),
				
				new Claim("Username" , username),
				//new Claim(ClaimTypes.Surname, lastName),
				new Claim("Status", status.ToString()),
				new Claim(ClaimTypes.Role, roleName),
				new Claim("Role", roleName)
			};
			int duration;
			if (type == 1) // Để đăng nhập
			{
				if (roleName != "Customer") // Loại đăng nhập - ko phải khác
					duration = 480; // Expire trong vòng 1 tiếng
				else  duration = 60;  // Admin/nhân viên thì 8 tiếng
			}
			else duration = 15; // Dùng chung cho các loại như quên pass, update mail, đkí acc mới ko quan tâm role - mặc định 15'
			// Cho mặc định các loại ngoài login là 0
            var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(duration),
				signingCredentials: credentials
				);
			
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public static string GetMailFromToken(string token)
		{
			var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
			return jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email").Value;
		}

		public static string ToHashString(string s)
		{
			using (SHA256 sha256Hash = SHA256.Create())
			{
				byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(s));
				StringBuilder result = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
					result.Append(bytes[i].ToString("x2"));
				return result.ToString();
			}
		}

		public static List<BookedSlotSchema> FormatSlotList(List<BookedSlot> slots)
		{
			List<BookedSlotSchema> result = new List<BookedSlotSchema>();
			foreach (var slot in slots)
				result.Add(new BookedSlotSchema { BookedSlotId = slot.SlotId, BookingId = slot.BookingId, Date = new DateOnly(slot.StartTime.Year, slot.StartTime.Month, slot.StartTime.Day), Start = slot.StartTime.Hour, End = slot.EndTime.Hour, CourtId = slot.CourtId });
			return result;
		}

		public static List<CourtDTO> FormatCourtList(List<Court> courts)
		{
			if (courts.Count == 0)
				return new List<CourtDTO>();

			List<CourtDTO> result = new List<CourtDTO>();
			foreach (var item in courts)
			{
				string[] components = item.CourtImg.Split('|');
				List<string> courtImg = new List<string>();
				for (int i = 0; i < components.Length; i++)
					courtImg.Add($"Image {i + 1}:{components[i]}");

				result.Add(new CourtDTO
				{
					CourtId = item.CourtId,
					CourtImg = courtImg,
					BranchId = item.BranchId,
					CourtName = item.CourtName,
					CourtStatus = item.CourtStatus,
					Description = item.Description,
					Price = item.Price
				});
			}
			return result;
		}

		public static List<BranchDTO> FormatBranchList(List<CourtBranch> branches)
		{
			List<BranchDTO> result = new List<BranchDTO>();
			foreach (var item in branches)
			{
				string[] components = item.BranchImg.Split('|');
				List<string> courtImg = new List<string>();
				for (int i = 0; i < components.Length; i++)
					courtImg.Add($"Image {i + 1}: {components[i]}");

				result.Add(new BranchDTO
				{
					BranchImg = courtImg,
					BranchId = item.BranchId,
					BranchName = item.BranchName,
					BranchStatus = item.BranchStatus,
					BranchPhone = item.BranchPhone,
					Location = item.Location,
					MapUrl = item.MapUrl,
				});
			}
			return result;
		}

		public static bool ArePricesValid(double? min, double? max) => min < max;

		public static bool IsLeapYear(int? year)
		{
			if (year == null || year % 4 != 0 || year < 0)
				return false;

			if (year % 100 != 0)
				return true;
			else
			{
				if (year % 400 == 0)
					return true;
				return false;
			}
		}

		public static List<DashboardResponseDTO> GenerateYearDashboard(int year, List<Payment> pList)
		{
			//-----------------------------------------------------
			List<DashboardResponseDTO> result = new List<DashboardResponseDTO>();
			//--------------------------------------------------
			for (int i = 1; i <= 12; i++)
			{
				float amount = 0;
				List<Payment> tmpStorage = pList.Where(x => x.Date.Month == i).ToList();
				foreach (var item in tmpStorage)
					amount += float.Parse(item.Amount.ToString());
				result.Add(new DashboardResponseDTO { Amount = amount, Period = $"{i}/{year}" });
			}
			return result;
		}

		public static List<DashboardResponseDTO> GenerateMonthDashboard(int year, int startMonth, int numMonths, List<Payment> pList)
		{
			int length = numMonths * 4;
			List<DashboardResponseDTO> result = new List<DashboardResponseDTO>();
			bool isLeapYear = IsLeapYear(year);
			int begin = 1;
			int week = 1;
			//--------------------------------------------------
			for (int i = 1; i <= length; i++)
			{
				float amount = 0;
				DateTime startDate = new DateTime(year, startMonth, begin, 0, 0, 0);
				DateTime endDate;
				string period;
				//----------------------------------------------------------------------------
				if (i % 4 != 0) // Ko phải tuần 4 - tuần cuối của tháng
				{
					//endDate = new DateTime(year, startMonth, begin + 6, 23, 59, 59);
					endDate = startDate.AddDays(7);
				}
				else // Tuần cuối của tháng - tuần 4
				{
					int endDay = (startMonth == 1 || startMonth == 3 || startMonth == 5 || startMonth == 7 || startMonth == 8 || startMonth == 10 || startMonth == 12) ? 31 : /* Check 31 ngày */
					((startMonth == 4 || startMonth == 6 || startMonth == 9 || startMonth == 11) ? 30 : /* Check 30 ngày */
					(isLeapYear == true ? 29 : 28)); /* Check năm nhuần tháng 2*/
					endDate = new DateTime(year, startMonth, endDay, 23, 59, 59);
					//----------------------------------------------------------------------------
				}
				List<Payment> tmpStorage = pList.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
				foreach (var item in tmpStorage)
					amount += float.Parse(item.Amount.ToString());
				result.Add(new DashboardResponseDTO { Amount = amount, Period = $"{startMonth}/{year} - Week {week}" });
				if (week == 4)
				{
					week = 1;
					startMonth++; // Qua tháng mới
					begin = 1; // Reset ngày băt đầu về lại từ đầu
				}
				else
				{
					week += 1;
					begin += 7;
				}
			}
			return result;
		}

		public static List<DashboardResponseDTO> GenerateWeekDashboard(int year, int month, int week, List<Payment> pList)
		{
			int begin = 0;
			if (week == 1)
				begin = 1;
			else if (week == 2)
				begin = 8;
			else if (week == 3)
				begin = 15;
			else begin = 22;

			//----------------------------------------
			bool isLeapYear = IsLeapYear(year);
			int endDay = 0;
			if (begin <= 15) // Tuần 1, 2, 3
				endDay = begin + 6;
			else endDay = (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) ? 31 : /* Check 31 ngày */
			((month == 4 || month == 6 || month == 9 || month == 11) ? 30 : /* Check 30 ngày */
			(isLeapYear == true ? 29 : 28)); /* Check năm nhuần tháng 2*/
			//----------------------------------------
			//DateTime startDate = new DateTime(year, month, begin, 0, 0, 0);
			//DateTime endDate = new DateTime(year, month, begin, 23, 59, 59);
			List<DashboardResponseDTO> result = new List<DashboardResponseDTO>();
			for (int i = begin; i <= endDay; i++)
			{
				float amount = 0;
				//List<Payment> tmpStorage = pList.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
				List<Payment> tmpStorage = pList.Where(x => x.Date.Year == year && x.Date.Month == month && x.Date.Day == i).ToList();
				foreach (var item in tmpStorage)
					amount += float.Parse(item.Amount.ToString());
				result.Add(new DashboardResponseDTO { Amount = amount, Period = $"{i}/{month}/{year}" });
				//startDate = startDate.AddDays(1);
				//endDate = endDate.AddDays(1);
			}
			return result;
		}

	}
}
