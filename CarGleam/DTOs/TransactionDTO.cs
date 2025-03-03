namespace CarGleam.DTOs
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }
        public int BookingId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string? CardNumber { get; set; }
        public string? CardExpiry { get; set; }
        public string? UpiId { get; set; }
        public string PaymentStatus { get; set; }

    }
}