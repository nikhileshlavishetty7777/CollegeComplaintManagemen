using System.ComponentModel.DataAnnotations;

namespace CollegeComplaintManagement.ViewModels
{
    // Only the fields a student may fill in. Status, UserId and TrackingNumber
    // are NOT here, so they can't be changed by tampering with the form.
    public class CreateComplaintViewModel
    {
        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Complaint Category")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, MinimumLength = 5, ErrorMessage = "Title must be 5 to 150 characters.")]
        [Display(Name = "Complaint Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "Description must be 20 to 2000 characters.")]
        [Display(Name = "Complaint Description")]
        public string Description { get; set; } = string.Empty;
    }
}
