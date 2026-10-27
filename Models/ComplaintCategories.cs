namespace CollegeComplaintManagement.Models
{
    public static class ComplaintCategories
    {
        public static readonly IReadOnlyList<string> All = new[]
        {
            "Classroom",
            "Laboratory",
            "Library",
            "Hostel",
            "Infrastructure",
            "Internet/Wi-Fi",
            "Faculty",
            "Other"
        };
    }
}
