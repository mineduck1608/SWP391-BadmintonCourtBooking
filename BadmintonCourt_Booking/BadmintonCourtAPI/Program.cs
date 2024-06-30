
//using BadmintonCourtServices;
//using BadmintonCourtServices.IService;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.Google;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Configuration;
//using System.Text;
//namespace BadmintonCourtAPI
//{
//	public class Program
//	{
//		public static void Main(string[] args)
//		{

//			var builder = WebApplication.CreateBuilder(args);


//			// Add services to the container.

//			builder.Services.AddControllers();
//			builder.Services.AddCors(options =>
//			{
//				options.AddPolicy("AllowAll", builder =>
//				{
//					builder.AllowAnyOrigin()
//						   .AllowAnyMethod()
//						   .AllowAnyHeader();
//				});
//			});
//			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//			builder.Services.AddEndpointsApiExplorer();
//			builder.Services.AddSwaggerGen();


//			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
//			{
//				options.TokenValidationParameters = new TokenValidationParameters
//				{
//					ValidateIssuer = true,
//					ValidateAudience = true,
//					ValidateLifetime = true,
//					ValidateIssuerSigningKey = true,
//					ValidIssuer = builder.Configuration["Jwt:Issuer"],
//					ValidAudience = builder.Configuration["Jwt:Audience"],
//					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//				};
//			});
//			builder.Services.AddRazorPages();
//			builder.Services.AddMvc(); 

//			builder.Services.AddSingleton<IVnPayService, VnPayService>();

//			var app = builder.Build();

//			if (app.Environment.IsDevelopment())
//			{
//				app.UseSwagger();
//				app.UseSwaggerUI();
//				app.UseDeveloperExceptionPage();
//			}

//			app.UseHttpsRedirection();
//			app.UseStaticFiles();

//			app.UseAuthentication();

//			app.UseRouting();

//			// Place app.UseAuthorization() before app.UseEndpoints(...)
//			app.UseAuthorization();
//			app.UseCors("AllowAll");


//			app.UseEndpoints(endpoints =>
//			{
//				endpoints.MapControllers();
//				endpoints.MapRazorPages();
//			});

//			app.Run();

//		}
//	}
//}



using BadmintonCourtBusinessDAOs;
using BadmintonCourtServices;
using BadmintonCourtServices.IService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Configuration;
using System.Text;

namespace BadmintonCourtAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers();
			var isTesting = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
			//if (isTesting)
			//{
			//	builder.Services.AddDbContext<BadmintonCourtContext>(options =>
			//		options.UseInMemoryDatabase("InMemoryDb"));
			//}

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", builder =>
				{
					builder.AllowAnyOrigin()
						   .AllowAnyMethod()
						   .AllowAnyHeader();
				});
			});

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(options =>
			{
				options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
				{
					Description = "Standard Authorization header using Bearer scheme",
					In = ParameterLocation.Header,
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey
				});

				options.OperationFilter<SecurityRequirementsOperationFilter>();
			});

			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = builder.Configuration["Jwt:Issuer"],
					ValidAudience = builder.Configuration["Jwt:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
				};
			});

			builder.Services.AddRazorPages();
			builder.Services.AddMvc();

			builder.Services.AddSingleton<IVnPayService, VnPayService>();

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseAuthentication();

			app.UseRouting();

			// Place app.UseAuthorization() before app.UseEndpoints(...)
			app.UseAuthorization();
			app.UseCors("AllowAll");

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapRazorPages();
			});

			app.Run();
		}
	}
}
