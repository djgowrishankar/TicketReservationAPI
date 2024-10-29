using Microsoft.AspNetCore.Mvc;
using TicketReservationAPI.Services;
using TicketReservationAPI.Models;

namespace TicketReservationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly EventService _eventService;
        private readonly BookingService _bookingService;

        public EventController(EventService eventService, BookingService bookingService)
        {
            _eventService = eventService;
            _bookingService = bookingService;
        }

        [HttpPost("addevent")]
        public IActionResult AddEvent([FromBody] Event newEvent)
        {
            _eventService.AddEvent(newEvent);
            return Ok(new { message = "Event added successfully." });
        }

        [HttpGet("viewevents")]
        public IActionResult GetEvents()
        {
            var events = _eventService.GetAllEvents();
            return Ok(events);
        }

        [HttpPut("editevent/{id}")]
        public IActionResult EditEvent(int id, [FromBody] Event updatedEvent)
        {
            var result = _eventService.EditEvent(id, updatedEvent);
            if (!result) return NotFound("Event not found.");
            return Ok(new { message = "Event updated successfully." });
        }

        [HttpDelete("deleteevent/{id}")]
        public IActionResult DeleteEvent(int id)
        {
            var result = _eventService.DeleteEvent(id);
            if (!result) return NotFound("Event not found.");
            return Ok(new { message = "Event deleted successfully." });
        }

        [HttpPost("booktickets")]
        public IActionResult BookTickets([FromBody] Tickets request)
        {
            var booking = _bookingService.BookTickets(request);
            if (booking == null)
                return BadRequest(new { message = "Booking failed. Check event availability." });

            return Ok(new { message = "Tickets booked successfully.", booking });
        }

        [HttpDelete("cancelbooking/{id}")]
        public IActionResult CancelBooking(int id)
        {
            var result = _bookingService.CancelBooking(id);
            if (!result) return NotFound("Booking not found.");

            return Ok(new { message = "Booking cancelled successfully." });
        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> GetBookingsForUser(string userName)
        {
            var bookings = await _bookingService.GetBookingsForUser(userName);
            return Ok(bookings);
        }
    }
}
