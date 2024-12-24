using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using GIS.Common.Settings;
using Microsoft.Extensions.Configuration;
using MassTransit;
using GIS.City.Service.Settings;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using GIS.District.Service;
using Microsoft.EntityFrameworkCore;

namespace GIS.Common.Repositories
{
    public static class Extensions
    {
        public static IServiceCollection AddPostgreSqlService<T>(this IServiceCollection services) where T : class, IEntity
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            IConfiguration configuration = serviceProvider.GetService<IConfiguration>(); 
            string connStr = configuration.GetSection(nameof(PostgreSqlSettings)).Get<PostgreSqlSettings>().ConnectionString;
            services.AddDbContext<BaseDbContext<T>>(options => options.UseNpgsql(connStr));

            services.AddSingleton<IRepository<T>>(serviceProvider =>
            {   
                return new PostgreRepository<T>();
            });

            return services;
        }
        public static IServiceCollection AddAuthenticationSettings(this IServiceCollection services)
        {
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>();
                jwt.RequireHttpsMetadata = false;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration?.GetSection("JWT:Issuer").Value,
                    ValidateAudience = true,
                    ValidAudience = configuration?.GetSection("JWT:Audience").Value,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration?.GetSection("JWT:SecretKey").Value)),
                    ValidateLifetime = true
                };

            });

            return services;
        }

        public static IServiceCollection AddRabbitMQService(this IServiceCollection services)
        {
            services.AddMassTransit(configure =>
            {
                configure.AddConsumers(Assembly.GetEntryAssembly());

                configure.UsingRabbitMq((context, configurator) =>
                {
                    IConfiguration configuration = context.GetService<IConfiguration>();
                    ServiceSettings serviceSettings = configuration.GetSection(nameof(serviceSettings)).Get<ServiceSettings>();
                    RabbitMQSettings rabbitMQSettings = configuration.GetSection(nameof(rabbitMQSettings)).Get<RabbitMQSettings>();
                    configurator.Host(rabbitMQSettings.Host);
                    configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                });
            });

            return services;
        }

        public static IServiceCollection AddMongoDatabase(this IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
            
            services.AddSingleton(serviceProvider =>
            {
                MongoDbSettings mongoDbSettings = serviceProvider.GetService<IConfiguration>().GetSection("MongoDbSettings").Get<MongoDbSettings>();
                MongoClient mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                ServiceSettings serviceSettings = serviceProvider.GetService<IConfiguration>().GetSection("ServiceSettings").Get<ServiceSettings>();
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });

            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T : IEntity
        {
            services.AddSingleton<IRepository<T>>(serviceProvider =>
            {
                IMongoDatabase mongoDatabase = serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepository<T>(mongoDatabase, collectionName);
            });

            return services;
        }
    }
}
