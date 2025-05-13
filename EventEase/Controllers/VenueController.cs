using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventEase.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;

namespace EventEase.Controllers
{
    public class VenueController : Controller
    {
        private readonly EventEaseDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<VenueController> _logger;

        public VenueController(EventEaseDbContext context, IWebHostEnvironment environment, IConfiguration configuration, ILogger<VenueController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Venue
        public async Task<IActionResult> Index()
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            var venues = await _context.Venue.ToListAsync();
            return View(venues);
        }

        // GET: Venue/Details/5
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

            var venue = await _context.Venue
                .Include(v => v.Events)
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venue/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new VenueViewModel());
        }

        // POST: Venue/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VenueViewModel model)
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

                var venue = new Venue
                {
                    VenueName = model.VenueName ?? string.Empty,
                    Location = model.Location ?? string.Empty,
                    Capacity = model.Capacity,
                    ImageUrl = imageUrl
                };

                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Venue/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }

            var viewModel = new VenueViewModel
            {
                VenueId = venue.VenueId,
                VenueName = venue.VenueName,
                Location = venue.Location,
                Capacity = venue.Capacity,
                ImageUrl = venue.ImageUrl
            };
            return View(viewModel);
        }

        // POST: Venue/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VenueViewModel model)
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            if (id != model.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var venue = await _context.Venue.FindAsync(id);
                if (venue == null)
                {
                    return NotFound();
                }

                venue.VenueName = model.VenueName ?? string.Empty;
                venue.Location = model.Location ?? string.Empty;
                venue.Capacity = model.Capacity;

                if (model.ImageFile != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    var blobClient = new BlobClient(_configuration.GetConnectionString("AzureBlobStorage"), "images", fileName);
                    using (var stream = model.ImageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }
                    venue.ImageUrl = model.ImageUrl;
                }

                _context.Update(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Venue/Delete/5
        [Authorize(Roles = "Admin")]
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

            var venue = await _context.Venue
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venue/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context == null)
            {
                return StatusCode(500, "Database context is not initialized.");
            }

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }

            // Check if there are any events associated with this venue
            bool hasEvents = await _context.Event.AnyAsync(e => e.VenueId == id);
            if (hasEvents)
            {
                ModelState.AddModelError("", "Cannot delete this venue because it has associated events.");
                return View(venue); // Return to the Delete confirmation view with the error
            }

            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Venue deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueId == id);
        }
    }
}