using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtServices;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BadmintonCourtAPI.Utils
{
	public class Util
	{
		public static string GenerateUserId(BadmintonCourtService service) => service.userService.GetAllUsers() != null ? "U" + (service.userService.GetAllUsers().Count() + 1).ToString("D7") : "U0000001";
		
			
		

		public static string GenerateBranchId(BadmintonCourtService service)
		{
			string number = $"{service.courtBranchService.GetAllCourtBranches().Count()}";
			int length = number.Length;
			while (length < 3)
			{
				string tmp = $"0{number}";
				number = tmp;
				length++;
			}
			return $"B{number}";
		}

		public static string GenerateCourtId(BadmintonCourtService service)
		{
			string number = $"{service.courtService.GetAllCourts().Count()}";
			int length = number.Length;
			while (length < 3)
			{
				string tmp = $"0{number}";
				number = tmp;
				length++;
			}
			return $"C{number}";
		}

		public static string GenerateSlotId(BadmintonCourtService service)
		{
			string number = $"{service.slotService.GetAllSlots().Count()}";
			int length = number.Length;
			while (length < 6)
			{
				string tmp = $"0{number}";
				number = tmp;
				length++;
			}
			return $"S{number}";
		}

		public static string GenerateBookingId(BadmintonCourtService service)
		{
			string number = $"{service.bookingService.GetAllBookings().Count()}";
			int length = number.Length;
			while (length < 7)
			{
				string tmp = $"0{number}";
				number = tmp;
				length++;
			}
			return $"BK{number}";
		}

		public static string GeneratPaymentId(BadmintonCourtService service)
		{
			string number = $"{service.paymentService.GetAllPayments().Count()}";
			int length = number.Length;
			while (length < 7)
			{
				string tmp = $"0{number}";
				number = tmp;
				length++;
			}
			return $"P{number}";
		}

		public static string GenerateFeedbackId(BadmintonCourtService service) => "F" + (service.userService.GetAllUsers().Count() + 1).ToString("D2");



		public static string GenerateRoleId(BadmintonCourtService service)
		{
			string number = $"{service.roleService.GetAllRoles().Count()}";
			int length = number.Length;
			while (length < 3)
			{
				string tmp = $"0{number}";
				number = tmp;
				length++;
			}
			return $"R{number}";
		}

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

		public static bool ArePricesValid(double? min, double? max) => min < max;
	}
}
