using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using server.Models;
using server.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios de controladores al contenedor de dependencias
builder.Services.AddControllers();

// Clave secreta para la firma del token JWT
var key = "Ib`|d)eH>G2{H|1H#4Os7kz[cvI:B+o:";

// Configuración de autenticación con JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Validar el emisor del token
        ValidateAudience = true, // Validar la audiencia del token
        ValidateLifetime = true, // Validar el tiempo de vida del token
        ValidateIssuerSigningKey = true, // Validar la clave de firma
        ValidIssuer = "http://localhost:5214", // URL del emisor válido
        ValidAudience = "http://localhost:5214", // URL de la audiencia válida
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)) // Clave de firma del token
    };
});

// Agregar soporte para documentación de la API con Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de la base de datos PostgreSQL con Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); 

// Definición de una política CORS para permitir solicitudes desde el frontend en Angular
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200") // Permitir solo solicitudes desde Angular
                                .AllowAnyHeader() // Permitir cualquier encabezado
                                .AllowAnyMethod(); // Permitir cualquier método (GET, POST, PUT, DELETE, etc.)
                      });
});

var app = builder.Build();

// Habilitar la política CORS definida anteriormente
app.UseCors(MyAllowSpecificOrigins);

if (app.Environment.IsDevelopment())
{
    // Habilitar Swagger en entorno de desarrollo para probar la API
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirigir automáticamente todas las solicitudes HTTP a HTTPS
app.UseHttpsRedirection();

// Habilitar la autenticación y autorización en la aplicación
app.UseAuthentication();
app.UseAuthorization();

// Mapear los controladores para manejar las solicitudes HTTP
app.MapControllers();

// Iniciar la aplicación
app.Run();
