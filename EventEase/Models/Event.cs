/*
 * Justin Fussell ST10280758 Group 3
 */
namespace EventEase.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public required string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public int? VenueId { get; set; } // Nullable until assigned
        public Venue? Venue { get; set; } // Navigation property
    }
}
//*******************************************************END OF FILE*****************************************************************