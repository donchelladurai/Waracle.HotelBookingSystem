using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Waracle.HotelBookingSystem.Application.QueryHandlers;
using Waracle.HotelBookingSystem.Data.Repositories;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Infrastructure.DatabaseContexts;
using Waracle.HotelBookingSystem.Web.Api.ModelBinders;

namespace Waracle.HotelBookingSystem
{
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

            builder.Services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new BritishDateTimeModelBinderProvider());
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); 

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
