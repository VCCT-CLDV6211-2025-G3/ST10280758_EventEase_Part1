using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase
{
    public class EventEaseDbContext : IdentityDbContext
    {
        public EventEaseDbContext(DbContextOptions<EventEaseDbContext> options)
            : base(options)
        {
        }

        public DbSet<Venue> Venue { get; set; } = null!;
        public DbSet<Event> Event { get; set; } = null!;
        public DbSet<Booking> Booking { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Venue>()
                .HasMany(v => v.Events)
                .WithOne(e => e.Venue)
                .HasForeignKey(e => e.VenueId);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Bookings)
                .WithOne(b => b.Event)
                .HasForeignKey(b => b.EventId);

            // Configure the UserId foreign key to IdentityUser
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
        }
    }
}