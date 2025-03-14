using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using server.Models;
using server.Data;
using System.Threading.Tasks;
using BCrypt.Net;

[Route("auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (await _context.Users.AnyAsync(u => u.Username == user.Username))
        {
            return BadRequest(new { message = "El usuario ya existe" });
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Usuario registrado con éxito" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel login)
    {
        var user = await _context.Users.FirstOrDefaultAsync(c => c.Username == login.Username);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password)) 
        {
            return Unauthorized(new { message = "Credenciales incorrectas" });
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("Ib`|d)eH>G2{H|1H#4Os7kz[cvI:B+o:"); 

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "http://localhost:5214",
            Audience = "http://localhost:5214",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { token = tokenString });
    }
}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
