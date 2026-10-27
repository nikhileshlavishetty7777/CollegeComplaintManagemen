using System.ComponentModel.DataAnnotations;

namespace CollegeComplaintManagement.Models
{
    public enum ComplaintStatus
    {
        Pending = 0,

        [Display(Name = "In Progress")]
        InProgress = 1,

        Resolved = 2
    }
}
