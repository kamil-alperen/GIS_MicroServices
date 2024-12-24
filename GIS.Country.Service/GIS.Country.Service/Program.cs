using System.Text;
using GIS.City.Service.Middlewares;
using GIS.Common.Repositories;
using GIS.Country.Service.Entities;

namespace GIS.Country.Service
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
                .AddMongoRepository<CountryEntity>("info");

            builder.Services.AddAuthenticationSettings();

            builder.Services.AddRabbitMQService();

            builder.Services.AddExceptionHandler<ExceptionHandlerMiddleware>();
            builder.Services.AddProblemDetails();

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
