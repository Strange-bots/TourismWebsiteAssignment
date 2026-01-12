using System.Web.Mvc;
using TourismWebsiteAssignment.Data;
using System.Data.Entity;
using System.Linq;

namespace TourismWebsiteAssignment.Controllers
{
    public class AgentController : Controller
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

                case "Packages":
                    return PartialView("~/Views/TravelPackages/DashboardPackages.cshtml",
                        db.TravelPackages.Include(t => t.TravelAgency).ToList());
                case "Bookings":
                    return PartialView("~/Views/Bookings/DashboardBookings.cshtml",
                        db.Bookings.Include(b => b.TourDate).Include(b => b.Tourist).ToList());

                case "TourDates":
                    return PartialView("TourDates");

                case "Profile":
                    return PartialView("Profile");

                case "Settings":
                    return PartialView("Settings");

                default:
                    return PartialView("Home");
            }
        }
    }
}
