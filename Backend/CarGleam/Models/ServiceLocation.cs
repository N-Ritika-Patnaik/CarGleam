using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarGleam.Models
{
    public class ServiceLocation
    {
        [Key]
        public int ServiceLocationId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ServiceName { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(100)]
        public string LocationName { get; set; }
        [Required]
        [MaxLength(50)]
        public string? ServiceType { get; set; }

        public ICollection<Booking> Bookings { get; set; } // navigation property to Booking
    }
}
