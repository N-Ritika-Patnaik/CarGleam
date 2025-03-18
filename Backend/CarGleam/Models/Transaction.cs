using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarGleam.Models
{
    public class Transaction : IValidatableObject  //--- inheritance or implementation of IValidatableObject in transaction
    {                                              // Interface defines a method named Validate, lets object perform custom validation logic.
                                                   // When implement this interface in a class, can add custom validation logic to that class.
        [Key]
        public int TransactionId { get; set; } // primary key

        [Required]
        public int BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; } // navigate to booking

        [Required]
        [MaxLength(10)]
        public string PaymentMethod { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, double.MaxValue)]
        public decimal PaymentAmount { get; set; }

        [MaxLength(16)]
        public string? CardNumber { get; set; } // ? = nullable
        public string? CardExpiry { get; set; }

        [MaxLength(16)]
        public string? UpiId { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string PaymentStatus { get; set; } = "Done";  

        // Validation logic  ensure only one payment method is accepted
        // TO ITERATE throught a COLLECTION we use Ienumerable
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // PaymentMethod must be one of "Cash", "Card", or "Upi"
            //if (PaymentMethod != "Cash" && PaymentMethod != "Card" && PaymentMethod != "Upi")
            if (PaymentMethod.ToLower() != "cash" && PaymentMethod.ToLower() != "card" && PaymentMethod.ToLower() != "upi")

            {
                yield return new ValidationResult( // yield return is used to return each element one at a time
                    "PaymentMethod must be 'Cash', 'Card', or 'Upi'.",
                    new[] { "PaymentMethod" }); // new[] { "PaymentMethod" } is an array of strings
            }

            if (PaymentMethod.ToLower() == "cash")
            {
                if (!string.IsNullOrEmpty(CardNumber) || !string.IsNullOrEmpty(CardExpiry))
                {
                    yield return new ValidationResult(
                        "Card Number and Card Expiry are null when Payment Method is Cash.",
                        new[] { "CardNumber", "CardExpiry" });
                }
                if (!string.IsNullOrEmpty(UpiId))
                {
                    yield return new ValidationResult(
                        "Upi Id is null when Payment Method is Cash.",
                        new[] { "UpiId" });
                }
            }
            //else if (PaymentMethod == "Card")
            else if (PaymentMethod.ToLower() == "card")
            {
                if (string.IsNullOrEmpty(CardNumber) || string.IsNullOrEmpty(CardExpiry))
                {
                    yield return new ValidationResult(
                        "Card Number and Card Expiry are required when Payment Method is Card.",
                        new[] { "CardNumber", "CardExpiry" });
                }
                if (!string.IsNullOrEmpty(UpiId))
                {
                    yield return new ValidationResult(
                        "Upi Id is null when PaymentMethod is Card.",
                        new[] { "UpiId" });
                }
            }

            else if (PaymentMethod.ToLower() == "upi")
            {
                if (string.IsNullOrEmpty(UpiId))
                {
                    yield return new ValidationResult(
                        "Upi Id is required when Payment Method is Upi.",
                        new[] { "UpiId" });
                }
                if (!string.IsNullOrEmpty(CardNumber) || !string.IsNullOrEmpty(CardExpiry))
                {
                    yield return new ValidationResult(
                        "Card Number and Card Expiry are null when Payment Method is Upi.",
                        new[] { "CardNumber", "CardExpiry" });
                }
            }
        }
    }
}