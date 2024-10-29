using TicketReservationAPI.Models;
using TicketReservationAPI.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TicketReservationAPI.Services
{
    public class BookingService
    {
        private readonly TicketReservationContext _context;

        public BookingService(TicketReservationContext context)
        {
            _context = context;
        }

        public Booking BookTickets(Tickets request)
        {
            var evnt = _context.Events.FirstOrDefault(e => e.EventId == request.EventID);

            if (evnt == null || evnt.AvailableSeats < request.NumberOfTickets)
                return null;

            evnt.AvailableSeats -= request.NumberOfTickets;

            var booking = new Booking
            {
                EventId = evnt.EventId,
                EventName = evnt.EventName,
                UserName = request.UserName,
                BookingDate = evnt.EventDate,
                NumberOfTickets = request.NumberOfTickets,
                BookingReference = Guid.NewGuid().ToString()
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return booking;
        }

        public bool CancelBooking(int bookingId)
        {
            var booking = _context.Bookings.Find(bookingId);
            if (booking == null)
                return false;

            var eventDetails = _context.Events.Find(booking.EventId);
            if (eventDetails != null)
                eventDetails.AvailableSeats += booking.NumberOfTickets;

            _context.Bookings.Remove(booking);
            _context.SaveChanges();

            return true;
        }

        public async Task<IQueryable<Booking>> GetBookingsForUser(string userName)
        {
            return _context.Bookings.Where(b => b.UserName == userName);
        }
    }
}
