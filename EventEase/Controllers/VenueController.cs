/*
 * Justin Fussell ST10280758 Group 3
 */
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class VenueController : Controller
    {
        private readonly EventEaseDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public VenueController(EventEaseDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: /Venue
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venue.ToListAsync());
        }

        // GET: /Venue/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Venue/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueName,Location,Capacity,ImageUrl")] Venue venue, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image upload
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // Validate file type
                        if (!ImageFile.ContentType.StartsWith("image/"))
                        {
                            ModelState.AddModelError("ImageFile", "Please upload a valid image file.");
                            return View(venue);
                        }

                        // Define the path to save the image
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Generate a unique file name
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Save the file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        // Store the relative path in ImageUrl
                        venue.ImageUrl = $"/images/{uniqueFileName}";
                    }
                    else
                    {
                        venue.ImageUrl = "/images/default.jpg"; // Default image if none uploaded
                    }

                    // Save to database
                    _context.Add(venue);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the venue. Please try again or contact support.");

                }
                catch (IOException)
                {
                    ModelState.AddModelError("ImageFile", "An error occurred while uploading the image. Please try again.");

                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");

                }
            }

            // If ModelState is invalid or an exception occurred, return to the view with errors
            return View(venue);
        }

        // GET: /Venue/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var venue = await _context.Venue.FindAsync(id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        // POST: /Venue/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue, IFormFile ImageFile)
        {
            if (id != venue.VenueId) return NotFound();
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Validate file type
                    if (!ImageFile.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("ImageFile", "Please upload a valid image file.");
                        return View(venue);
                    }

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(venue.ImageUrl) && System.IO.File.Exists(Path.Combine(_hostingEnvironment.WebRootPath, venue.ImageUrl.TrimStart('/'))))
                    {
                        System.IO.File.Delete(Path.Combine(_hostingEnvironment.WebRootPath, venue.ImageUrl.TrimStart('/')));
                    }

                    // Save new image
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    venue.ImageUrl = $"/images/{uniqueFileName}";
                }

                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the venue. Please try again or contact support.");
                    // Log the exception if logging is set up
                    // _logger.LogError(ex, "Error updating venue");
                }
                catch (IOException)
                {
                    ModelState.AddModelError("ImageFile", "An error occurred while uploading the image. Please try again.");
                    // Log the exception if logging is set up
                    // _logger.LogError(ex, "Error uploading image");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                    // Log the exception if logging is set up
                    // _logger.LogError(ex, "Unexpected error");
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(venue);
        }

        // GET: /Venue/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        // GET: /Venue/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        // POST: /Venue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venue.FindAsync(id);
            if (venue != null)
            {
                // Delete the image file if it exists
                if (!string.IsNullOrEmpty(venue.ImageUrl) && System.IO.File.Exists(Path.Combine(_hostingEnvironment.WebRootPath, venue.ImageUrl.TrimStart('/'))))
                {
                    System.IO.File.Delete(Path.Combine(_hostingEnvironment.WebRootPath, venue.ImageUrl.TrimStart('/')));
                }

                _context.Venue.Remove(venue);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueId == id);
        }
    }
}