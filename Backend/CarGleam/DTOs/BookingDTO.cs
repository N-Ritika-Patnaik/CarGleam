namespace CarGleam.DTOs
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public int ServiceLocationId { get; set; }
        public int MachineId { get; set; }
        public DateTime ServiceDate { get; set; }
    }
}
