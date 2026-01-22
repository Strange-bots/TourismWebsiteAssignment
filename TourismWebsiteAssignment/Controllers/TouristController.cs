using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TourismWebsiteAssignment.Data;
using TourismWebsiteAssignment.Models;


namespace TourismWebsiteAssignment.Controllers
{
    public class TouristController : Controller
    {
        private readonly TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();
        // GET: Tourist
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            int userId = (int)Session["UserId"];

            var profile = db.TouristProfiles
                .Include(t => t.User)
                .FirstOrDefault(t => t.UserId == userId); // requires TouristProfile.UserId FK

            if (profile == null)
                return HttpNotFound("Tourist profile not found for this user.");

            return View(profile);
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
