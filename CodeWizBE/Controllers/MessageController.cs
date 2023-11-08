using CodeWizBE.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeWizBE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MessageController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("/api/createchat")]
        public async Task<IActionResult> CreateChat([FromBody] ChatDTO chatName)
        {
            string userId = HttpContext.Items["UserId"]?.ToString();
            var existingUser = await _context.Users
                .Where(u => u.UserId == Convert.ToInt16(userId))
                .FirstOrDefaultAsync();
            if (userId != null)
            {
                try
                {
                    var newChat = new Chat()
                    {
                        ChatName = chatName.ChatName,
                        User = existingUser
                    };
                    await _context.Chats.AddAsync(newChat);
                    await _context.SaveChangesAsync();
                    return Ok();
                }catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return Ok(existingUser);
        }
        [HttpGet("/api/getmychats")]
        public async Task<IActionResult> getMyChats()
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            var chatList = await _context.Chats
                .Where(u => u.User.UserId == userId).ToListAsync();
            return Ok(chatList);
        }
        [HttpDelete("api/deletechat")]
        public async Task<IActionResult> deleteChat(int chatId)
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            var chat = await _context.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.User.UserId == userId && c.ChatId == chatId);
            if (chat == null)
            {
                return NotFound("Chat not found");
            }
            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("/api/getmessages")]
        public async Task<IActionResult> getMessagesByChatId(int chatId)
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            var chat = await _context.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.User.UserId == userId && c.ChatId == chatId);
            if (chat == null)
            {
                return NotFound("Chat not found");
            }
            var messages = chat.Messages.Select(m => new
            {
                m.MessageId,
                m.MessageContent,
                m.UserMessage
            });

            return Ok(messages);
        }
        [HttpPost("/api/addmessage")]
        public async Task<IActionResult> AddMessagesByChatId(int chatId, [FromBody] MessageDTO message)
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            var chat = await _context.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.User.UserId == userId && c.ChatId == chatId);
            if (chat == null)
            {
                return NotFound("Chat not found");
            }
            var newMessage = new Message
            {
                MessageContent = message.MessageContent,
                UserMessage = message.UserMessage,
                Chat = chat
            };
            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
