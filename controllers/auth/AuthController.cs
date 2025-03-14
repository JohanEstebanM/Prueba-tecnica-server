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
    private readonly AppDbContext _context; // Contexto de base de datos para interactuar con la tabla de usuarios

    public AuthController(AppDbContext context)
    {
        _context = context; // Inyecta el contexto de la base de datos en el controlador
    }

    [HttpPost("signup")] // Ruta HTTP POST para el registro de usuarios
    public async Task<IActionResult> Register([FromBody] User user) // Recibe un usuario en el cuerpo de la solicitud
    {
        // Verifica si el usuario ya existe en la base de datos
        if (await _context.Users.AnyAsync(u => u.Username == user.Username))
        {
            return BadRequest(new { message = "El usuario ya existe" }); // Retorna error si el usuario ya está registrado
        }

        // Hashea la contraseña antes de guardarla en la base de datos
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        // Agrega el usuario a la base de datos
        _context.Users.Add(user);
        await _context.SaveChangesAsync(); // Guarda los cambios de forma asíncrona

        return Ok(new { message = "Usuario registrado con éxito" }); // Retorna mensaje de éxito
    }

    [HttpPost("login")] // Ruta HTTP POST para iniciar sesión
    public async Task<IActionResult> Login([FromBody] LoginModel login) // Recibe credenciales en el cuerpo de la solicitud
    {
        // Busca al usuario en la base de datos por su nombre de usuario
        var user = await _context.Users.FirstOrDefaultAsync(c => c.Username == login.Username);
        
        // Si el usuario no existe o la contraseña es incorrecta, retorna error de autenticación
        if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password)) 
        {
            return Unauthorized(new { message = "Credenciales incorrectas" }); // Retorna error 401
        }

        // Genera un token JWT para el usuario autenticado
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("Ib`|d)eH>G2{H|1H#4Os7kz[cvI:B+o:"); // Clave secreta para firmar el token

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username), // Agrega el nombre del usuario como reclamo en el token
                new Claim("UserId", user.Id.ToString()) // Agrega el ID del usuario como reclamo en el token
            }),
            Expires = DateTime.UtcNow.AddHours(1), // Establece la expiración del token a 1 hora
            Issuer = "http://localhost:5214", // Define el emisor del token
            Audience = "http://localhost:5214", // Define la audiencia del token
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // Firma el token con HMAC SHA256
        };

        var token = tokenHandler.CreateToken(tokenDescriptor); // Crea el token con la configuración dada
        var tokenString = tokenHandler.WriteToken(token); // Convierte el token a una cadena de texto

        return Ok(new { token = tokenString }); // Retorna el token generado
    }
}

// Modelo para recibir credenciales de inicio de sesión
public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
