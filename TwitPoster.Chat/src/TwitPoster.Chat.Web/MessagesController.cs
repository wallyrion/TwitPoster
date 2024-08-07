using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitPoster.Chat.Application.Common.Interfaces;
using TwitPoster.Chat.Application.Messages.Commands;
using TwitPoster.Chat.Domain;
using TwitPoster.Chat.Domain.MessageAggregateRoot;

namespace TwitPoster.Chat;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MessagesController(IMessagesRepository messagesRepository, ICurrentUser currentUser, ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<List<Message>> Get() =>
        await messagesRepository.GetAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> Get(string id)
    {
        var message = await messagesRepository.GetAsync(id);

        if (message is null)
        {
            return NotFound();
        }

        return message;
    }


    [HttpPost]
    public async Task<IActionResult> Post(string text, string chatId)
    {
        var newMessage = new AddMessageToChatCommand(chatId, text, currentUser.Id);
        var (message, _) = await sender.Send(newMessage);

        return CreatedAtAction(nameof(Get), new { id = message.Id }, newMessage);
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var book = await messagesRepository.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        //await messagesRepository.RemoveAsync(id);

        return NoContent();
    }
}