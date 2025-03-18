using CarGleam.Controllers;
using CarGleam.Data;
using CarGleam.DTOs;
using CarGleam.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarGleam.Test.Controllers
{
    [TestFixture]
    public class TransactionControllerTest
    {
        private EFCoreDBContext _context;
        private Mock<ILogger<TransactionController>> _mockLogger;
        private TransactionController _controller;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EFCoreDBContext>()
                .UseInMemoryDatabase(databaseName: "CarGleamTest")
                .Options;
            _context = new EFCoreDBContext(options);
            _mockLogger = new Mock<ILogger<TransactionController>>();
            _controller = new TransactionController(_context);
            ClearDatabase();
            SeedDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void ClearDatabase()
        {
            _context.Transactions.RemoveRange(_context.Transactions);
            _context.SaveChanges();
        }

        private void SeedDatabase()
        {
            var transaction = new Transaction
            {
                TransactionId = 1,
                BookingId = 1,
                PaymentAmount = 1000m,
                PaymentMethod = "Card",
                CardNumber = "1234567890123456",
                CardExpiry = "12/25",
                UpiId = null,
                PaymentStatus = "Completed"
            };
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }

        //[Test]
        //public async Task GetTransactions_ReturnsOkResult_WithListOfTransactions()
        //{
        //    // Act
        //    var result = await _controller.GetTransactions();

        //    // Assert
        //    Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        //    var okResult = result.Result as OkObjectResult;
        //    Assert.That(okResult.Value, Is.InstanceOf<List<TransactionDTO>>());
        //    var transactionList = okResult.Value as List<TransactionDTO>;
        //    Assert.That(transactionList.Count, Is.EqualTo(1));
        //}

        //[Test]
        //public async Task GetTransaction_ReturnsOkResult_WithTransaction()
        //{
        //    // Arrange
        //    var transactionId = 1;

        //    // Act
        //    var result = await _controller.GetTransaction(transactionId);

        //    // Assert
        //    Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        //    var okResult = result.Result as OkObjectResult;
        //    Assert.That(okResult.Value, Is.InstanceOf<TransactionDTO>());
        //}

        [Test]
        public async Task GetTransaction_ReturnsNotFound_WhenTransactionNotFound()
        {
            // Arrange
            var transactionId = 99; // Transaction ID that does not exist in the seeded data

            // Act
            var result = await _controller.GetTransaction(transactionId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task CreateTransaction_ReturnsCreatedAtActionResult_WithTransaction()
        {
            // Arrange
            var transactionDto = new TransactionDTO
            {
                BookingId = 2,
                PaymentAmount = 500m,
                PaymentMethod = "Cash",
                CardNumber = null,
                CardExpiry = null,
                UpiId = null,
                PaymentStatus = "Pending"
            };

            // Act
            var result = await _controller.CreateTransaction(transactionDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult.Value, Is.InstanceOf<TransactionDTO>());
        }

        [Test]
        public async Task CreateTransaction_ReturnsBadRequest_WhenDtoIsInvalid()
        {
            // Arrange
            var transactionDto = new TransactionDTO
            {
                BookingId = 2,
                PaymentAmount = 500m,
                PaymentMethod = "Card",
                CardNumber = null, // Invalid value
                CardExpiry = null, // Invalid value
                UpiId = null,
                PaymentStatus = "Pending"
            };

            // Act
            var result = await _controller.CreateTransaction(transactionDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}