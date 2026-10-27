using CollegeComplaintManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollegeComplaintManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Complaint> Complaints => Set<Complaint>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Complaint>(entity =>
            {
                // Tracking number must be unique
                entity.HasIndex(c => c.TrackingNumber).IsUnique();

                // Store status as readable text ("Pending") instead of 0/1/2
                entity.Property(c => c.Status)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                // One user -> many complaints
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Complaints)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
