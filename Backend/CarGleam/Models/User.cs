using System.ComponentModel.DataAnnotations;

namespace CarGleam.Models
{
    public class User: Base //  inheritance from Base.cs class
    {
        [Key] // these data anootations for validation
        public int UserId { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } // scalar property

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } = "User";  // Default role is 'User'

        public DateTime LastLogin { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;  // Default is active

        public ICollection<Booking> Bookings { get; set; } // navigation property to Booking

        // Icollection is a GENERIC INTERFACE  represents COLLECTION OF OBJECTS  that can be individually accessed by index
        // INTERFACE [abstract class with empty methods]         
    }
}