using NUnit.Framework;
using Moq;
using TicketReservationAPI.Controllers;
using TicketReservationAPI.Data;
using TicketReservationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TicketReservationAPI.Tests
{
    [TestFixture]
    public class EventControllerTests
    {
        private Mock<TicketReservationContext> _mockContext;
        private EventController _controller;
        private Mock<DbSet<Event>> _mockEventDbSet;
        private Mock<DbSet<Booking>> _mockBookingDbSet;
        private List<Event> _eventList;
        private List<Booking> _bookingList;

        [SetUp]
        public void Setup()
        {
            // Set up mock data for Events
            _eventList = new List<Event>
            {
                new Event { EventId = 1, EventName = "Concert", Venue = "Hall A", TotalSeats = 100, AvailableSeats = 50 },
                new Event { EventId = 2, EventName = "Theater Play", Venue = "Theater B", TotalSeats = 200, AvailableSeats = 150 }
            };

            // Set up mock data for Bookings
            _bookingList = new List<Booking>
            {
                new Booking { BookingId = 1, EventId = 1, UserName = "JohnDoe", NumberOfTickets = 10, BookingDate = DateTime.Now }
            };

            // Mocking DbSet for Events
            _mockEventDbSet = MockDbSet(_eventList);
            _mockBookingDbSet = MockDbSet(_bookingList);

            // Mock DbContext
            _mockContext = new Mock<TicketReservationContext>();
            _mockContext.Setup(c => c.Events).Returns(_mockEventDbSet.Object);
            _mockContext.Setup(c => c.Bookings).Returns(_mockBookingDbSet.Object);

            // Simulate SaveChanges to ensure it works correctly
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Initialize the controller with the mocked context
            _controller = new EventController(_mockContext.Object);
        }

        [Test]
        public void GetEvents_ReturnsOkResult_WithEventsList()
        {
            // Act
            var result = _controller.GetEvents() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
            var events = result.Value as List<Event>;
            Assert.AreEqual(2, events.Count);
        }

        [Test]
        public void AddEvent_ValidEvent_ReturnsOkResult()
        {
            // Arrange
            var newEvent = new Event { EventId = 3, EventName = "Conference", Venue = "Room C", TotalSeats = 150, AvailableSeats = 150 };

            // Act
            var result = _controller.AddEvent(newEvent) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Event added successfully.", (result.Value as dynamic).message);

            // Verify that the event was added to the context
            _mockEventDbSet.Verify(m => m.Add(It.IsAny<Event>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Test]
        public void EditEvent_ExistingEvent_ReturnsOkResult()
        {
            // Arrange
            var updatedEvent = new Event
            {
                EventId = 1,
                EventName = "Updated Concert",
                Venue = "Hall B",
                TotalSeats = 100,
                AvailableSeats = 50
            };

            // Act
            var result = _controller.EditEvent(1, updatedEvent) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Event updated successfully.", (result.Value as dynamic).message);

            // Verify that the event was updated
            var eventInDb = _eventList.FirstOrDefault(e => e.EventId == 1);
            Assert.AreEqual("Updated Concert", eventInDb.EventName);
            Assert.AreEqual("Hall B", eventInDb.Venue);

            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Test]
        public void DeleteEvent_ExistingEvent_ReturnsOkResult()
        {
            // Act
            var result = _controller.DeleteEvent(1) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Event deleted successfully.", (result.Value as dynamic).message);

            // Verify that the event was removed from the list
            var eventInDb = _eventList.FirstOrDefault(e => e.EventId == 1);
            Assert.IsNull(eventInDb);

            _mockEventDbSet.Verify(m => m.Remove(It.IsAny<Event>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Test]
        public void BookTickets_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var ticketRequest = new Tickets
            {
                EventID = 1,
                UserName = "JohnDoe",
                NumberOfTickets = 10
            };

            // Act
            var result = _controller.BookTickets(ticketRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ActionResult<Booking>>(result);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("Tickets booked successfully.", (okResult.Value as dynamic).message);

            // Verify available seats were reduced
            var eventInDb = _eventList.FirstOrDefault(e => e.EventId == 1);
            Assert.AreEqual(40, eventInDb.AvailableSeats);

            // Verify booking was created
            _mockBookingDbSet.Verify(m => m.Add(It.IsAny<Booking>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Test]
        public void CancelBooking_ExistingBooking_ReturnsOkResult()
        {
            // Act
            var result = _controller.CancelBooking(1) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Booking cancelled successfully.", (result.Value as dynamic).message);

            // Verify that the available seats were restored
            var eventInDb = _eventList.FirstOrDefault(e => e.EventId == 1);
            Assert.AreEqual(50, eventInDb.AvailableSeats);

            // Verify that the booking was deleted
            _mockBookingDbSet.Verify(m => m.Remove(It.IsAny<Booking>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        // Helper method to mock DbSet for tests
        private static Mock<DbSet<T>> MockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));
            dbSet.Setup(d => d.Remove(It.IsAny<T>())).Callback<T>((s) => sourceList.Remove(s));
            return dbSet;
        }
    }
}
