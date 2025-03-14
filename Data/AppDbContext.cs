using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Technician> Technicians { get; set; }
        public DbSet<Workshop> Workshops { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}
