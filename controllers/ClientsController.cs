using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using server.Data;
using server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace server.Controllers
{
    // Define la ruta base para este controlador como "api/clients"
    [Route("api/clients")]
    [ApiController]
    [Authorize] // Requiere autenticación para acceder a los endpoints de este controlador
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClientsController> _logger;

        // Constructor que inyecta el contexto de la base de datos y el sistema de logs
        public ClientsController(AppDbContext context, ILogger<ClientsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Endpoint GET para obtener la lista de clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            _logger.LogInformation("Se ha llamado al endpoint GET /api/clients");

            // Obtiene todos los clientes de la base de datos de forma asíncrona
            var clients = await _context.Clients.ToListAsync();

            // Verifica si no hay clientes registrados
            if (clients == null || clients.Count == 0)
            {
                _logger.LogWarning("No se encontraron clientes en la base de datos.");
                return NotFound("No hay clientes registrados.");
            }

            return Ok(clients); // Retorna la lista de clientes con un estado 200 OK
        }

        // Endpoint POST para crear un nuevo cliente
        [HttpPost]
        public async Task<ActionResult<Client>> CreateClient([FromBody] Client client)
        {
            // Valida que el cliente no sea nulo y que tenga un correo válido
            if (client == null || string.IsNullOrWhiteSpace(client.Email))
            {
                _logger.LogWarning("Intento de crear un cliente con datos inválidos.");
                return BadRequest("El cliente debe tener un correo válido.");
            }

            _logger.LogInformation($"Creando cliente con ID: {client.Id}, Email: {client.Email}");

            // Agrega el nuevo cliente a la base de datos y guarda los cambios
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            // Retorna un estado 201 Created con el cliente creado
            return CreatedAtAction(nameof(GetClients), new { id = client.Id }, client);
        }
    }
}
