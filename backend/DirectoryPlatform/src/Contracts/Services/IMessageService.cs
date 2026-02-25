using DirectoryPlatform.Contracts.DTOs.Message;

namespace DirectoryPlatform.Contracts.Services;

public interface IMessageService
{
    Task<IEnumerable<MessageDto>> GetInboxAsync(Guid userId);
    Task<IEnumerable<MessageDto>> GetSentAsync(Guid userId);
    Task<MessageDto?> GetByIdAsync(Guid id, Guid userId);
    Task<MessageDto> SendAsync(CreateMessageDto dto, Guid senderId);
    Task MarkAsReadAsync(Guid id, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
}
