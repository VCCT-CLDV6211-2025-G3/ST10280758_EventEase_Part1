/*
 * Justin Fussell ST10280758 Group 3
 */
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly EventEaseDbContext _context;

        public BookingsController(EventEaseDbContext context)
        {
            _context = context;
        }

        // GET: /Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .ToListAsync();
            return View(bookings);
        }

        // GET: /Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null) return NotFound();

            return View(booking);
        }

        // GET: /Bookings/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Events = await _context.Events.ToListAsync();
            ViewBag.Venues = await _context.Venues.ToListAsync();
            return View();
        }

        // POST: /Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,VenueId,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                var eventDetails = await _context.Events.FindAsync(booking.EventId);
                if (eventDetails == null) return NotFound();

                // Check for double bookings
                var conflict = await _context.Bookings
                    .Where(b => b.VenueId == booking.VenueId && b.EventId != booking.EventId)
                    .AnyAsync(b => eventDetails.EventDate < _context.Events
                        .Where(e => e.EventId == b.EventId)
                        .Select(e => e.EndDate).First()
                        && eventDetails.EndDate > _context.Events
                        .Where(e => e.EventId == b.EventId)
                        .Select(e => e.EventDate).First());
                if (conflict)
                {
                    ModelState.AddModelError("", "This venue is already booked for that time.");
                }
                else
                {
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.Events = await _context.Events.ToListAsync();
            ViewBag.Venues = await _context.Venues.ToListAsync();
            return View(booking);
        }

        // GET: /Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            ViewBag.Events = await _context.Events.ToListAsync();
            ViewBag.Venues = await _context.Venues.ToListAsync();
            return View(booking);
        }

        // POST: /Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,VenueId,BookingDate")] Booking booking)
        {
            if (id != booking.BookingId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Events = await _context.Events.ToListAsync();
            ViewBag.Venues = await _context.Venues.ToListAsync();
            return View(booking);
        }

        // GET: /Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: /Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(b => b.BookingId == id);
        }
    }
}
//*******************************************************END OF FILE*****************************************************************