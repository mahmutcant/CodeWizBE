using CodeWizBE.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeWizBE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("/getUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] UserDTO user)
        {
            if(user != null)
            {
                try
                {
                    var newUser = new User()
                    {
                        UserName = user.UserName,
                        Password = user.Password,
                        UserEmail = user.UserEmail
                    };
                    await _context.Users.AddAsync(newUser);
                    await _context.SaveChangesAsync();
                    return Ok();
                }catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();

        }
        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO user)
        {
            if(user != null)
            {
                try
                {
                    var existingUser = await _context.Users
                .Where(u => u.UserName == user.UserName && u.Password == user.Password)
                .FirstOrDefaultAsync();
                    if (existingUser != null)
                    {
                        return Ok("Giriş Başarılı");
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            return BadRequest();
        }
    }
}
