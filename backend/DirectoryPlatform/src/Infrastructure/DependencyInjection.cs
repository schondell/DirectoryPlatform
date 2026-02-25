using Amazon.S3;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using DirectoryPlatform.Infrastructure.Repositories;
using DirectoryPlatform.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .UseSnakeCaseNamingConvention());

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IRegionRepository, RegionRepository>();
        services.AddScoped<IListingRepository, ListingRepository>();
        services.AddScoped<IAttributeDefinitionRepository, AttributeDefinitionRepository>();
        services.AddScoped<IListingAttributeRepository, ListingAttributeRepository>();
        services.AddScoped<IListingDetailRepository, ListingDetailRepository>();
        services.AddScoped<IListingMediaRepository, ListingMediaRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // S3/MinIO
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var config = new AmazonS3Config
            {
                ServiceURL = configuration["AWSSettings:ServiceURL"] ?? "http://localhost:9000",
                ForcePathStyle = true
            };
            return new AmazonS3Client(
                configuration["AWSSettings:AccessKey"] ?? "minioadmin",
                configuration["AWSSettings:SecretKey"] ?? "minioadmin",
                config);
        });
        services.AddScoped<IMediaService, S3MediaService>();

        // Email
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
