using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IListingService, ListingService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IRegionService, RegionService>();
        services.AddScoped<IAttributeDefinitionService, AttributeDefinitionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IListingEngagementService, ListingEngagementService>();
        services.AddScoped<IBoostService, BoostService>();
        services.AddScoped<IBookkeepingService, BookkeepingService>();
        services.AddScoped<IAdminDashboardService, AdminDashboardService>();
        services.AddScoped<IUserKpiService, UserKpiService>();
        services.AddScoped<IScraperAnalyticsService, ScraperAnalyticsService>();
        services.AddScoped<IAiListingService, AiListingService>();

        return services;
    }
}
