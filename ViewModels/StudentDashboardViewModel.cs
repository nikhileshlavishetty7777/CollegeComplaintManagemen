using CollegeComplaintManagement.Models;

namespace CollegeComplaintManagement.ViewModels
{
    public class StudentDashboardViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Pending { get; set; }
        public int InProgress { get; set; }
        public int Resolved { get; set; }
        public List<Complaint> Recent { get; set; } = new();
    }
}
