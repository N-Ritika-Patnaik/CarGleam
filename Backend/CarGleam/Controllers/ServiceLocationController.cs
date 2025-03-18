using CarGleam.Data;
using CarGleam.DTOs;
using CarGleam.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CarGleam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceLocationController : ControllerBase
    {
        private readonly EFCoreDBContext _context;
        public ServiceLocationController(EFCoreDBContext context)
        {
            _context = context;
        }

        // GET: api/ServiceLocations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceLocationDTO>>> GetServiceLocations()
        {
            return await _context.ServiceLocations
                .Select(sl => new ServiceLocationDTO
                {
                    ServiceLocationId = sl.ServiceLocationId,
                    ServiceName = sl.ServiceName,
                    Price = sl.Price,
                    LocationName = sl.LocationName,
                    ServiceType = sl.ServiceType
                })
                .ToListAsync();
        }

        // GET: api/ServiceLocations/5
        [HttpGet("GetServiceLocationBy/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceLocationDTO>> GetServiceLocation(int id)
        {
            var serviceLocation = await _context.ServiceLocations
                .Where(sl => sl.ServiceLocationId == id)
                .Select(sl => new ServiceLocationDTO
                {
                    ServiceLocationId = sl.ServiceLocationId,
                    ServiceName = sl.ServiceName,
                    Price = sl.Price,
                    LocationName = sl.LocationName
                })
                .FirstOrDefaultAsync(); // takes the first value

            if (serviceLocation == null)
            {
                return NotFound();
            }

            return serviceLocation;
        }

        // POST: api/ServiceLocations
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceLocationDTO>> CreateServiceLocation(ServiceLocationDTO serviceLocationDTO)
        {
            if (serviceLocationDTO == null)
            {
                return BadRequest("ServiceLocation is required.");
            }
            // Check if service name and location name already exists
            if (await _context.ServiceLocations.AnyAsync(sl => sl.ServiceName == serviceLocationDTO.ServiceName && sl.LocationName == serviceLocationDTO.LocationName))
            {
                return BadRequest("ServiceLocation  already exists.");
            }

            var serviceLocation = new ServiceLocation
            {
                ServiceName = serviceLocationDTO.ServiceName,
                Price = serviceLocationDTO.Price,
                LocationName = serviceLocationDTO.LocationName,
                ServiceType = serviceLocationDTO.ServiceType
            };

            _context.ServiceLocations.Add(serviceLocation);
            await _context.SaveChangesAsync();

            serviceLocationDTO.ServiceLocationId = serviceLocation.ServiceLocationId;

            return CreatedAtAction(nameof(GetServiceLocation), new { id = serviceLocation.ServiceLocationId }, serviceLocationDTO); 
        }

        // PUT: api/ServiceLocations/5
        [HttpPut("UpdateServiceLocation/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateServiceLocation(int id, ServiceLocationDTO serviceLocationDTO)
        {
            if (id == serviceLocationDTO.ServiceLocationId)
            {
                var serviceLocation = await _context.ServiceLocations.FindAsync(id);
                if (serviceLocation == null)
                {
                    return NotFound();
                }

                serviceLocation.ServiceName = serviceLocationDTO.ServiceName;
                serviceLocation.Price = serviceLocationDTO.Price;
                serviceLocation.LocationName = serviceLocationDTO.LocationName;
                serviceLocation.ServiceType = serviceLocationDTO.ServiceType;

                _context.Entry(serviceLocation).State = EntityState.Modified;
                await _context.SaveChangesAsync();


                return NoContent();
            }

            return BadRequest();
        }

        // DELETE: api/ServiceLocations/5
        [HttpDelete("DeleteServiceLocation/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteServiceLocation(int id)
        {
            var serviceLocation = await _context.ServiceLocations.FindAsync(id);
            if (serviceLocation == null)
            {
                return NotFound();
            }

            _context.ServiceLocations.Remove(serviceLocation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceLocationExists(int id)
        {
            return _context.ServiceLocations.Any(e => e.ServiceLocationId == id);
        }
    }
}