namespace CarGleam.DTOs
{
    public class LoginDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}

// no need to have a table to make a dto i.e from user table we can make a dto for login and signup