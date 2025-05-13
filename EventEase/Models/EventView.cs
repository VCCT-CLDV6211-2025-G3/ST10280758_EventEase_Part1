using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // For IFormFile

namespace EventEase.Models
{
    public class EventViewModel
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(100, ErrorMessage = "Event name cannot exceed 100 characters.")]
        public string? EventName { get; set; }

        [Required(ErrorMessage = "Event start date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Event end date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Venue is required.")]
        public int VenueId { get; set; }

        public string? ImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}