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
    [Authorize] 
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var appointments = await _context.Appointments.ToListAsync();
            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment newAppointment)
        {
            if (newAppointment == null)
                return BadRequest("Datos inválidos");

            newAppointment.Date = DateTime.SpecifyKind(newAppointment.Date, DateTimeKind.Utc);

            bool technicianHasAppointment = await _context.Appointments
                .AnyAsync(a => a.TechnicianId == newAppointment.TechnicianId && a.Date == newAppointment.Date);

            if (technicianHasAppointment)
            {
                return BadRequest("Este técnico ya tiene una cita a esta hora.");
            }

            int workshopAppointments = await _context.Appointments
                .CountAsync(a => a.WorkshopId == newAppointment.WorkshopId && a.Date == newAppointment.Date);

            if (workshopAppointments >= 3)
            {
                return BadRequest("Este taller ya tiene 3 citas programadas en esta hora.");
            }

            _context.Appointments.Add(newAppointment);
            await _context.SaveChangesAsync();

            return Ok(newAppointment);
        }
    }
}
