using TicketReservationAPI.Models;
using TicketReservationAPI.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketReservationAPI.Services
{
    public class EventService
    {
        private readonly TicketReservationContext _context;

        public EventService(TicketReservationContext context)
        {
            _context = context;
        }

        public IEnumerable<Event> GetAllEvents()
        {
            return _context.Events.ToList();
        }

        public Event GetEventById(int id)
        {
            return _context.Events.Find(id);
        }

        public void AddEvent(Event newEvent)
        {
            _context.Events.Add(newEvent);
            _context.SaveChanges();
        }

        public bool EditEvent(int id, Event updatedEvent)
        {
            var existingEvent = _context.Events.Find(id);
            if (existingEvent == null)
                return false;

            existingEvent.EventName = updatedEvent.EventName;
            existingEvent.EventDate = updatedEvent.EventDate;
            existingEvent.Venue = updatedEvent.Venue;
            existingEvent.TotalSeats = updatedEvent.TotalSeats;
            existingEvent.AvailableSeats = updatedEvent.AvailableSeats;

            _context.SaveChanges();
            return true;
        }

        public bool DeleteEvent(int id)
        {
            var existingEvent = _context.Events.Find(id);
            if (existingEvent == null)
                return false;

            _context.Events.Remove(existingEvent);
            _context.SaveChanges();
            return true;
        }
    }
}
