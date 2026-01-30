using Amazon.S3;
using MassTransit; 
using Media.Application.Abstractions;
using Media.Application.Data;
using Media.Infrastructure.Data;
using Media.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Media.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        var storageConfig = configuration.GetSection("Storage");
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var config = new AmazonS3Config
            {
                ServiceURL = storageConfig["ServiceUrl"],
                ForcePathStyle = true,
                AuthenticationRegion = "auto"
            };
            return new AmazonS3Client(storageConfig["AccessKey"], storageConfig["SecretKey"], config);
        });

        services.AddScoped<IStorageService, S3StorageService>();
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<MediaDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        
        services.AddScoped<IApplicationDbontext, MediaDbContext>();


        services.AddMassTransit(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();

            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["MessageBroker:Host"], "/", h =>
                {
                    h.Username(configuration["MessageBroker:UserName"]);
                    h.Password(configuration["MessageBroker:Password"]);
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}