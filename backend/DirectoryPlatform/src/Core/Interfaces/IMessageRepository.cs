using DirectoryPlatform.Core.Entities;

namespace DirectoryPlatform.Core.Interfaces;

public interface IMessageRepository : IRepository<Message>
{
    Task<IEnumerable<Message>> GetInboxAsync(Guid userId);
    Task<IEnumerable<Message>> GetSentAsync(Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
}
