namespace CollegeComplaintManagement.ViewModels
{
    public class StudentRowViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
        public int ComplaintCount { get; set; }
    }

    public class ManageStudentsViewModel
    {
        public List<StudentRowViewModel> Students { get; set; } = new();
        public string? Search { get; set; }
    }
}
