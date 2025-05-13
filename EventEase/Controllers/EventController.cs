using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Azure.Storage.Blobs;

namespace EventEase.Controllers
{
    public class EventController : Controller
    {
        private readonly EventEaseDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EventController> _logger;

        public EventController(EventEaseDbContext context, IWebHostEnvironment environment, IConfiguration configuration, ILogger<EventController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            var events = await _context.Event
                .Include(e => e.Venue)
                .ToListAsync();
            return View(events);
        }

        public IActionResult Create()
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            ViewBag.VenueList = new SelectList(_context.Venue, "VenueId", "VenueName");
            return View(new EventViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventViewModel model)
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            if (ModelState.IsValid)
            {
                string? imageUrl = null;
                if (model.ImageFile != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    var blobClient = new BlobClient(_configuration.GetConnectionString("AzureBlobStorage"), "images", fileName);
                    using (var stream = model.ImageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }
                    imageUrl = blobClient.Uri.ToString();
                }

                var @event = new Event
                {
                    EventName = model.EventName ?? string.Empty,
                    EventDate = model.EventDate,
                    EndDate = model.EndDate,
                    VenueId = model.VenueId,
                    ImageUrl = imageUrl
                };

                _context.Add(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.VenueList = new SelectList(_context.Venue, "VenueId", "VenueName", model.VenueId);
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            var viewModel = new EventViewModel
            {
                EventId = @event.EventId,
                EventName = @event.EventName,
                EventDate = @event.EventDate,
                EndDate = @event.EndDate,
                VenueId = @event.VenueId,
                ImageUrl = @event.ImageUrl
            };
            ViewBag.VenueList = new SelectList(_context.Venue, "VenueId", "VenueName", @event.VenueId);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventViewModel model)
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            if (id != model.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var @event = await _context.Event.FindAsync(id);
                if (@event == null)
                {
                    return NotFound();
                }

                @event.EventName = model.EventName ?? string.Empty;
                @event.EventDate = model.EventDate;
                @event.EndDate = model.EndDate;
                @event.VenueId = model.VenueId;

                if (model.ImageFile != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    var blobClient = new BlobClient(_configuration.GetConnectionString("AzureBlobStorage"), "images", fileName);
                    using (var stream = model.ImageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }
                    @event.ImageUrl = model.ImageUrl;
                }

                _context.Update(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.VenueList = new SelectList(_context.Venue, "VenueId", "VenueName", model.VenueId);
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            // Check if there are any bookings associated with this event
            bool hasBookings = await _context.Booking.AnyAsync(b => b.EventId == id);
            if (hasBookings)
            {
                ModelState.AddModelError("", "Cannot delete this event because it has associated bookings.");
                return View(@event); // Return to the Delete confirmation view with the error
            }

            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventId == id);
        }

        public async Task<IActionResult> Search(string searchString)
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            ViewData["CurrentFilter"] = searchString;
            var events = string.IsNullOrEmpty(searchString)
                ? await _context.Event.Include(e => e.Venue).ToListAsync()
                : await _context.Event
                    .Include(e => e.Venue)
                    .Where(e => e.EventName.Contains(searchString))
                    .ToListAsync();

            return View(events);
        }
    }
}