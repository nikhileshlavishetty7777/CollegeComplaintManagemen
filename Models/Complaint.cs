using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeComplaintManagement.Models
{
    public class Complaint
    {
        [Key]
        public int ComplaintId { get; set; }

        [Required, StringLength(20)]
        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Required, StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;

        [StringLength(1000)]
        [Display(Name = "Admin Remarks")]
        public string? AdminRemarks { get; set; }

        [Display(Name = "Submitted On")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Updated")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
