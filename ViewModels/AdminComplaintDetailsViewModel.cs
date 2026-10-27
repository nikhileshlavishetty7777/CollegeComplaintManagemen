using System.ComponentModel.DataAnnotations;
using CollegeComplaintManagement.Models;

namespace CollegeComplaintManagement.ViewModels
{
    // Used to display a complaint AND to receive the admin's update form.
    public class AdminComplaintDetailsViewModel
    {
        // Display only (never bound from the form)
        public Complaint? Complaint { get; set; }

        // Form fields
        public int ComplaintId { get; set; }

        [Required(ErrorMessage = "Please select a status.")]
        public ComplaintStatus Status { get; set; }

        [StringLength(1000, ErrorMessage = "Remarks cannot exceed 1000 characters.")]
        [Display(Name = "Administrative Remarks")]
        public string? AdminRemarks { get; set; }
    }
}
