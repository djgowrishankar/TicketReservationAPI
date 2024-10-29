using NUnit.Framework;
using TicketReservationAPI.Services;
using TicketReservationAPI.Data;
using TicketReservationAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Moq;

namespace TicketReservationAPI.Tests
{
    [TestFixture]
    public class EventServiceTests
    {
        private EventService _eventService;
        private TicketReservationContext _context;

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TicketReservationContext>()
                .UseInMemoryDatabase(databaseName: "TicketReservationTestDB")
                .Options;
            _context = new TicketReservationContext(options);

            _eventService = new EventService(_context);
        }

        [Test]
        public void AddEvent_ShouldAddEvent()
        {
            // Arrange
            var newEvent = new Event
            {
                EventName = "Test Event",
                EventDate = DateTime.Now,
                Venue = "Test Venue",
                TotalSeats = 100,
                AvailableSeats = 100
            };

            // Act
            _eventService.AddEvent(newEvent);

            // Assert
            var addedEvent = _context.Events.Find(newEvent.EventId);
            Assert.NotNull(addedEvent);
            Assert.AreEqual("Test Event", addedEvent.EventName);
        }


        [Test]
        public void AddEvent_ShouldAddEventSuccessfully()
        {
            var newEvent = new Event { EventName = "Test Event", Venue = "Test Venue", TotalSeats = 100, AvailableSeats = 100 };
            _eventService.AddEvent(newEvent);

            var events = _context.Events.ToList();
            Assert.AreEqual(2, events.Count);
        }
        

        [Test]
        public void EditEvent_ShouldUpdateEventDetails()
        {
            var newEvent = new Event { EventName = "Original Event", Venue = "Original Venue", TotalSeats = 100, AvailableSeats = 100 };
            _context.Events.Add(newEvent);
            _context.SaveChanges();

            var updatedEvent = new Event { EventName = "Updated Event", Venue = "Updated Venue", TotalSeats = 150, AvailableSeats = 150 };
            var result = _eventService.EditEvent(newEvent.EventId, updatedEvent);

            Assert.IsTrue(result);
            var updated = _context.Events.Find(newEvent.EventId);
            Assert.AreEqual("Updated Event", updated.EventName);
        }

        [Test]
        public void DeleteEvent_ShouldRemoveEventSuccessfully()
        {
            var newEvent = new Event { EventName = "Test Event", Venue = "Test Venue", TotalSeats = 100, AvailableSeats = 100 };
            _context.Events.Add(newEvent);
            _context.SaveChanges();

            var result = _eventService.DeleteEvent(newEvent.EventId);
            Assert.IsTrue(result);

            var events = _context.Events.ToList();
            Assert.AreEqual(2, events.Count);
        }
    }
}
