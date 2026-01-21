using System.Linq;
using System.Web.Mvc;
using TourismWebsiteAssignment.Data;
using TourismWebsiteAssignment.Models;

namespace TourismWebsiteAssignment.Controllers
{
    public class ShowDataController : Controller
    {
        private readonly TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        public ActionResult Index()
        {
            var vm = new ShowData
            {
                TourDates = db.TourDates.ToList(),
                TravelPackages = db.TravelPackages.ToList(),
                Feedbacks = db.Feedbacks.ToList()
            };

            return View(vm);
        }
    }
}
