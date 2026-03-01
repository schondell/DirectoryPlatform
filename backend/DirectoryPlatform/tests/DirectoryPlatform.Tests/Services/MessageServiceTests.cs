using AutoMapper;
using DirectoryPlatform.Application.Mapping;
using DirectoryPlatform.Application.Services;
using DirectoryPlatform.Contracts.DTOs.Message;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace DirectoryPlatform.Tests.Services;

public class MessageServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMessageRepository> _messageRepoMock;
    private readonly IMapper _mapper;
    private readonly MessageService _messageService;

    public MessageServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _messageRepoMock = new Mock<IMessageRepository>();
        _unitOfWorkMock.Setup(u => u.Messages).Returns(_messageRepoMock.Object);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _messageService = new MessageService(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task GetInboxAsync_ReturnsMessages()
    {
        var userId = Guid.NewGuid();
        var messages = new List<Message>
        {
            new() { Id = Guid.NewGuid(), SenderId = Guid.NewGuid(), RecipientId = userId, Subject = "Hi", Body = "Hello" }
        };
        _messageRepoMock.Setup(r => r.GetInboxAsync(userId)).ReturnsAsync(messages);

        var result = await _messageService.GetInboxAsync(userId);

        result.Should().HaveCount(1);
        result.First().Subject.Should().Be("Hi");
    }

    [Fact]
    public async Task GetSentAsync_ReturnsMessages()
    {
        var userId = Guid.NewGuid();
        var messages = new List<Message>
        {
            new() { Id = Guid.NewGuid(), SenderId = userId, RecipientId = Guid.NewGuid(), Subject = "Sent", Body = "Body" }
        };
        _messageRepoMock.Setup(r => r.GetSentAsync(userId)).ReturnsAsync(messages);

        var result = await _messageService.GetSentAsync(userId);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_AsRecipient_ReturnsMessage()
    {
        var userId = Guid.NewGuid();
        var msg = new Message
        {
            Id = Guid.NewGuid(), SenderId = Guid.NewGuid(), RecipientId = userId,
            Subject = "Test", Body = "Body"
        };
        _messageRepoMock.Setup(r => r.GetByIdAsync(msg.Id)).ReturnsAsync(msg);

        var result = await _messageService.GetByIdAsync(msg.Id, userId);

        result.Should().NotBeNull();
        result!.Subject.Should().Be("Test");
    }

    [Fact]
    public async Task GetByIdAsync_AsSender_ReturnsMessage()
    {
        var userId = Guid.NewGuid();
        var msg = new Message
        {
            Id = Guid.NewGuid(), SenderId = userId, RecipientId = Guid.NewGuid(),
            Subject = "Test", Body = "Body"
        };
        _messageRepoMock.Setup(r => r.GetByIdAsync(msg.Id)).ReturnsAsync(msg);

        var result = await _messageService.GetByIdAsync(msg.Id, userId);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_UnrelatedUser_ReturnsNull()
    {
        var msg = new Message
        {
            Id = Guid.NewGuid(), SenderId = Guid.NewGuid(), RecipientId = Guid.NewGuid(),
            Subject = "Test", Body = "Body"
        };
        _messageRepoMock.Setup(r => r.GetByIdAsync(msg.Id)).ReturnsAsync(msg);

        var result = await _messageService.GetByIdAsync(msg.Id, Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_NonExisting_ReturnsNull()
    {
        _messageRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Message?)null);

        var result = await _messageService.GetByIdAsync(Guid.NewGuid(), Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task SendAsync_CreatesMessage()
    {
        var senderId = Guid.NewGuid();
        var dto = new CreateMessageDto
        {
            RecipientId = Guid.NewGuid(),
            Subject = "Hello",
            Body = "How are you?"
        };

        _messageRepoMock.Setup(r => r.AddAsync(It.IsAny<Message>()))
            .ReturnsAsync((Message m) => m);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _messageService.SendAsync(dto, senderId);

        result.Should().NotBeNull();
        result.Subject.Should().Be("Hello");
        result.Body.Should().Be("How are you?");
        _messageRepoMock.Verify(r => r.AddAsync(It.Is<Message>(m =>
            m.SenderId == senderId && m.RecipientId == dto.RecipientId)), Times.Once);
    }

    [Fact]
    public async Task MarkAsReadAsync_RecipientMarksRead_SetsIsRead()
    {
        var userId = Guid.NewGuid();
        var msg = new Message
        {
            Id = Guid.NewGuid(), SenderId = Guid.NewGuid(), RecipientId = userId,
            Subject = "Test", Body = "Body", IsRead = false
        };
        _messageRepoMock.Setup(r => r.GetByIdAsync(msg.Id)).ReturnsAsync(msg);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _messageService.MarkAsReadAsync(msg.Id, userId);

        msg.IsRead.Should().BeTrue();
        msg.ReadAt.Should().NotBeNull();
    }

    [Fact]
    public async Task MarkAsReadAsync_NotRecipient_DoesNotMark()
    {
        var msg = new Message
        {
            Id = Guid.NewGuid(), SenderId = Guid.NewGuid(), RecipientId = Guid.NewGuid(),
            Subject = "Test", Body = "Body", IsRead = false
        };
        _messageRepoMock.Setup(r => r.GetByIdAsync(msg.Id)).ReturnsAsync(msg);

        await _messageService.MarkAsReadAsync(msg.Id, Guid.NewGuid());

        msg.IsRead.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_BySender_SetsDeletedBySender()
    {
        var senderId = Guid.NewGuid();
        var msg = new Message
        {
            Id = Guid.NewGuid(), SenderId = senderId, RecipientId = Guid.NewGuid(),
            Subject = "Test", Body = "Body"
        };
        _messageRepoMock.Setup(r => r.GetByIdAsync(msg.Id)).ReturnsAsync(msg);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _messageService.DeleteAsync(msg.Id, senderId);

        msg.IsDeletedBySender.Should().BeTrue();
        msg.IsDeletedByRecipient.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ByRecipient_SetsDeletedByRecipient()
    {
        var recipientId = Guid.NewGuid();
        var msg = new Message
        {
            Id = Guid.NewGuid(), SenderId = Guid.NewGuid(), RecipientId = recipientId,
            Subject = "Test", Body = "Body"
        };
        _messageRepoMock.Setup(r => r.GetByIdAsync(msg.Id)).ReturnsAsync(msg);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _messageService.DeleteAsync(msg.Id, recipientId);

        msg.IsDeletedByRecipient.Should().BeTrue();
        msg.IsDeletedBySender.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_NonExisting_ThrowsKeyNotFoundException()
    {
        _messageRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Message?)null);

        var act = () => _messageService.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetUnreadCountAsync_ReturnsCount()
    {
        var userId = Guid.NewGuid();
        _messageRepoMock.Setup(r => r.GetUnreadCountAsync(userId)).ReturnsAsync(5);

        var result = await _messageService.GetUnreadCountAsync(userId);

        result.Should().Be(5);
    }
}
