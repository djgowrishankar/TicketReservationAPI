using Moq;
using NUnit.Framework;
using TicketReservationAPI.Controllers;
using TicketReservationAPI.Models;
using TicketReservationAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace TicketReservationAPITests
{
    [TestFixture]
    public class EventControllerTests
    {
        private Mock<IEventService> _mockEventService;
        private EventController _eventController;

        [SetUp]
        public void Setup()
        {
            _mockEventService = new Mock<IEventService>();
            _eventController = new EventController(_mockEventService.Object);
        }

        [Test]
        public void AddEvent_ShouldReturnOkResult_WhenEventIsValid()
        {
            // Arrange
            var newEvent = new Event
            {
                Name = "Concert",
                Date = DateTime.Now,
                Venue = "City Hall",
                TotalSeats = 150,
                AvailableSeats = 100
            };

            _mockEventService.Setup(service => service.AddEvent(newEvent)).Returns(newEvent);

            // Act
            var result = _eventController.AddEvent(newEvent) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as Event;
            Assert.IsNotNull(response);
            Assert.AreEqual("Concert", response.Name);
            Assert.AreEqual(100, response.AvailableSeats);
            Assert.AreEqual("City Hall", response.Venue);
            Assert.AreEqual(150, response.TotalSeats);
        }

        [Test]
        public void GetEvents_ShouldReturnAllEvents()
        {
            // Arrange
            var eventsList = new List<Event>
            {
                new Event { Name = "Concert", AvailableSeats = 100, TotalSeats = 150, Date = DateTime.Now, Venue = "City Hall" },
                new Event { Name = "Play", AvailableSeats = 50, TotalSeats = 100, Date = DateTime.Now.AddDays(1), Venue = "Community Theater" }
            };

            _mockEventService.Setup(service => service.GetEvents()).Returns(eventsList);

            // Act
            var result = _eventController.GetEvents() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as List<Event>;
            Assert.IsNotNull(response);
            Assert.AreEqual(2, response.Count);
        }

        [Test]
        public void BookTickets_ShouldReturnBooking_WhenRequestIsValid()
        {
            // Arrange
            var newEvent = new Event { Name = "Concert", AvailableSeats = 100, TotalSeats = 150, Date = DateTime.Now, Venue = "City Hall" };
            var request = new BookingRequest { EventName = "Concert", UserName = "JohnDoe", Tickets = 2 };
            var booking = new Booking { Name = "Concert", UserName = "JohnDoe", Tickets = 2, BookingReference = Guid.NewGuid().ToString() };

            _mockEventService.Setup(service => service.BookTickets(request)).Returns(booking);

            // Act
            var result = _eventController.BookTickets(request) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as Booking;
            Assert.IsNotNull(response);
            Assert.AreEqual("Concert", response.Name);
            Assert.AreEqual("JohnDoe", response.UserName);
            Assert.AreEqual(2, response.Tickets);
        }

        [Test]
        public void CancelBooking_ShouldReturnTrue_WhenBookingReferenceIsValid()
        {
            // Arrange
            var bookingReference = Guid.NewGuid().ToString();
            var ticketCancel = new TicketCancel { Referrence = bookingReference };

            _mockEventService.Setup(service => service.CancelBooking(ticketCancel)).Returns(true);

            // Act
            var result = _eventController.CancelBooking(ticketCancel) as OkResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void GetBookingDetails_ShouldReturnBooking_WhenReferenceIsValid()
        {
            // Arrange
            var bookingReference = Guid.NewGuid().ToString();
            var booking = new Booking { Name = "Concert", UserName = "JohnDoe", Tickets = 2, BookingReference = bookingReference };

            _mockEventService.Setup(service => service.GetBookingDetails(bookingReference)).Returns(booking);

            // Act
            var result = _eventController.GetBookingDetails(bookingReference) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as Booking;
            Assert.IsNotNull(response);
            Assert.AreEqual("Concert", response.Name);
            Assert.AreEqual("JohnDoe", response.UserName);
            Assert.AreEqual(2, response.Tickets);
        }
    }
}
