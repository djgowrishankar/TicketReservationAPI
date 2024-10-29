using System.ComponentModel.DataAnnotations.Schema;

namespace TicketReservationAPI.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int EventId { get; set; }
        public string UserName { get; set; }
        public int NumberOfTickets { get; set; }
        public string BookingReference { get; set; }
        public DateTime BookingDate { get; set; }
        [NotMapped]
        public string EventName { get; set; }
    }

    public class Tickets
    {
        public int EventID { get; set; }
        public string UserName { get; set; }
        public int NumberOfTickets { get; set; }
    }
}
