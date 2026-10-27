using CollegeComplaintManagement.Models;

namespace CollegeComplaintManagement.ViewModels
{
    public class ComplaintFilterViewModel
    {
        public string? Search { get; set; }
        public ComplaintStatus? Status { get; set; }
        public string? Category { get; set; }

        public bool HasFilters =>
            !string.IsNullOrWhiteSpace(Search) || Status.HasValue || !string.IsNullOrWhiteSpace(Category);
    }
}
