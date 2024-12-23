
using GIS.City.Service.Entities;
using GIS.Common.Repositories;
using Polly;
using Polly.Timeout;
using MassTransit;
using GIS.City.Service.Settings;
using GIS.Common.Settings;
using System.Reflection;

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

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddMongoDatabase()
                .AddMongoRepository<CityEntity>("info");

            builder.Services.AddAuthenticationSettings();

            builder.Services.AddRabbitMQService();

            var services = builder.Services;

            /*builder.Services.AddHttpClient<CountryClient>(opt =>
            {
                opt.BaseAddress = new Uri("https://localhost:44330");
            }).AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
                5,
                retryAttempt =>
                {
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                },
                onRetry: (outcome, timespan, retryAttempt, _) =>
                {
                    ServiceProvider serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CountryClient>>().LogInformation($"CountryClient : Retry_{retryAttempt} attempt is made");
                }
            )).AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(
                TimeSpan.FromSeconds(1),
                TimeoutStrategy.Pessimistic
            ));*/

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


            app.MapControllers();

            app.Run();
        }
    }
}
