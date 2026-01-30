using Amazon.S3;
using Marten;
using MassTransit; // <--- Thêm namespace này
using Media.Application.Abstractions;
using Media.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Media.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Cấu hình Storage (S3/R2)
        var storageConfig = configuration.GetSection("Storage");
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var config = new AmazonS3Config
            {
                ServiceURL = storageConfig["ServiceUrl"],
                // Với R2/S3 thật thì ForcePathStyle thường là false (hoặc bỏ qua). 
                // Với MinIO thì bắt buộc true.
                ForcePathStyle = true 
            };
            return new AmazonS3Client(storageConfig["AccessKey"], storageConfig["SecretKey"], config);
        });

        services.AddScoped<IStorageService, S3StorageService>();

        // 2. Cấu hình Database (Marten)
        services.AddMarten(opts =>
        {
            opts.Connection(configuration.GetConnectionString("Database")!);
        }).UseLightweightSessions();

        // 3. Cấu hình Message Broker (MassTransit/RabbitMQ) ---> THÊM ĐOẠN NÀY
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