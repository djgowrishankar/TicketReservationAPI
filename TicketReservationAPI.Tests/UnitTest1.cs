using NUnit.Framework;
using TicketReservationAPI.Controllers;
using TicketReservationAPI.Models;
using TicketReservationAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace TicketReservationAPI.Tests
{
    public class EventControllerTests
    {
        private TicketReservationContext _context;
        private EventController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TicketReservationContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new TicketReservationContext(options);
            _controller = new EventController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose of the context after each test
            _context.Dispose();
        }

        [Test]
        public void AddEvent_ShouldReturnOkResult_WhenEventIsValid()
        {
            // Arrange
            var newEvent = new Event
            {
                EventName = "Music Concert",
                EventDate = System.DateTime.Now,
                Venue = "Concert Hall",
                TotalSeats = 100,
                AvailableSeats = 100
            };

            // Act
            var result = _controller.AddEvent(newEvent) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result, "Expected result to be not null.");
            Assert.AreEqual(200, result.StatusCode);

            var response = result.Value as Newtonsoft.Json.Linq.JObject; // Ensure Newtonsoft.Json is used
            Assert.IsNotNull(response, "Expected response to be not null.");
            Assert.AreEqual("Event added successfully.", response["message"].ToString());
        }



        [Test]
        public void GetEvents_ShouldReturnAllEvents()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { EventId = 1, EventName = "Event 1", Venue = "Venue 1" },
                new Event { EventId = 2, EventName = "Event 2", Venue = "Venue 2" }
            };

            _context.Events.AddRange(events);
            _context.SaveChanges();

            // Act
            var result = _controller.GetEvents() as OkObjectResult;
            var returnEvents = result.Value as List<Event>;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(2, returnEvents.Count);
        }

        [Test]
        public void BookTickets_ShouldReturnBadRequest_WhenNotEnoughSeats()
        {
            // Arrange
            var evnt = new Event
            {
                EventId = 1,
                EventName = "Test Event",
                AvailableSeats = 5
            };

            var request = new Tickets
            {
                EventID = 1,
                UserName = "John Doe",
                NumberOfTickets = 10
            };

            _context.Events.Add(evnt);
            _context.SaveChanges();

            // Act
            var result = _controller.BookTickets(request);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Not enough seats available.", (badRequestResult.Value as dynamic).message);
        }
    }
}
