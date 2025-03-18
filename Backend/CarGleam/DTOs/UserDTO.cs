using System.ComponentModel.DataAnnotations;

namespace CarGleam.DTOs
{
    // dto isn't stored in database but used to transfer data between client and server - so no need for login ans sigup , make dto
    public class UserDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; }

        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[!@#$%^&*]).+$", ErrorMessage = "Password must contain at least one number and one special character.")]
        public string Password { get; set; }
        public string Email { get; set; }
    }
}

