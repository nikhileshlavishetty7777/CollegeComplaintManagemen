using CollegeComplaintManagement.Models;

namespace CollegeComplaintManagement.ViewModels
{
    public class ManageComplaintsViewModel : ComplaintFilterViewModel
    {
        public List<Complaint> Complaints { get; set; } = new();
    }
}
