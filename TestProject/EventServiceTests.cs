using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketReservationAPI.Data;
using TicketReservationAPI.Models;
using TicketReservationAPI.Services;

namespace TestProject
{
    public class EventServiceTests
    {
        private Mock<TicketReservationContext> _mockContext;
        private EventService _eventService;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<TicketReservationContext>();

            // Create a mock DbSet for Events
            var mockEvents = new List<Event>
            {
                new Event { EventId = 1, EventName = "Concert", EventDate = DateTime.Now.AddDays(10), Venue = "Stadium", TotalSeats = 100, AvailableSeats = 100 },
                new Event { EventId = 2, EventName = "Play", EventDate = DateTime.Now.AddDays(20), Venue = "Theater", TotalSeats = 50, AvailableSeats = 50 }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Event>>();
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(mockEvents.Provider);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(mockEvents.Expression);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(mockEvents.ElementType);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(mockEvents.GetEnumerator());

            // Set up the Events property
            _mockContext.Setup(c => c.Events).Returns(mockDbSet.Object);

            // Create instance of EventService
            _eventService = new EventService(_mockContext.Object);
        }

        [Test]
        public async Task AddEvent_ShouldAddEvent()
        {
            // Arrange
            var newEvent = new Event { EventId = 3, EventName = "Festival", EventDate = DateTime.Now.AddDays(30), Venue = "Park", TotalSeats = 200, AvailableSeats = 200 };

            // Act
            await _eventService.AddEventAsync(newEvent);

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
