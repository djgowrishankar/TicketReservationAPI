using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using TicketReservationAPI.Models;
using TicketReservationAPI.Services;
using TicketReservationAPI.Data;

namespace TicketReservationAPI.Tests
{
    public class BookingServiceTests
    {
        private TicketReservationContext _context;
        private BookingService _bookingService;

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TicketReservationContext>()
                .UseInMemoryDatabase(databaseName: "TicketReservationTestDb")
                .Options;

            _context = new TicketReservationContext(options);
            _bookingService = new BookingService(_context);

            // Add a sample event for booking tests
            _context.Events.Add(new Event
            {
                EventId = 1,
                EventName = "Sample Event",
                EventDate = DateTime.Now.AddDays(1),
                Venue = "Sample Venue",
                TotalSeats = 100,
                AvailableSeats = 100
            });
            _context.SaveChanges();
        }

        [Test]
        public void BookTickets_ShouldBookTickets_WhenSeatsAvailable()
        {
            // Arrange
            var ticketsRequest = new Tickets
            {
                EventID = 1,
                UserName = "TestUser",
                NumberOfTickets = 5
            };

            // Act
            var booking = _bookingService.BookTickets(ticketsRequest);

            // Assert
            var savedBooking = _context.Bookings.FirstOrDefault(b => b.BookingId == booking.BookingId);
            Assert.NotNull(savedBooking);
            Assert.AreEqual("TestUser", savedBooking.UserName);
            Assert.AreEqual(5, savedBooking.NumberOfTickets);

            // Verify seat count reduction
            var evnt = _context.Events.FirstOrDefault(e => e.EventId == 1);
            Assert.AreEqual(95, evnt.AvailableSeats);
        }

        
    }
}
