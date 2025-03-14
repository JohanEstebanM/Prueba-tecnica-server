using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; } 

        [Required]
        public int WorkshopId { get; set; } 

        [Required]
        public int TechnicianId { get; set; } 

        [Required]
        public DateTime Date { get; set; } 
    }
}
