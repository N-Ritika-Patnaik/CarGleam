namespace CarGleam.Models
{
    public class Base
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } 
    }
}
