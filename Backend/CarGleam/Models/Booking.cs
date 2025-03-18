using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CarGleam.Models
{
    public class Booking // main entity
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]

        [Required]
        public string FullName { get; set; }
        [ForeignKey("FullName")]
        public User User { get; set; } //navigation property to User // related entity

        [Required]
        public int ServiceLocationId { get; set; }
        [ForeignKey("ServiceLocationId")]
        public ServiceLocation ServiceLocation { get; set; } //reference navigation property to ServiceLocation // related entity

        [Required]
        public int MachineId { get; set; }

        [ForeignKey("MachineId")]

        public Machine Machine { get; set; } // related entity

        [Required]
        public DateTime ServiceDate { get; set; } = DateTime.Now;  // Default to current time
        public ICollection<Transaction> Transactions { get; set; } // collection navigation property to Transaction //related entity
    }
}



//public DateTime ServiceDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
