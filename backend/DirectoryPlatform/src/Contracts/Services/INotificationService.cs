using DirectoryPlatform.Contracts.DTOs.Notification;

namespace DirectoryPlatform.Contracts.Services;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetByUserIdAsync(Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkAsReadAsync(Guid id, Guid userId);
    Task MarkAllAsReadAsync(Guid userId);
    Task CreateAsync(Guid userId, string type, string title, string message, string? actionUrl = null);
}
