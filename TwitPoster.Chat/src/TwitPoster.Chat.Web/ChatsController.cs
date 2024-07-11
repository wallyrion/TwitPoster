using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitPoster.Chat.Application;
using TwitPoster.Chat.Domain;

namespace TwitPoster.Chat;

public record CreateRoomChatRequest(List<Guid> Participants, string? Name = null);

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ChatsController(IChatsRepository chatsRepository, IMessagesRepository messagesRepository) : ControllerBase
{
    [HttpGet]
    public async Task<List<RoomChat>> Get() =>
        await chatsRepository.GetAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomChat>> Get(string id)
    {
        var message = await chatsRepository.GetAsync(id);

        if (message is null)
        {
            return NotFound();
        }

        return message;
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateRoomChatRequest roomChatRequest)
    {
        var chat = new RoomChat
        {
            Participants = roomChatRequest.Participants,
            CreatedAt = DateTime.UtcNow,
            Name = roomChatRequest.Name
        };
        await chatsRepository.CreateAsync(chat);

        return CreatedAtAction(nameof(Get), new { id = chat.Id }, chat);
    }
    
        
    [HttpGet("{chatId}/messages")]
    public async Task<ActionResult<List<Message>>> GetByChatId(string chatId)
    {
        var message = await messagesRepository.GetByChatIdAsync(chatId);

        return message;
    }

    /*[HttpPut("{id:guid}")]
public async Task<IActionResult> Update(string id, Message updatedMessage)
{
    var book = await _messagesService.GetAsync(id);

    if (book is null)
    {
        return NotFound();
    }

    updatedMessage.Id = book.Id;

    await _messagesService.UpdateAsync(id, updatedMessage);

    return NoContent();
}*/
}