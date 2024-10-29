using System.Collections.Generic;
using TicketReservationAPI.Models;

namespace TicketReservationAPI.Services
{
    public interface IEventService
    {
        Event AddEvent(Event newEvent);
        IEnumerable<Event> GetEvents();
        Booking BookTickets(Booking request);
        bool CancelBooking(Tickets ticketCancel);
        Booking GetBookingDetails(string bookingReference);
    }
}
