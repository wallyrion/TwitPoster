using Microsoft.AspNetCore.Mvc;
using TwitPoster.BLL.Extensions;
using TwitPoster.Chat;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly MessagesService _messagesService;
    private readonly ICurrentUser _currentUser;

    public MessagesController(MessagesService messagesService, ICurrentUser currentUser)
    {
        _messagesService = messagesService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<List<Message>> Get() =>
        await _messagesService.GetAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> Get(string id)
    {
        var message = await _messagesService.GetAsync(id);

        if (message is null)
        {
            return NotFound();
        }

        return message;
    }

    [HttpPost]
    public async Task<IActionResult> Post(string message)
    {
        var newMessage = new Message(message, _currentUser.Id);
        await _messagesService.CreateAsync(newMessage);

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
        var book = await _messagesService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        await _messagesService.RemoveAsync(id);

        return NoContent();
    }
}