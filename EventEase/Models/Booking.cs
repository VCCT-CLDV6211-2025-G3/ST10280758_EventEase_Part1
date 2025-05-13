using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EventEase.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Event is required.")]
        public int EventId { get; set; }
        public Event? Event { get; set; }

        [Required(ErrorMessage = "User is required.")]
        public string UserId { get; set; } = string.Empty;
        public IdentityUser? User { get; set; }

        [Required(ErrorMessage = "Booking date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime BookingDate { get; set; }
    }
}