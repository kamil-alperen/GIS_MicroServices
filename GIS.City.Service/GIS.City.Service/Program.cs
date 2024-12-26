
using GIS.City.Service.Entities;
using GIS.Common.Repositories;
using Polly;
using Polly.Timeout;
using MassTransit;
using GIS.City.Service.Settings;
using GIS.Common.Settings;
using System.Reflection;
using GIS.City.Service.Middlewares;
using GIS.City.Service.Client;

namespace GIS.City.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure logging settings

            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // Add services to the container.

            builder.Services.AddMongoDatabase()
                .AddMongoRepository<CityEntity>("info");

            builder.Services.AddAuthenticationSettings();

            builder.Services.AddRabbitMQService();

            builder.Services.AddExceptionHandler<ExceptionHandlerMiddleware>();
            builder.Services.AddProblemDetails();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddHttpClient<CountryClient>(opt =>
            {
                opt.BaseAddress = new Uri("https://localhost:44330");
            }).AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(15));

            builder.Services.AddControllers();
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

            app.UseAuthentication();

            app.UseAuthorization();

            //app.UseMiddleware<CustomExceptionHandlerMiddleware>();

            app.UseExceptionHandler();

            app.MapControllers();

            app.Run();
        }
    }
}
