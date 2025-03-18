using CarGleam.Data;
using CarGleam.DTOs;
using CarGleam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarGleam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly EFCoreDBContext _context;
        public TransactionController(EFCoreDBContext context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        [Authorize(Roles= "Admin")]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetTransactions()
        {
            return await _context.Transactions
                .Select(t => new TransactionDTO
                {
                    TransactionId = t.TransactionId,
                    BookingId = t.BookingId,
                    PaymentAmount = t.PaymentAmount,
                    PaymentMethod = t.PaymentMethod,
                    CardNumber = t.CardNumber,
                    CardExpiry = t.CardExpiry,
                    UpiId = t.UpiId,
                    PaymentStatus = t.PaymentStatus,
                })
                .ToListAsync();
        }

        // GET: api/Transactions/5
        [HttpGet("GetTransactionBy/{id}")]
        public async Task<ActionResult<TransactionDTO>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions
                .Where(t => t.TransactionId == id)
                .Select(t => new TransactionDTO
                {
                    TransactionId = t.TransactionId,
                    BookingId = t.BookingId,
                    PaymentAmount = t.PaymentAmount,
                    PaymentMethod = t.PaymentMethod,
                    CardNumber = t.CardNumber,
                    CardExpiry = t.CardExpiry,
                    UpiId = t.UpiId,
                    PaymentStatus = t.PaymentStatus,
                })
                .FirstOrDefaultAsync();

            if (transaction == null)
            {
                return NotFound("Transaction not found.");
            }

            return transaction;
        }

        // POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<TransactionDTO>> CreateTransaction(TransactionDTO transactionDTO)
        {
            if (transactionDTO == null)
            {
                return BadRequest("Transaction is required.");
            }

            // Validate payment method
            if (transactionDTO.PaymentMethod == "Cash")
            {
                transactionDTO.CardNumber = null;
                transactionDTO.CardExpiry = null;
                transactionDTO.UpiId = null;
            }
            else if (transactionDTO.PaymentMethod == "Card")
            {
                if (transactionDTO.CardNumber == null || transactionDTO.CardExpiry == null)
                {
                    return BadRequest("Card Number and Card Expiry are required when Payment Method is Card.");
                }
                transactionDTO.UpiId = null;
            }
            else if (transactionDTO.PaymentMethod == "Upi")
            {
                if (transactionDTO.UpiId == null)
                {
                    return BadRequest("Upi Id is required when Payment Method is Upi.");
                }
                transactionDTO.CardNumber = null;
                transactionDTO.CardExpiry = null;
            }
            else
            {
                return BadRequest("Invalid Payment Method. Must be 'Cash', 'Card', or 'Upi'.");
            }

            var transaction = new Transaction
            {
                BookingId = transactionDTO.BookingId,
                PaymentAmount = transactionDTO.PaymentAmount,
                PaymentMethod = transactionDTO.PaymentMethod,
                CardNumber = transactionDTO.CardNumber,
                CardExpiry = transactionDTO.CardExpiry,
                UpiId = transactionDTO.UpiId,
                PaymentStatus = transactionDTO.PaymentStatus,
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            transactionDTO.TransactionId = transaction.TransactionId;

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.TransactionId }, transactionDTO);
        }

    }
}



//---------------------------------------------------------------------------------------------------------------------

// PUT: api/Transactions/5
//[HttpPut("UpdateTransaction/{id}")]
//public async Task<IActionResult> UpdateTransaction(int id, TransactionDTO transactionDTO)
//{
//    if (id != transactionDTO.TransactionId)
//    {
//        return BadRequest();
//    }
//    var transaction = await _context.Transactions.FindAsync(id);
//    if (transaction == null)
//    {
//        return NotFound();
//    }
//    transaction.BookingId = transactionDTO.BookingId;
//    transaction.PaymentAmount = transactionDTO.PaymentAmount;
//    transaction.PaymentStatus = transactionDTO.PaymentStatus;
//    transaction.NotificationMessage = transactionDTO.NotificationMessage;
//    _context.Entry(transaction).State = EntityState.Modified;
//    try
//    {
//        await _context.SaveChangesAsync();
//    }
//    catch (DbUpdateConcurrencyException)
//    {
//        if (!TransactionExists(id))
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

// DELETE: api/Transactions/5
//[HttpDelete("DeleteTransaction/{id}")]
//public async Task<IActionResult> DeleteTransaction(int id)
//{
//    var transaction = await _context.Transactions.FindAsync(id);
//    if (transaction == null)
//    {
//        return NotFound();
//    }
//    _context.Transactions.Remove(transaction);
//    await _context.SaveChangesAsync();
//    return NoContent();
//}
//private bool TransactionExists(int id)
//{
//    return _context.Transactions.Any(e => e.TransactionId == id);
//}

//        // PUT: api/Transactions/5
//        //[HttpPut("UpdateTransaction/{id}")]
//        //public async Task<IActionResult> UpdateTransaction(int id, TransactionDTO transactionDTO)
//        //{
//        //    if (id != transactionDTO.TransactionId)
//        //    {
//        //        return BadRequest();
//        //    }
//        //    var transaction = await _context.Transactions.FindAsync(id);
//        //    if (transaction == null)
//        //    {
//        //        return NotFound();
//        //    }
//        //    transaction.BookingId = transactionDTO.BookingId;
//        //    transaction.PaymentAmount = transactionDTO.PaymentAmount;
//        //    transaction.PaymentStatus = transactionDTO.PaymentStatus;
//        //    transaction.NotificationMessage = transactionDTO.NotificationMessage;
//        //    _context.Entry(transaction).State = EntityState.Modified;
//        //    try
//        //    {
//        //        await _context.SaveChangesAsync();
//        //    }
//        //    catch (DbUpdateConcurrencyException)
//        //    {
//        //        if (!TransactionExists(id))
//        //        {
//        //            return NotFound();
//        //        }
//        //        else
//        //        {
//        //            throw;
//        //        }
//        //    }
//        //    return NoContent();
//        //}

//        // DELETE: api/Transactions/5
//        //[HttpDelete("DeleteTransaction/{id}")]
//        //public async Task<IActionResult> DeleteTransaction(int id)
//        //{
//        //    var transaction = await _context.Transactions.FindAsync(id);
//        //    if (transaction == null)
//        //    {
//        //        return NotFound();
//        //    }
//        //    _context.Transactions.Remove(transaction);
//        //    await _context.SaveChangesAsync();
//        //    return NoContent();
//        //}
//        //private bool TransactionExists(int id)
//        //{
//        //    return _context.Transactions.Any(e => e.TransactionId == id);
//        //}
//    }
//}