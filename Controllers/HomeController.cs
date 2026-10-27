using System.Diagnostics;
using CollegeComplaintManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollegeComplaintManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Called by UseStatusCodePagesWithReExecute (404, 403, ...)
        public IActionResult HttpStatus(int code)
        {
            var (heading, message) = code switch
            {
                404 => ("Page Not Found", "The page you are looking for doesn't exist or has been moved."),
                403 => ("Access Denied", "You don't have permission to view this page."),
                _   => ("Something Went Wrong", "An unexpected error occurred. Please try again.")
            };

            ViewData["Code"] = code;
            ViewData["Heading"] = heading;
            ViewData["Message"] = message;
            return View();
        }
    }
}
