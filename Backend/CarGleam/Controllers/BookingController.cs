using CarGleam.Data;
using CarGleam.DTOs;
using CarGleam.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarGleam.Services;

namespace CarGleam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly EFCoreDBContext _context;
        private readonly EmailNotificationService _emailNotificationService;
        private string serviceType;

        public BookingController(EFCoreDBContext context, EmailNotificationService emailNotificationService)
        {
            _context = context;
            _emailNotificationService = emailNotificationService;

        }

        //GET: api/Bookings
        [HttpGet("GetAllBookings")]
        public async Task<ActionResult<IEnumerable<BookingResponseDTO>>> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Select(b => new BookingResponseDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    FullName =b.FullName,
                    ServiceName = b.ServiceLocation.ServiceName,
                    LocationName = b.ServiceLocation.LocationName,
                    MachineName = b.Machine.MachineName,
                    MachineId = b.MachineId,
                    ServiceDate = b.ServiceDate
                })
                .ToListAsync();

            return Ok(bookings);
        }

        // GET: api/Bookings/5
        [HttpGet("GetBookingBy/{id}")]
        public async Task<ActionResult<BookingDTO>> GetBooking(int id)
        {
            // LINQ query to get the booking by ID and user ID
            var booking = await _context.Bookings
                .Where(b => b.BookingId == id)
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    FullName = b.User.FullName,
                    ServiceLocationId = b.ServiceLocationId,
                    MachineId = b.MachineId,
                    ServiceDate = b.ServiceDate
                })
                .FirstOrDefaultAsync();  // method returns the first matching booking or null if no match is found

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        // GET: api/Bookingsbyuserid/5
        [HttpGet("GetBookingsByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<BookingResponseDTO>>> GetBookingsByUserId(int userId)
        {
            var bookings = await _context.Bookings
                 .Where(b => b.UserId == userId)
                .Select(b => new BookingResponseDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    ServiceName = b.ServiceLocation.ServiceName,
                    LocationName = b.ServiceLocation.LocationName,
                    MachineName = b.Machine.MachineName,
                    ServiceDate = b.ServiceDate
                })
               .ToListAsync();


            if (bookings == null || !bookings.Any())
            {
                return NotFound();
            }

            return Ok(bookings);
        }

        // GET: api/BookingsbyServicetype/5
        [HttpGet("GetBookingsByServiceType/{serviceType}")]
        public async Task<ActionResult<IEnumerable<object>>> GetBookingsByServiceType(string serviceType)
        {
            var result = await _context.ServiceLocations
                .Where(sl => sl.ServiceType == serviceType)
                .Select(sl => new
                {
                    sl.ServiceLocationId,
                    sl.ServiceName,
                    sl.LocationName,
                    sl.ServiceType,
                    Machines = _context.Machines
                        .Where(m => m.MachineType == sl.ServiceType)
                        .Select(m => new
                        {
                            m.MachineId,
                            m.MachineName,
                            m.Status,
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(result);
        }

        // POST: api/Bookings
        [HttpPost("AddBooking")]
        public async Task<ActionResult<BookingDTO>> CreateBooking(BookingDTO bookingDTO)
        {
            if (bookingDTO == null)
            {
                return BadRequest("Booking is required.");
            }

            var machine = await _context.Machines.FindAsync(bookingDTO.MachineId);
            if (machine == null)
            {
                return NotFound("Machine not found.");
            }

            // Define the buffer time
            TimeSpan bufferTime = TimeSpan.FromMinutes(20);

            // Calculate the start and end times for the new booking
            DateTime newBookingStart = bookingDTO.ServiceDate;
            DateTime newBookingEnd = bookingDTO.ServiceDate.Add(machine.Duration + bufferTime);

            // Retrieve all bookings for the machine and perform the availability check in memory
            var bookings = await _context.Bookings
                .Where(b => b.MachineId == bookingDTO.MachineId)
                .ToListAsync();

            var isAvailable = bookings
                .All(b => b.ServiceDate.Add(b.Machine.Duration + bufferTime) <= newBookingStart || b.ServiceDate >= newBookingEnd);

            //if (!isAvailable)
            //{
            //    return BadRequest("Machine is not available for the specified date and time at the selected service location.");
            //}

            var service = await _context.ServiceLocations.SingleOrDefaultAsync(s => s.ServiceLocationId == bookingDTO.ServiceLocationId);

            var booking = new Booking
            {
                UserId = bookingDTO.UserId,
                ServiceLocationId = bookingDTO.ServiceLocationId,
                MachineId = bookingDTO.MachineId,
                ServiceDate = bookingDTO.ServiceDate,
                FullName = bookingDTO.FullName,
                User = null,
                Transactions = new List<Transaction>()
            };

            _context.Bookings.Add(booking);
            machine.Status = "Unavailable"; // Update machine status to "Unavailable"
            _context.Entry(machine).State = EntityState.Modified; // Mark updated
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


//[HttpGet]
// [Authorize(Roles = "Admin")]
// public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
// {
//     return await _context.Bookings
//         .ToListAsync(); // asynchronously convert a query result into a list. to retrieve data from the database without blocking the main thread
// }

//[HttpGet("GetBookingBy/{id}")]
//public async Task<ActionResult<BookingDTO>> GetBooking(int id)
//{
//    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
//    //linq query
//    var booking = await _context.Bookings
//        .Where(b => b.BookingId == id)
//        //.Where(b => b.UserId == userId)
//        .Select(b => new BookingDTO
//        {
//            BookingId = b.BookingId,
//            UserId = b.UserId,
//            FullName = b.User.FullName,
//            ServiceLocationId = b.ServiceLocationId,
//            ServiceName = b.ServiceLocation.ServiceName,
//            LocationName = b.ServiceLocation.LocationName,
//            MachineId = b.MachineId,
//            MachineName = b.Machine.MachineName,
//            ServiceDate = b.ServiceDate
//        })
//        .FirstOrDefaultAsync();  //method returns the first matching booking or null if no match is found
//    if (booking == null)
//    {
//        return NotFound();
//    }
//    return booking;
//}

// POST: api/Bookings
//    [HttpPost("AddBooking")]
//    public async Task<ActionResult<BookingDTO>> CreateBooking(BookingDTO bookingDTO)
//    {
//        if (bookingDTO == null)
//        {
//            return BadRequest("Booking is required.");
//        }
//        var machine = await _context.Machines.FindAsync(bookingDTO.MachineId);
//        if (machine == null)
//        {
//            return NotFound("Machine not found.");
//        }
//        // Define the buffer time
//        TimeSpan bufferTime = TimeSpan.FromMinutes(20);
//        // Check if the machine is available for the specified date and time
//        //var isAvailable = await _context.Bookings
//        //    .Where(b => b.MachineId == bookingDTO.MachineId)
//        //    .AllAsync(b => b.ServiceDate < bookingDTO.ServiceDate || b.ServiceDate >= bookingDTO.ServiceDate.Add(machine.Duration));
//        // Calculate the start and end times for the new booking
//        DateTime newBookingStart = bookingDTO.ServiceDate;
//        DateTime newBookingEnd = bookingDTO.ServiceDate.Add(machine.Duration + bufferTime);
//        // Check if the machine is available for the specified date and time with buffer
//        //var isAvailable = await _context.Bookings
//        //    .Where(b => b.MachineId == bookingDTO.MachineId)
//        //    .AllAsync(b => b.ServiceDate.Add(machine.Duration + bufferTime) <= bookingDTO.ServiceDate || b.ServiceDate >= bookingDTO.ServiceDate.Add(machine.Duration + bufferTime));
//        var isAvailable = await _context.Bookings
//            .Where(b => b.MachineId == bookingDTO.MachineId)
//            .AllAsync(b => b.ServiceDate.Add(b.Machine.Duration + bufferTime) <= newBookingStart || b.ServiceDate >= newBookingEnd);
//        if (!isAvailable)
//        {
//            return BadRequest("Machine is not available for the specified date and time at the selected service location.");
//        }
//        var booking = new Booking
//        {
//            UserId = bookingDTO.UserId,
//            ServiceLocationId = bookingDTO.ServiceLocationId,
//            MachineId = bookingDTO.MachineId,
//            ServiceDate = bookingDTO.ServiceDate
//        };
//        _context.Bookings.Add(booking);
//        machine.Status = "Unavailable"; // Update machine status to "Unavailable"
//        _context.Entry(machine).State = EntityState.Modified; // Mark updated
//        await _context.SaveChangesAsync();
//        bookingDTO.BookingId = booking.BookingId;
//        // Send notification
//        var user = await _context.Users.FindAsync(booking.UserId);
//        if (user != null)
//        {
//            var subject = "Booking Confirmation";
//            var message = $"Dear {user.FullName},<br/><br/>Your booking has been confirmed.<br/>Booking ID: {booking.BookingId}<br/>Service Date: {booking.ServiceDate}<br/><br/>Thank you!";
//            Console.WriteLine(user.Email, subject, message);
//            await _emailNotificationService.SendEmailAsync(user.Email, subject, message);
//        }
//        return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, bookingDTO);
//    }
//}
//    }



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