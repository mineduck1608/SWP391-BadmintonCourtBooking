using BadmintonCourtBusinessObjects.Entities;
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

		public static bool IsPhoneFormatted(string phone) => !phone.IsNullOrEmpty() ? new Regex(@"^0[9832]\d{8}$").IsMatch(phone) : false;

		public static bool IsPasswordSecure(string password) => !password.IsNullOrEmpty() ? new Regex(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=<>?])[A-Za-z\d!@#$%^&*()_\-+=<>?]{12,}$").IsMatch(password) : false;

		public static bool IsMailFormatted(string mail) => !mail.IsNullOrEmpty() ? new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").IsMatch(mail) : false;

		public static string GenerateToken(string id, string lastName, string username, string roleName, IConfiguration config)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			if (username.IsNullOrEmpty())
				username = "";
			if (lastName.IsNullOrEmpty())
				lastName = "";
			var claims = new[]
			{
				new Claim("UserId", id.ToString()),
				//new Claim(ClaimTypes.NameIdentifier, username),
				
				new Claim("Username" , username),
				//new Claim(ClaimTypes.Surname, lastName),
				new Claim("Lastname", lastName),
				//new Claim(ClaimTypes.Role, roleName)
				new Claim("Role", roleName)
			};
			var token = new JwtSecurityToken(
				issuer: config["Jwt:Issuer"],
				audience: config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(15),
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

		public static DateTime CustomizeDate(int period) => new DateTime(1900, 1, 1, period, 0, 0);

		public static List<BookedSlotSchema> FormatSlotList(List<BookedSlot> slots)
		{
			List <BookedSlotSchema> result = new List<BookedSlotSchema>();
			foreach (var slot in slots)
				result.Add(new BookedSlotSchema { BookedSlotId = slot.SlotId, BookingId = slot.BookingId, Date = new DateOnly(slot.StartTime.Year, slot.StartTime.Month, slot.StartTime.Day), Start = slot.StartTime.Hour, End = slot.EndTime.Hour, CourtId = slot.CourtId });
			return result;
		}

		public static bool ArePricesValid(double? min, double? max) => min < max;

		public static bool IsLeapYear(int? year) => year == null ? false : year % 4 == 0 || year % 400 == 0;

		public static List<DashboardResponseDTO> GenerateYearDashboard(int year, List<Payment> pList)
		{
			//-----------------------------------------------------
			List<DashboardResponseDTO> result = new List<DashboardResponseDTO>();
			//--------------------------------------------------
			bool isLeapYear = IsLeapYear(year);
            //--------------------------------------------------
            for (int i = 1; i <= 12; i++)
            {
				float amount = 0;
				int endDay = (i == 1 || i == 3 || i == 5 || i == 7 || i == 8 || i == 10 || i == 12) ? 31 : /* Check 31 ngày */
					((i == 4 || i == 6 || i == 9 || i == 1) ? 30 : /* Check 30 ngày */
					(isLeapYear == true ? 29 : 28)); /* Check năm nhuần tháng 2*/
				DateTime startDate = new DateTime(year, i, 1, 0, 0, 0);
				DateTime endDate = new DateTime(year, i, endDay, 23, 59, 59);
				List<Payment> tmpStorage = pList.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
				foreach (var item in tmpStorage)
					amount += float.Parse(item.Amount.ToString());
                result.Add(new DashboardResponseDTO { Amount = amount, Period = $"{i}/{year}"});
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
				//----------------------------------------------------------------------------
				if (i % 4 != 0) // Ko phải tuần 4 - tuần cuối của tháng
				{
					endDate = new DateTime(year, startMonth, begin + 6, 23, 59, 59);
					begin += 7;
				}
				else // Tuần cuối của tháng - tuần 4
				{
					int endDay = (startMonth == 1 || startMonth == 3 || startMonth == 5 || startMonth == 7 || startMonth == 8 || startMonth == 10 || startMonth == 12) ? 31 : /* Check 31 ngày */
					((startMonth == 4 || startMonth == 6 || startMonth == 9 || startMonth == 1) ? 30 : /* Check 30 ngày */
					(isLeapYear == true ? 29 : 28)); /* Check năm nhuần tháng 2*/
					endDate = new DateTime(year, startMonth, endDay, 23, 59, 59);
					//----------------------------------------------------------------------------
					startMonth++; // Qua tháng mới
					begin = 1; // Reset ngày băt đầu về lại từ đầu
				}
				List<Payment> tmpStorage = pList.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
				foreach (var item in tmpStorage)
					amount += float.Parse(item.Amount.ToString());
				result.Add(new DashboardResponseDTO { Amount = amount, Period = $"{startMonth}/{year} - Week {week}" });
				if (week == 4)
					week = 1;
				else week += 1;
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
			((month == 4 || month == 6 || month == 9 || month == 1) ? 30 : /* Check 30 ngày */
			(isLeapYear == true ? 29 : 28)); /* Check năm nhuần tháng 2*/
			//----------------------------------------
			DateTime startDate = new DateTime(year, month, begin, 0, 0, 0);
			DateTime endDate = new DateTime(year, month, begin, 23, 59, 59);
			List<DashboardResponseDTO> result = new List<DashboardResponseDTO>();
			for (int i = begin; i <= endDay; i++)
            {
				float amount = 0;
				List<Payment> tmpStorage = pList.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
				foreach (var item in tmpStorage)
					amount += float.Parse(item.Amount.ToString());
				result.Add(new DashboardResponseDTO { Amount = amount, Period = $"{i}/{month}/{year}" });
				startDate = startDate.AddDays(1);
				endDate = endDate.AddDays(1);
			}
			return result;
        }

	}
}
