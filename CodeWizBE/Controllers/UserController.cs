using CodeWizBE.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        [HttpGet("/api/profile/getUser")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.
                Include(u => u.Chats)
                .ToListAsync();
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
                        string jwtSecret = GenerateJwtToken(existingUser);
                        return Ok(jwtSecret);
                        
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
        private string GenerateJwtToken(User user)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string jwtSecretKey = config.GetSection("SecretKey")["jwtSecret"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("userId", user.UserId.ToString()),
                    new Claim("username", user.UserName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        [HttpGet("user/details")]
        public IActionResult GetUserDetails()
        {
            string userId = HttpContext.Items["UserId"]?.ToString();
            string username = HttpContext.Items["Username"]?.ToString();

            return Ok(userId);
        }
    }

}
