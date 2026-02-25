using AutoMapper;
using DirectoryPlatform.Contracts.DTOs.Message;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;

namespace DirectoryPlatform.Application.Services;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MessageService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MessageDto>> GetInboxAsync(Guid userId)
    {
        var messages = await _unitOfWork.Messages.GetInboxAsync(userId);
        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<IEnumerable<MessageDto>> GetSentAsync(Guid userId)
    {
        var messages = await _unitOfWork.Messages.GetSentAsync(userId);
        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<MessageDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var msg = await _unitOfWork.Messages.GetByIdAsync(id);
        if (msg == null || (msg.SenderId != userId && msg.RecipientId != userId))
            return null;
        return _mapper.Map<MessageDto>(msg);
    }

    public async Task<MessageDto> SendAsync(CreateMessageDto dto, Guid senderId)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(), SenderId = senderId, RecipientId = dto.RecipientId,
            ListingId = dto.ListingId, Subject = dto.Subject, Body = dto.Body
        };
        await _unitOfWork.Messages.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<MessageDto>(message);
    }

    public async Task MarkAsReadAsync(Guid id, Guid userId)
    {
        var msg = await _unitOfWork.Messages.GetByIdAsync(id);
        if (msg != null && msg.RecipientId == userId)
        {
            msg.IsRead = true;
            msg.ReadAt = DateTime.UtcNow;
            await _unitOfWork.Messages.UpdateAsync(msg);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var msg = await _unitOfWork.Messages.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Message not found");
        if (msg.SenderId == userId) msg.IsDeletedBySender = true;
        if (msg.RecipientId == userId) msg.IsDeletedByRecipient = true;
        await _unitOfWork.Messages.UpdateAsync(msg);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
        => await _unitOfWork.Messages.GetUnreadCountAsync(userId);
}
