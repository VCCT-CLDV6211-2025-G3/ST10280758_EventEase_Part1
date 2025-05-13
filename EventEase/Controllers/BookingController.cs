using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace EventEase.Controllers
{
    public class BookingController : Controller
    {
        private readonly EventEaseDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<BookingController> _logger;

        public BookingController(EventEaseDbContext context, UserManager<IdentityUser> userManager, ILogger<BookingController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            var bookings = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.User)
                .ToListAsync();
            return View(bookings);
        }

        // GET: Booking/Create
        public async Task<IActionResult> Create()
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            // Populate the dropdown list for Events
            ViewBag.EventList = new SelectList(await _context.Event
                .Include(e => e.Venue)
                .ToListAsync(), "EventId", "EventName");

            return View();
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,BookingDate")] Booking booking)
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            if (ModelState.IsValid)
            {
                // Check for double booking
                bool isDoubleBooked = await _context.Booking.AnyAsync(b =>
                    b.EventId == booking.EventId &&
                    b.BookingDate == booking.BookingDate);

                if (isDoubleBooked)
                {
                    ModelState.AddModelError("BookingDate", "This event is already booked for this date and time.");
                    ViewBag.EventList = new SelectList(await _context.Event
                        .Include(e => e.Venue)
                        .ToListAsync(), "EventId", "EventName", booking.EventId);
                    return View(booking);
                }

                // Get the current user's ID
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    booking.UserId = user.Id;
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "User not authenticated.");
                    ViewBag.EventList = new SelectList(await _context.Event
                        .Include(e => e.Venue)
                        .ToListAsync(), "EventId", "EventName", booking.EventId);
                    return View(booking);
                }
            }

            // If validation fails, repopulate the dropdown
            ViewBag.EventList = new SelectList(await _context.Event
                .Include(e => e.Venue)
                .ToListAsync(), "EventId", "EventName", booking.EventId);
            return View(booking);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewBag.EventList = new SelectList(await _context.Event.ToListAsync(), "EventId", "EventName", booking.EventId);
            return View(booking);
        }

        // POST: Booking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,UserId,BookingDate")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.EventList = new SelectList(await _context.Event.ToListAsync(), "EventId", "EventName", booking.EventId);
            return View(booking);
        }

        // GET: Booking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Booking deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return (_context.Booking?.Any(e => e.BookingId == id)).GetValueOrDefault();
        }
    }
}