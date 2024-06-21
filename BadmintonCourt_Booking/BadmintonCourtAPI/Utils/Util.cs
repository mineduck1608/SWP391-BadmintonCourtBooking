using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtBusinessObjects.SupportEntities.Slot;
using BadmintonCourtServices;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
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

		public static bool IsPhoneFormatted(string phone) => phone != null ? new Regex(@"\d{10}").IsMatch(phone) : false ;

		public static bool IsPasswordSecure(string password) => password != null ? new Regex(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=<>?])[A-Za-z\d!@#$%^&*()_\-+=<>?]{12,}").IsMatch(password) : false;

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
				expires: DateTime.Now.AddMinutes(30),
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
	}
}
