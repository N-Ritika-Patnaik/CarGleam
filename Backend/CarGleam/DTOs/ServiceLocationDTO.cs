namespace CarGleam.DTOs
{
    public class ServiceLocationDTO
    {
        public int ServiceLocationId { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public string LocationName { get; set; }
        public string? ServiceType { get; set; }
    }
}