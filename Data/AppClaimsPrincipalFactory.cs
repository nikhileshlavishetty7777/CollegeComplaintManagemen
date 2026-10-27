using System.Security.Claims;
using CollegeComplaintManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CollegeComplaintManagement.Data
{
    // Adds the user's full name to the login cookie so the navbar can show it
    // without an extra database query on every page.
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public AppClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("FullName", user.FullName));
            return identity;
        }
    }
}
