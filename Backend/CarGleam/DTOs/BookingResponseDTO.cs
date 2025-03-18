namespace CarGleam.DTOs
{
    public class BookingResponseDTO
    {
        public int UserId { get; set; }
        public int BookingId { get; set; }
        public string FullName { get; set; }
        public string ServiceName { get; set; }
        public string LocationName { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; }
        public DateTime ServiceDate { get; set; }

    }
}
