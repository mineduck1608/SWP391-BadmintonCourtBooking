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
			if (isTesting)
			{
				builder.Services.AddDbContext<BadmintonCourtContext>(options =>
					options.UseInMemoryDatabase("InMemoryDb"));
			}
			else
			{
				// Add your production database context here
				// builder.Services.AddDbContext<BadmintonCourtContext>(options =>
				//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
			}

			// Configure CORS policy
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", policyBuilder =>
				{
					policyBuilder.AllowAnyOrigin()
								 .AllowAnyMethod()
								 .AllowAnyHeader()
								 .AllowCredentials()
								 .WithOrigins("");
				});
			});

			// Configure Swagger/OpenAPI
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

			// Configure authentication
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

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			// Authentication must come before Authorization
			app.UseAuthentication();
			app.UseAuthorization();

			// CORS policy must be applied before the endpoints are mapped
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
