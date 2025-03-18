using System.Text;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using CarGleam.Data;
using CarGleam.DTOs;
using CarGleam.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarGleam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EFCoreDBContext _context;
        private readonly IConfiguration _configuration; // to access appsettings.json
        public AuthController(EFCoreDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Auth/signup
        [HttpPost("Signup")]
        public async Task<ActionResult<object>> Signup(UserDTO userDTO)
        {
            //lambda expression, linq query
            if (_context.Users.Any(u => u.Email == userDTO.Email)) // u me sara detail chala gaya and usse email and dto.email nikal ke compare
            {
                return Conflict("A user with this Email already exists.");
            }

            var user = new User
            {
                FullName = userDTO.FullName,
                Email = userDTO.Email,
                Password = HashPassword(userDTO.Password),
                Role = "User" // Default role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            userDTO.UserId = user.UserId;

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        // POST: api/Auth/login
        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDTO.Email); // single user with email

            if (user == null)
            {
                return NotFound("User not found.");
            }
            //if (!user.IsActive)
            if (user.IsActive == false)

            {
                return Unauthorized("User account is inactive.");
            }

            if (!VerifyPassword(loginDTO.Password, user.Password))
            {
                return Unauthorized("Invalid password.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSetting");

            var key = Encoding.ASCII.GetBytes(jwtSettings["key"]);

            var tokenHandler = new JwtSecurityTokenHandler();  // create and write token
            var tokenDescriptor = new SecurityTokenDescriptor  // describes properties of token
            {
                //Claims are key-value pairs provide details about the user and their identity.
                Subject = new ClaimsIdentity(new[]
                {
                    //new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim("fullName", user.FullName),
                    //new Claim("role", user.Role),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1), // token expiration time
                Issuer = jwtSettings["issuer"],    // issuer is the entity that issues the token
                Audience = jwtSettings["audience"],  // audience is the intended recipient of the token`
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        //generates fixed-size(256 - bit) hash value from input data, one - way function, infeasible to reverse the hash to obtain the original input.
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var enteredHash = HashPassword(enteredPassword);
            return enteredHash == storedHash;
        }
    }
}