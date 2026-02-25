using AutoMapper;
using DirectoryPlatform.Contracts.DTOs.Notification;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;

namespace DirectoryPlatform.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(Guid userId)
    {
        var notifications = await _unitOfWork.Notifications.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
        => await _unitOfWork.Notifications.GetUnreadCountAsync(userId);

    public async Task MarkAsReadAsync(Guid id, Guid userId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
        if (notification != null && notification.UserId == userId)
        {
            notification.IsRead = true;
            await _unitOfWork.Notifications.UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId)
        => await _unitOfWork.Notifications.MarkAllAsReadAsync(userId);

    public async Task CreateAsync(Guid userId, string type, string title, string message, string? actionUrl = null)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(), UserId = userId,
            Type = Enum.Parse<NotificationType>(type),
            Title = title, Message = message, ActionUrl = actionUrl
        };
        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();
    }
}
