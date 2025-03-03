using System.ComponentModel.DataAnnotations;

namespace CarGleam.Models
{
    public class Machine
    {
        [Key]
        public int MachineId { get; set; }

        [Required]
        [MaxLength(100)]
        public string MachineName { get; set; }

        [Required]
        public string Status { get; set; } = "Available";  // Default status is 'Available'

        public ICollection<Booking> Bookings { get; set; }// reference navigation property to Booking
    }
}
