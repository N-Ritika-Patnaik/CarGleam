////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;

////namespace CarGleam.Test.Controller
////{
////    internal class MachineControllerTest
////    {
////    }
////}

//using CarGleam.Controllers;
//using CarGleam.Data;
//using CarGleam.DTOs;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using CarGleam.Models;
//using Microsoft.Extensions.Logging;
//using NUnit.Framework;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace CarGleam.Tests.Controllers
//{
//    [TestFixture] // indicate  contains NUnit tests
//    public class MachineControllerTests
//    {
//        private EFCoreDBContext _context;
//        private Mock<ILogger<MachineController>> _mockLogger;
//        private MachineController _controller;

//        [SetUp] // initializes the in-memory database, logger mock, and controller instance before each test.
//        public void SetUp()
//        {
//            var options = new DbContextOptionsBuilder<EFCoreDBContext>()
//                .UseInMemoryDatabase(databaseName: "CarGleamTest")
//                .Options;

//            _context = new EFCoreDBContext(options);
//            _mockLogger = new Mock<ILogger<MachineController>>();
//            _controller = new MachineController(_context);

//            ClearDatabase();
//            SeedDatabase();
//        }

//        [TearDown] // disposes of the database context after each test to clean up resources
//        public void TearDown()
//        {
//            _context.Dispose();
//        }

//        private void ClearDatabase() //removes all machines from the database for clean state before seeding.
//        {
//            _context.Machines.RemoveRange(_context.Machines);
//            _context.SaveChanges();
//        }

//        private void SeedDatabase() // adds two sample machines to the database for testing purposes.
//        {
//            var machine1 = new Machine
//            {
//                MachineId = 1,
//                MachineName = "Machine 1",
//                Status = "Available"
//            };

//            var machine2 = new Machine
//            {
//                MachineId = 2,
//                MachineName = "Machine 2",
//                Status = "In Use"
//            };

//            _context.Machines.AddRange(machine1, machine2);
//            _context.SaveChanges();
//        }

//        [Test] // returns an OkObjectResult containing a list of machines.
//        public async Task GetMachines_ReturnsOkResult_WithListOfMachines()
//        {
//            // Act
//            var result = await _controller.GetMachines();

//            // Assert
//            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
//            var okResult = result.Result as OkObjectResult;
//            Assert.That(okResult.Value, Is.InstanceOf<List<MachineDTO>>());
//            var machineList = okResult.Value as List<MachineDTO>;
//            Assert.That(machineList.Count, Is.EqualTo(2));
//        }

//        [Test]
//        public async Task GetMachine_ReturnsOkResult_WithMachine()
//        {
//            // Arrange
//            var machineId = 1;

//            // Act
//            var result = await _controller.GetMachine(machineId);

//            // Assert
//            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
//            var okResult = result.Result as OkObjectResult;
//            Assert.That(okResult.Value, Is.InstanceOf<MachineDTO>());
//        }

//        [Test]
//        public async Task GetMachine_ReturnsNotFound_WhenMachineNotFound()
//        {
//            // Arrange
//            var machineId = 99; // Machine ID that does not exist

//            // Act
//            var result = await _controller.GetMachine(machineId);

//            // Assert
//            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
//        }

//        [Test]
//        public async Task CreateMachine_ReturnsCreatedAtActionResult_WithMachine()
//        {
//            // Arrange
//            var machineDto = new MachineDTO
//            {
//                MachineName = "New Machine",
//                Status = "Available"
//            };

//            // Act
//            var result = await _controller.CreateMachine(machineDto);

//            // Assert
//            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
//            var createdResult = result.Result as CreatedAtActionResult;
//            Assert.That(createdResult.Value, Is.InstanceOf<MachineDTO>());
//        }

//        [Test]
//        public async Task CreateMachine_ReturnsBadRequest_WhenDtoIsInvalid()
//        {
//            // Arrange
//            var machineDto = new MachineDTO
//            {
//                MachineName = "Test machine", // Invalid value
//                Status = "Available"
//            };

//            _controller.ModelState.AddModelError("MachineName", "Required");

//            // Act
//            var result = await _controller.CreateMachine(machineDto);

//            // Assert
//            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
//        }

//        [Test]
//        public async Task UpdateMachine_ReturnsNoContent_WhenMachineUpdated()
//        {
//            // Arrange
//            var machineId = 1;
//            var machineDto = new MachineDTO
//            {
//                MachineId = machineId,
//                MachineName = "Updated Machine",
//                Status = "Available"
//            };

//            // Act
//            var result = await _controller.UpdateMachine(machineId, machineDto);

//            // Assert
//            Assert.That(result, Is.InstanceOf<NoContentResult>());
//        }

//        [Test]
//        public async Task UpdateMachine_ReturnsNotFound_WhenMachineNotFound()
//        {
//            // Arrange
//            var machineId = 99; // Machine ID that does not exist
//            var machineDto = new MachineDTO
//            {
//                MachineId = machineId,
//                MachineName = "Updated Machine",
//                Status = "Available"
//            };

//            // Act
//            var result = await _controller.UpdateMachine(machineId, machineDto);

//            // Assert
//            Assert.That(result, Is.InstanceOf<NotFoundResult>());
//        }

//        [Test]
//        public async Task DeleteMachine_ReturnsNoContent_WhenMachineDeleted()
//        {
//            // Arrange
//            var machineId = 1;

//            // Act
//            var result = await _controller.DeleteMachine(machineId);

//            // Assert
//            Assert.That(result, Is.InstanceOf<NoContentResult>());
//        }

//        [Test]
//        public async Task DeleteMachine_ReturnsNotFound_WhenMachineNotFound()
//        {
//            // Arrange
//            var machineId = 99; // Machine ID that does not exist

//            // Act
//            var result = await _controller.DeleteMachine(machineId);

//            // Assert
//            Assert.That(result, Is.InstanceOf<NotFoundResult>());
//        }
//    }
//}

