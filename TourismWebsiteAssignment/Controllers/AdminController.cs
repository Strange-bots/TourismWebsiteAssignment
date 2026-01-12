using System.Web.Mvc;
using TourismWebsiteAssignment.Data;
using System.Data.Entity;
using System.Linq;



namespace TourismWebsiteAssignment.Controllers
{
    public class AdminController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();
        // GET: /Agent
        public ActionResult Index()
        {
            // This loads the dashboard shell (sidebar + main area)
            return View();
        }

        // GET: /Agent/LoadSection?section=Home
        [HttpGet]
        public ActionResult LoadSection(string section)
        {
            switch ((section ?? "").Trim())
            {
                case "Home":
                    return PartialView("Home");

                case "Users":
                    return PartialView("Users");

                case "RolesPermissions":
                    return PartialView("Roles");

                case "TravelAgencies":
                    return PartialView("TravelAgencies");

                case "Bookings":
                    return PartialView("Bookings");

                case "Payment":
                    return PartialView("Payment");

                case "Settings":
                    return PartialView("Settings");

                default:
                    return PartialView("Home");     
            }
        }
    }
}
