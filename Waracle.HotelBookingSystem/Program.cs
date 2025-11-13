using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Waracle.HotelBookingSystem.Application.QueryHandlers;
using Waracle.HotelBookingSystem.Data.Repositories;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Infrastructure.DatabaseContexts;
using Waracle.HotelBookingSystem.Web.Api.Middleware;

namespace Waracle.HotelBookingSystem
{
    using System.Text;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddLogging();

            var connString = builder.Configuration.GetConnectionString("HotelsDbConnection");
            builder.Services.AddDbContext<AzureSqlHbsDbContext>(options => options.UseSqlServer(connString));
            
            builder.Services.AddScoped<IHotelsRepository, HotelRepository>();
            builder.Services.AddScoped<IRoomsRepository, RoomsRepository>();
            builder.Services.AddScoped<IBookingsRepository, BookingsRepository>();

            builder.Services.AddApplicationInsightsTelemetry();

            builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetHotelsByNameQueryHandler).Assembly));

            
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                                                                {
                                                                    ValidateIssuer = true,
                                                                    ValidateAudience = true,
                                                                    ValidateLifetime = true,
                                                                    ValidateIssuerSigningKey = true,
                                                                    ValidIssuer = jwtSettings["Issuer"],
                                                                    ValidAudience = jwtSettings["Audience"],
                                                                    IssuerSigningKey = new SymmetricSecurityKey(key)
                                                                };
                    });

            // Logging
            builder.Logging.AddApplicationInsights(
                configureTelemetryConfiguration: (config) => config.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"],
                configureApplicationInsightsLoggerOptions: (options) => { }
            );

            var appInsightsLogLevelString = builder.Configuration["ApplicationInsights:LogLevel"];
            if (Enum.TryParse<LogLevel>(appInsightsLogLevelString, true, out var appInsightsLogLevel))
            {
                builder.Logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("", appInsightsLogLevel);
            }
            else
            {
                builder.Logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("", LogLevel.Information);
            }

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Don Chelladurai's Hotel Booking System API - Developed as part of an interview process",
                    Version = "v1.0",
                    Description = "An API for managing hotel bookings",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Don Chelladurai",
                        Email = "donchelladurai@gmail.com"
                    }
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
