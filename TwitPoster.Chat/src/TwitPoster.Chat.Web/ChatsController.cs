using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitPoster.Chat.Application.Chats.Commands;
using TwitPoster.Chat.Application.Chats.Queries.GetChats;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Application.Messages.Queries.GetMessagesByChatId;
using TwitPoster.Chat.Domain;
using TwitPoster.Chat.Domain.ChatAggregateRoot;
using TwitPoster.Chat.Requests;

namespace TwitPoster.Chat;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ChatsController(IChatsRepository chatsRepository, ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<List<RoomChat>> Get()
    {
        var request = new GetChatsQuery();
        var chats = await sender.Send(request);

        return chats;
    }

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
        var createRoomCharCommand = new CreateChatCommand(roomChatRequest.ParticipantsIds, roomChatRequest.Name);
        var result = await sender.Send(createRoomCharCommand);
        
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }
    
        
    [HttpGet("{chatId}/messages")]
    public async Task<ActionResult<List<Message>>> GetByChatId(string chatId)
    {
        var request = new GetMessagesByChatIdQuery(chatId);
        var messages = await sender.Send(request);

        return Ok(messages);
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