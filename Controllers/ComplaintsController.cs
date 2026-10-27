using CollegeComplaintManagement.Data;
using CollegeComplaintManagement.Models;
using CollegeComplaintManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeComplaintManagement.Controllers
{
    // Server-side protection: only logged-in users in the Student role get in
    [Authorize(Roles = AppRoles.Student)]
    public class ComplaintsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ComplaintsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ---------- DASHBOARD ----------

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Every query filters by the logged-in student's id
            var mine = _context.Complaints.AsNoTracking().Where(c => c.UserId == user.Id);

            var model = new StudentDashboardViewModel
            {
                FullName = user.FullName,
                Total = await mine.CountAsync(),
                Pending = await mine.CountAsync(c => c.Status == ComplaintStatus.Pending),
                InProgress = await mine.CountAsync(c => c.Status == ComplaintStatus.InProgress),
                Resolved = await mine.CountAsync(c => c.Status == ComplaintStatus.Resolved),
                Recent = await mine.OrderByDescending(c => c.CreatedAt).Take(5).ToListAsync()
            };

            return View(model);
        }

        // ---------- SUBMIT COMPLAINT ----------

        [HttpGet]
        public IActionResult Create() => View(new CreateComplaintViewModel());

        [HttpPost]
        public async Task<IActionResult> Create(CreateComplaintViewModel model)
        {
            // Category must be one of the allowed values (server-side check)
            if (!ComplaintCategories.All.Contains(model.Category))
                ModelState.AddModelError(nameof(model.Category), "Please select a valid category.");

            if (!ModelState.IsValid)
                return View(model);

            var userId = _userManager.GetUserId(User)!;
            var now = DateTime.UtcNow;

            var complaint = new Complaint
            {
                TrackingNumber = await GenerateTrackingNumberAsync(),
                UserId = userId,                       // taken from the login, never from the form
                Category = model.Category,
                Title = model.Title.Trim(),
                Description = model.Description.Trim(),
                Status = ComplaintStatus.Pending,      // always starts as Pending
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Complaint submitted successfully! Your tracking number is {complaint.TrackingNumber}.";
            return RedirectToAction(nameof(Details), new { id = complaint.ComplaintId });
        }

        // ---------- MY COMPLAINTS ----------

        public async Task<IActionResult> MyComplaints(string? search, ComplaintStatus? status)
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.Complaints.AsNoTracking().Where(c => c.UserId == userId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(c => c.Title.Contains(term) || c.TrackingNumber.Contains(term));
            }

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            var model = new MyComplaintsViewModel
            {
                Complaints = await query.OrderByDescending(c => c.CreatedAt).ToListAsync(),
                Search = search,
                Status = status
            };

            return View(model);
        }

        // ---------- COMPLAINT DETAILS ----------

        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);

            // Id AND owner must match. Another student's complaint returns 404,
            // so changing the number in the URL reveals nothing.
            var complaint = await _context.Complaints
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ComplaintId == id && c.UserId == userId);

            if (complaint == null)
                return NotFound();

            return View(complaint);
        }

        // ---------- PROFILE ----------

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Department = user.Department,
                RegisteredAt = user.RegisteredAt,
                TotalComplaints = await _context.Complaints.CountAsync(c => c.UserId == user.Id)
            };

            return View(model);
        }

        // ---------- HELPER ----------

        // Format: CMP-20261023-A1B2C3 (19 characters, fits the column).
        // The database also has a unique index as a final safety net.
        private async Task<string> GenerateTrackingNumberAsync()
        {
            for (var attempt = 0; attempt < 5; attempt++)
            {
                var suffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
                var candidate = $"CMP-{DateTime.UtcNow:yyyyMMdd}-{suffix}";

                if (!await _context.Complaints.AnyAsync(c => c.TrackingNumber == candidate))
                    return candidate;
            }

            throw new InvalidOperationException("Could not generate a unique tracking number.");
        }
    }
}
