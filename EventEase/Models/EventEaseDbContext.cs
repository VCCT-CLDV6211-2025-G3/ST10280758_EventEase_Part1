/*
 * Justin Fussell ST10280758 Group 3
 */
using Microsoft.EntityFrameworkCore;

namespace EventEase.Models
{
    public class EventEaseDbContext(DbContextOptions<EventEaseDbContext> options) : DbContext(options)
    {
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}
//*******************************************************END OF FILE*****************************************************************