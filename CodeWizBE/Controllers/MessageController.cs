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
            var messages = chat.Messages;
            return Ok(messages);
        }
    }
}
