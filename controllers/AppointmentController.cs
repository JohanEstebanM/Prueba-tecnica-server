using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Controllers
{
    [Authorize] // Requiere autenticación para acceder a los endpoints de este controlador
    [ApiController]
    [Route("api/appointments")] // Define la ruta base del controlador como "api/appointments"
    public class AppointmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Constructor que inyecta el contexto de la base de datos
        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint GET para obtener la lista de citas
        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var appointments = await _context.Appointments.ToListAsync(); // Obtiene todas las citas
            return Ok(appointments); // Retorna la lista con estado 200 OK
        }

        // Endpoint POST para crear una nueva cita
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment newAppointment)
        {
            // Verifica que la cita no sea nula
            if (newAppointment == null)
                return BadRequest("Datos inválidos");

            // Asegura que la fecha de la cita se almacene en formato UTC
            newAppointment.Date = DateTime.SpecifyKind(newAppointment.Date, DateTimeKind.Utc);

            // Verifica si el técnico ya tiene una cita en la misma fecha y hora
            bool technicianHasAppointment = await _context.Appointments
                .AnyAsync(a => a.TechnicianId == newAppointment.TechnicianId && a.Date == newAppointment.Date);

            if (technicianHasAppointment)
            {
                return BadRequest("Este técnico ya tiene una cita a esta hora.");
            }

            // Verifica si el taller ya tiene el número máximo de citas permitidas (3) en la misma fecha y hora
            int workshopAppointments = await _context.Appointments
                .CountAsync(a => a.WorkshopId == newAppointment.WorkshopId && a.Date == newAppointment.Date);

            if (workshopAppointments >= 3)
            {
                return BadRequest("Este taller ya tiene 3 citas programadas en esta hora.");
            }

            // Agrega la nueva cita a la base de datos y guarda los cambios
            _context.Appointments.Add(newAppointment);
            await _context.SaveChangesAsync();

            return Ok(newAppointment); // Retorna la cita creada con estado 200 OK
        }
    }
}
