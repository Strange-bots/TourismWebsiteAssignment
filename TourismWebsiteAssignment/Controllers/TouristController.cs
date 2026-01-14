using System.Web.Mvc;

namespace TourismWebsiteAssignment.Controllers
{
    public class TouristController : Controller
    {
        // GET: Tourist
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Bookings()
        {
            return View();
        }

        public ActionResult Payments()
        {
            return View();
        }

        public ActionResult Feedback()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }
    }
}
