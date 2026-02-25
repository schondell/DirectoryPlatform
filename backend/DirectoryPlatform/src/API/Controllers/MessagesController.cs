using DirectoryPlatform.Contracts.DTOs.Message;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers;

[Authorize]
public class MessagesController : BaseController
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet("inbox")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetInbox()
    {
        var messages = await _messageService.GetInboxAsync(GetCurrentUserId());
        return Ok(messages);
    }

    [HttpGet("sent")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetSent()
    {
        var messages = await _messageService.GetSentAsync(GetCurrentUserId());
        return Ok(messages);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MessageDto>> GetById(Guid id)
    {
        var message = await _messageService.GetByIdAsync(id, GetCurrentUserId());
        return message == null ? NotFound() : Ok(message);
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var count = await _messageService.GetUnreadCountAsync(GetCurrentUserId());
        return Ok(count);
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> Send([FromBody] CreateMessageDto dto)
    {
        var message = await _messageService.SendAsync(dto, GetCurrentUserId());
        return Ok(message);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        await _messageService.MarkAsReadAsync(id, GetCurrentUserId());
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _messageService.DeleteAsync(id, GetCurrentUserId());
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}
