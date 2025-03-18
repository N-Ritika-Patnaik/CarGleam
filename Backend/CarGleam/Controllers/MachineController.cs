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
    public class MachineController : ControllerBase
    {
        private readonly EFCoreDBContext _context;
        public MachineController(EFCoreDBContext context)
        {
            _context = context;
        }
        // GET: api/Machines
        [HttpGet]
       public async Task<ActionResult<IEnumerable<MachineDTO>>> GetMachines()
        {
            return await _context.Machines
                .Select(m => new MachineDTO
                {
                    MachineId = m.MachineId,
                    MachineName = m.MachineName,
                    Status = m.Status,
                    MachineType = m.MachineType,
                    Duration = m.Duration
                })
                .ToListAsync();
        }

        // GET: api/Machines/5
        [HttpGet("GetMachineBy/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MachineDTO>> GetMachine(int id)
        {
            var machine = await _context.Machines
                .Where(m => m.MachineId == id)
                .Select(m => new MachineDTO
                {
                    MachineId = m.MachineId,
                    MachineName = m.MachineName,
                    Status = m.Status,
                    MachineType = m.MachineType,
                    Duration = m.Duration
                })
                .FirstOrDefaultAsync();

            if (machine == null)
            {
                return NotFound();
            }

            return machine;
        }

        // GET: api/Machines/CheckAvailability
        //[HttpGet("checkavailability")]
        //public async Task<ActionResult<bool>> CheckMachineAvailability(int machineId, DateTime serviceDate)
        //{
        //    var isAvailable = !await _context.Bookings
        //        .AnyAsync(b => b.MachineId == machineId && b.ServiceDate == serviceDate);

        //    return Ok(isAvailable);
        //}

        // POST: api/Machines
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MachineDTO>> CreateMachine(MachineDTO machineDTO)
        {
            if (machineDTO == null)
            {
                return BadRequest("Machine is required.");
            }
            // Check if the machine  already exists
            if (await _context.Machines.AnyAsync(m => m.MachineName == machineDTO.MachineName))
            {
                return BadRequest("Machine already exists.");
            }

            var machine = new Machine
            {
                MachineName = machineDTO.MachineName,
                MachineType = machineDTO.MachineType,
                Duration = machineDTO.Duration
                //Status = machineDTO.Status
            };

            _context.Machines.Add(machine);
            await _context.SaveChangesAsync();

            machineDTO.MachineId = machine.MachineId;

            return CreatedAtAction(nameof(GetMachine), new { id = machine.MachineId }, machineDTO);
        }

        // PUT: api/Machines/5
        [HttpPut("UpdateMachine/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMachine(int id, MachineDTO machineDTO)
        {
            if (id != machineDTO.MachineId)
            {
                return BadRequest();
            }

            var machine = await _context.Machines.FindAsync(id);
            if (machine == null)
            {
                return NotFound();
            }

            machine.MachineName = machineDTO.MachineName;
            machine.MachineType = machineDTO.MachineType;
            machine.Duration = machineDTO.Duration;

            _context.Entry(machine).State = EntityState.Modified; // mark updated

            //// exception handling for concurrency conflicts i. e . when two users try to update the same record at the same time
            //try
            //{
            await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!MachineExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return NoContent();
        }

        // DELETE: api/Machines/5
        [HttpDelete("DeleteMachine/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMachine(int id)
        {
            var machine = await _context.Machines.FindAsync(id);
            if (machine == null)
            {
                return NotFound();
            }

            _context.Machines.Remove(machine);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool MachineExists(int id)
        {
            return _context.Machines.Any(e => e.MachineId == id);
        }
    }
}
