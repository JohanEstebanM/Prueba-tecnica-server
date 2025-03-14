using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Data
{
    // Clase que representa el contexto de la base de datos en Entity Framework Core
    public class AppDbContext : DbContext
    {
        // Constructor que recibe opciones de configuración y las pasa a la clase base
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Definición de los conjuntos de datos (tablas) en la base de datos
        public DbSet<User> Users { get; set; } // Tabla de usuarios
        public DbSet<Client> Clients { get; set; } // Tabla de clientes
        public DbSet<Technician> Technicians { get; set; } // Tabla de técnicos
        public DbSet<Workshop> Workshops { get; set; } // Tabla de talleres
        public DbSet<Appointment> Appointments { get; set; } // Tabla de citas
    }
}
