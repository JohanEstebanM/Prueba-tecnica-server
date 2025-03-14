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
    [Route("api/clients")]
    [ApiController]
    [Authorize] 
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(AppDbContext context, ILogger<ClientsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            _logger.LogInformation("Se ha llamado al endpoint GET /api/clients");

            var clients = await _context.Clients.ToListAsync();

            if (clients == null || clients.Count == 0)
            {
                _logger.LogWarning("No se encontraron clientes en la base de datos.");
                return NotFound("No hay clientes registrados.");
            }

            return Ok(clients);
        }

        [HttpPost]
        public async Task<ActionResult<Client>> CreateClient([FromBody] Client client)
        {
            if (client == null || string.IsNullOrWhiteSpace(client.Email))
            {
                _logger.LogWarning("Intento de crear un cliente con datos inválidos.");
                return BadRequest("El cliente debe tener un correo válido.");
            }

            _logger.LogInformation($"Creando cliente con ID: {client.Id}, Email: {client.Email}");

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClients), new { id = client.Id }, client);
        }
    }
}
