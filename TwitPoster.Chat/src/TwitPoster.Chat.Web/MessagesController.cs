using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitPoster.Chat.Application;
using TwitPoster.Chat.Domain;

namespace TwitPoster.Chat;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MessagesController(IMessagesRepository messagesRepository, ICurrentUser currentUser) : ControllerBase
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
    public async Task<IActionResult> Post(string message)
    {
        var newMessage = new Message(message, currentUser.Id);
        await messagesRepository.CreateAsync(newMessage);

        return CreatedAtAction(nameof(Get), new { id = newMessage.Id }, newMessage);
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

        await messagesRepository.RemoveAsync(id);

        return NoContent();
    }
}