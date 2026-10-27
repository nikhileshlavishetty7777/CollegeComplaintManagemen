namespace CollegeComplaintManagement.ViewModels
{
    public class ProfileViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
        public int TotalComplaints { get; set; }
    }
}
