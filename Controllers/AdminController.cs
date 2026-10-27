using CollegeComplaintManagement.Data;
using CollegeComplaintManagement.Models;
using CollegeComplaintManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeComplaintManagement.Controllers
{
    // Every action in this controller requires the Admin role (checked on the server)
    [Authorize(Roles = AppRoles.Admin)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------- DASHBOARD ----------

        public async Task<IActionResult> Dashboard(string? search, ComplaintStatus? status, string? category)
        {
            var all = _context.Complaints.AsNoTracking();

            var filtered = ApplyFilters(all.Include(c => c.User), search, status, category);

            var model = new AdminDashboardViewModel
            {
                // All statistics are calculated from the database
                TotalStudents = await StudentsQuery().CountAsync(),
                Total = await all.CountAsync(),
                Pending = await all.CountAsync(c => c.Status == ComplaintStatus.Pending),
                InProgress = await all.CountAsync(c => c.Status == ComplaintStatus.InProgress),
                Resolved = await all.CountAsync(c => c.Status == ComplaintStatus.Resolved),
                Recent = await filtered.OrderByDescending(c => c.CreatedAt).Take(10).ToListAsync(),
                Search = search,
                Status = status,
                Category = category
            };

            return View(model);
        }

        // ---------- MANAGE COMPLAINTS ----------

        public async Task<IActionResult> ManageComplaints(string? search, ComplaintStatus? status, string? category)
        {
            var query = ApplyFilters(
                _context.Complaints.AsNoTracking().Include(c => c.User),
                search, status, category);

            var model = new ManageComplaintsViewModel
            {
                Complaints = await query.OrderByDescending(c => c.CreatedAt).ToListAsync(),
                Search = search,
                Status = status,
                Category = category
            };

            return View(model);
        }

        // ---------- COMPLAINT DETAILS (+ update form) ----------

        public async Task<IActionResult> Details(int id)
        {
            var complaint = await _context.Complaints
                .AsNoTracking()
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.ComplaintId == id);

            if (complaint == null)
                return NotFound();

            return View(new AdminComplaintDetailsViewModel
            {
                Complaint = complaint,
                ComplaintId = complaint.ComplaintId,
                Status = complaint.Status,
                AdminRemarks = complaint.AdminRemarks
            });
        }

        // ---------- UPDATE STATUS + REMARKS ----------

        [HttpPost]
        public async Task<IActionResult> UpdateComplaint(
            [Bind("ComplaintId,Status,AdminRemarks")] AdminComplaintDetailsViewModel model)
        {
            var complaint = await _context.Complaints
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.ComplaintId == model.ComplaintId);

            if (complaint == null)
                return NotFound();

            if (!Enum.IsDefined(model.Status))
                ModelState.AddModelError(nameof(model.Status), "Please select a valid status.");

            if (!ModelState.IsValid)
            {
                model.Complaint = complaint;
                return View("Details", model);
            }

            complaint.Status = model.Status;
            complaint.AdminRemarks = string.IsNullOrWhiteSpace(model.AdminRemarks)
                ? null
                : model.AdminRemarks.Trim();
            complaint.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Complaint {complaint.TrackingNumber} was updated successfully.";
            return RedirectToAction(nameof(Details), new { id = complaint.ComplaintId });
        }

        // ---------- DELETE (POST only, confirmed in a modal) ----------

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null)
            {
                TempData["Error"] = "Complaint not found. It may have already been deleted.";
                return RedirectToAction(nameof(ManageComplaints));
            }

            _context.Complaints.Remove(complaint);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Complaint {complaint.TrackingNumber} was deleted.";
            return RedirectToAction(nameof(ManageComplaints));
        }

        // ---------- MANAGE STUDENTS ----------

        public async Task<IActionResult> ManageStudents(string? search)
        {
            var query = StudentsQuery().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(u => u.FullName.Contains(term)
                                      || (u.Email != null && u.Email.Contains(term))
                                      || u.Department.Contains(term));
            }

            // Select only safe columns: password hashes are never loaded
            var students = await query
                .OrderBy(u => u.FullName)
                .Select(u => new StudentRowViewModel
                {
                    FullName = u.FullName,
                    Email = u.Email ?? string.Empty,
                    Department = u.Department,
                    RegisteredAt = u.RegisteredAt,
                    ComplaintCount = u.Complaints.Count
                })
                .ToListAsync();

            return View(new ManageStudentsViewModel { Students = students, Search = search });
        }

        // ---------- HELPERS ----------

        private static IQueryable<Complaint> ApplyFilters(
            IQueryable<Complaint> query, string? search, ComplaintStatus? status, string? category)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(c => c.TrackingNumber.Contains(term) || c.Title.Contains(term));
            }

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(c => c.Category == category);

            return query;
        }

        // Users who belong to the Student role (the Admin is excluded)
        private IQueryable<ApplicationUser> StudentsQuery()
        {
            var studentRoleId = _context.Roles
                .Where(r => r.Name == AppRoles.Student)
                .Select(r => r.Id);

            return _context.Users.Where(u =>
                _context.UserRoles.Any(ur => ur.UserId == u.Id && studentRoleId.Contains(ur.RoleId)));
        }
    }
}
