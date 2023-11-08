using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        
        if (context.Request.Path.StartsWithSegments("/register") || context.Request.Path.StartsWithSegments("/login"))
        {
            await _next(context);
            return;
        }

        
        string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (token == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

        string jwtSecretKey = config.GetSection("SecretKey")["jwtSecret"];
        var key = Encoding.ASCII.GetBytes(jwtSecretKey);
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        string userId = jwtToken.Claims.First(x => x.Type == "userId").Value;
        string username = jwtToken.Claims.First(x => x.Type == "username").Value;

        context.Items["UserId"] = userId;
        context.Items["Username"] = username;

        await _next(context);
    }
}