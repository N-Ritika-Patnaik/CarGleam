using CarGleam.Data;
using CarGleam.DTOs;
using CarGleam.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarGleam.Services;
using Microsoft.AspNetCore.Authorization;

namespace CarGleam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly EFCoreDBContext _context;
        private readonly EmailNotificationService _emailNotificationService; 
        public BookingController(EFCoreDBContext context, EmailNotificationService emailNotificationService)
        {
            _context = context;
            _emailNotificationService = emailNotificationService;

        }

        // GET: api/Bookings
        [HttpGet]
        [Authorize(Roles ="Admin")]        
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookings()
        {
            return await _context.Bookings
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    ServiceLocationId = b.ServiceLocationId,
                    MachineId = b.MachineId,
                    ServiceDate = b.ServiceDate
                })
                .ToListAsync(); // asynchronously convert a query result into a list. to retrieve data from the database without blocking the main thread
        }

        // GET: api/Bookings/5
        [HttpGet("GetBookingBy/{id}")]
        public async Task<ActionResult<BookingDTO>> GetBooking(int id)
        {
            //linq query
            var booking = await _context.Bookings
                .Where(b => b.BookingId == id)
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    ServiceLocationId = b.ServiceLocationId,
                    MachineId = b.MachineId,
                    ServiceDate = b.ServiceDate
                })
                .FirstOrDefaultAsync();  //method returns the first matching booking or null if no match is found

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        // POST: api/Bookings
        [HttpPost("AddBooking")]
        public async Task<ActionResult<BookingDTO>> CreateBooking(BookingDTO bookingDTO)
        {   
            if (bookingDTO == null)
            {
                return BadRequest("Booking is required.");
            }

            //-------------new 26.----------
            var machine = await _context.Machines.FindAsync(bookingDTO.MachineId);
            if (machine == null)
            {
                return NotFound("Machine not found.");
            }

            // Check if the machine is available for the specified date at same location
            var isAvailable = !await _context.Bookings
                .AnyAsync(b => b.MachineId == bookingDTO.MachineId && b.ServiceDate == bookingDTO.ServiceDate && b.ServiceLocationId == bookingDTO.ServiceLocationId);

            if (!isAvailable)
            {
                return BadRequest("Machine is not available for the specified date and time at the selected service location.");
            }
            //----------------------------------------------------

            var booking = new Booking
            {
                UserId = bookingDTO.UserId,
                ServiceLocationId = bookingDTO.ServiceLocationId,
                MachineId = bookingDTO.MachineId,
                ServiceDate = bookingDTO.ServiceDate
            };

            _context.Bookings.Add(booking);
            //------------new 26.----------------
            machine.Status = "Unavailable";           // Update machine status to "Unavailable"

            _context.Entry(machine).State = EntityState.Modified; // mark updated
            //----------------------------
            await _context.SaveChangesAsync();

            bookingDTO.BookingId = booking.BookingId;

            // Send notification
            var user = await _context.Users.FindAsync(booking.UserId);
            if (user != null)
            {
                var subject = "Booking Confirmation";
                var message = $"Dear {user.FullName},<br/><br/>Your booking has been confirmed.<br/>Booking ID: {booking.BookingId}<br/>Service Date: {booking.ServiceDate}<br/><br/>Thank you!";
                Console.WriteLine(user.Email, subject, message);
                await _emailNotificationService.SendEmailAsync(user.Email, subject, message);
            }

            return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, bookingDTO);
        }

        
     
    }
 
}


//--------------------------------------------------------------------------------------------------------
//// POST: api/Bookings
//[HttpPost("AddBooking")]
//public async Task<ActionResult<BookingDTO>> CreateBooking(BookingDTO bookingDTO)
//{
//    //var subject = "Booking Confirmation";
//    //var message = $"Dear {"Holy"},<br/><br/>Your booking has been confirmed.<br/>Booking ID: {"#223"}<br/>Service Date: {"12 feb 2025"}<br/><br/>Thank you!";
//    //await _emailNotificationService.SendEmailAsync("ritikapatnaik.n@gmail.com", subject, message);

//    if (bookingDTO == null)
//    {
//        return BadRequest("Booking is required.");
//    }

//    // Check if the MachineId exists in the Machines table
//    //var machineExists = await _context.Machines.AnyAsync(m => m.MachineId == bookingDTO.MachineId);
//    //if (!machineExists)
//    //{
//    //    return BadRequest("Invalid MachineId. The specified machine does not exist.");
//    //}

// PUT: api/Bookings/5
//[HttpPut("UpdateBooking/{id}")]
//public async Task<IActionResult> UpdateBooking(int id, BookingDTO bookingDTO)
//{
//    if (id != bookingDTO.BookingId)
//    {
//        return BadRequest();
//    }
//    var booking = await _context.Bookings.FindAsync(id);
//    if (booking == null)
//    {
//        return NotFound();
//    }
//    // Check if the MachineId exists in the Machines table
//    var machineExists = await _context.Machines.AnyAsync(m => m.MachineId == bookingDTO.MachineId);
//    if (!machineExists)
//    {
//        return BadRequest("Invalid MachineId. The specified machine does not exist.");
//    }
//    booking.UserId = bookingDTO.UserId;
//    booking.ServiceLocationId = bookingDTO.ServiceLocationId;
//    booking.MachineId = bookingDTO.MachineId;
//    booking.ServiceDate = bookingDTO.ServiceDate;
//    _context.Entry(booking).State = EntityState.Modified;
//    try
//    {
//        await _context.SaveChangesAsync();
//    }
//    catch (DbUpdateConcurrencyException)
//    {
//        if (!BookingExists(id))
//        {
//            return NotFound();
//        }
//        else
//        {
//            throw;
//        }
//    }
//    return NoContent();
//}

// DELETE: api/Bookings/5
//    [HttpDelete("DeleteBooking/{id}")]
//    public async Task<IActionResult> DeleteBooking(int id)
//    {
//        var booking = await _context.Bookings.FindAsync(id);
//        if (booking == null)
//        {
//            return NotFound();
//        }
//        _context.Bookings.Remove(booking);
//        await _context.SaveChangesAsync();
//        return NoContent();
//    }
//    private bool BookingExists(int id)
//    {
//        return _context.Bookings.Any(e => e.BookingId == id);
//    }