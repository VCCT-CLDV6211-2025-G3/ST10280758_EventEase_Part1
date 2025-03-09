/*
 * Justin Fussell ST10280758 Group 3
 */
namespace EventEase.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int EventId { get; set; }
        public int VenueId { get; set; }
        public DateTime BookingDate { get; set; }
        public required Event Event { get; set; } // Navigation property
        public required Venue Venue { get; set; } // Navigation property
    }
}
//*******************************************************END OF FILE*****************************************************************