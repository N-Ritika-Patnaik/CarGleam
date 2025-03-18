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

        [Required]
        [MaxLength(50)]
        public string MachineType { get; set; }

        [Required]
        public TimeSpan Duration { get; set; } // Duration of the service

        public bool IsAvailable(DateTime serviceDate, TimeSpan duration)
        {
            // Check if there are any bookings that overlap with the requested service date and duration
            return !Bookings.Any(b => b.ServiceDate < serviceDate.Add(duration) && b.ServiceDate.Add(Duration) > serviceDate);
        }

        public ICollection<Booking> Bookings { get; set; }// reference navigation property to Booking
    }
}
