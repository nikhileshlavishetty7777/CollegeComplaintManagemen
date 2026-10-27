using CollegeComplaintManagement.Models;

namespace CollegeComplaintManagement.ViewModels
{
    public class MyComplaintsViewModel
    {
        public List<Complaint> Complaints { get; set; } = new();
        public string? Search { get; set; }
        public ComplaintStatus? Status { get; set; }
    }
}
