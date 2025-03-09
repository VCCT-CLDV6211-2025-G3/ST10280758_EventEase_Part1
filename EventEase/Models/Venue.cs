/*
 * Justin Fussell ST10280758 Group 3
 */
namespace EventEase.Models
{
    public class Venue
    {
        public int VenueId { get; set; }
        public required string VenueName { get; set; }
        public required string Location { get; set; }
        public int Capacity { get; set; }
        public string? ImageUrl { get; set; } // Nullable for placeholder URLs
    }
}
//*******************************************************END OF FILE*****************************************************************