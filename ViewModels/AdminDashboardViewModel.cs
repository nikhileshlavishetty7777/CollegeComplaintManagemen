using CollegeComplaintManagement.Models;

namespace CollegeComplaintManagement.ViewModels
{
    public class AdminDashboardViewModel : ComplaintFilterViewModel
    {
        public int TotalStudents { get; set; }
        public int Total { get; set; }
        public int Pending { get; set; }
        public int InProgress { get; set; }
        public int Resolved { get; set; }
        public List<Complaint> Recent { get; set; } = new();
    }
}
