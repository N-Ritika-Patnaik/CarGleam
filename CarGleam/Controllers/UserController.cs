using System.Text;
using System.Security.Cryptography;
using CarGleam.Data;
using CarGleam.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using System.Security.Claims;


namespace CarGleam.Controllers
{
    [Route("api/[controller]")] //-------name that needs to be entered in the postman ----/api/controller_name
    [ApiController]
    public class UserController : ControllerBase 
    {
        private readonly EFCoreDBContext _context; // creating an variable/field of EFCoreDBContext
        public UserController(EFCoreDBContext context) // dependency injection so that we can access all the methods of EFCoreDBContext
        {
            _context = context;         //accessing private variable/field of EFCoreDBContext
        }

        // GET: api/Users 
        [HttpGet]                      //-----this outputs all the users that are stored in database
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {                             //ienumerable is a collection of items (user dto) that can be enumerated one at a time

            return await _context.Users //accessing Users table from EFCoreDBContext
                .Select(u => new UserDTO 
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email
                })
                .ToListAsync(); //returns a list of all users stored in the database
        }

        //GET: api/Users/5 
       [HttpGet("GetUserBy/{id}")]
       //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            //---------------------------26.2.25
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value); //getting the user id from the token and converting it to int

            if (userId != id) // user can access only his/her details
            {
                return Forbid("You are not authorized to view this user's details."); 
            }
            //----------------------------

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDTO = new UserDTO
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email
            };

            return userDTO; //returns the user details
        }

        // GET: api/Users/IsUserActive/5
        [HttpGet("IsUserActive/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> GetUserActiveStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user.IsActive; //returns the active status of the user
        }

        // PUT: api/Users/5 // update user details
        [HttpPut("UpdateUser/{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> UpdateUser(int id, UserDTO userDTO)
        {
            if (id != userDTO.UserId)
            {
                return BadRequest("User ID mismatch.");
            }

            if (!IsValidPassword(userDTO.Password))
            {
                return BadRequest("Password must be at least 6 characters long and contain at least one number and one special character.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = userDTO.FullName;
            user.Email = userDTO.Email;
            user.Password = HashPassword(userDTO.Password);

            _context.Entry(user).State = EntityState.Modified; //updating the user details

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) 
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); 
        }

        // DELETE: api/Users/5
        [HttpDelete("DeleteUser/{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create()) 
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password)); 
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower(); //converting the hashed password to string
            }
        }

        //private bool VerifyPassword(string enteredPassword, string storedHash)
        //{
        //    var enteredHash = HashPassword(enteredPassword);
        //    return enteredHash == storedHash;
        //}

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
        private bool IsValidPassword(string password)
        {
            if (password.Length < 6)
            {
                return false;
            }

            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                return false;
            }

            if (!Regex.IsMatch(password, @"[!@#$%^&*]"))
            {
                return false;
            }

            return true;
        }
    }
}

