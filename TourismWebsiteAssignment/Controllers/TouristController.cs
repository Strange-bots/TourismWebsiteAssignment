using System.Web.Mvc;

namespace TourismWebsiteAssignment.Controllers
{
    public class TouristController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LoadSection(string section)
        {
            switch ((section ?? "").Trim())
            {
                case "Home":
                    return PartialView("Home");

                case "MyBookings":
                    return PartialView("MyBookings");

                case "MyProfile":
                    return PartialView("Profile");

                case "Payments":
                    return PartialView("Payments");

                case "Feedback":
                    return PartialView("Feedback");

                default:
                    return PartialView("Home");
            }
        }
    }
}
