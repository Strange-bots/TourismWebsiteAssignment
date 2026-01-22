using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TourismWebsiteAssignment.Data;
using TourismWebsiteAssignment.Models; 
namespace TourismWebsiteAssignment.Controllers
{
    public class HomeController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult StatsData()
        {
            var numberOfPackages = db.Bookings
                .Select(b => b.TourDate.PackageId)
                .Distinct()
                .Count();

            var numberOfAgencies = db.Bookings
                .Select(b => b.TourDate.TravelPackage.TravelAgency.AgencyId) // adjust key name
                .Distinct()
                .Count();

            var numberOfUsers = db.Bookings
                .Select(b => b.TouristProfileId)
                .Distinct()
                .Count();

            var avgRating = db.Feedbacks.Any()
                ? db.Feedbacks.Average(f => (double)f.Rating)
                : 0.0;

            var vm = new HomeStatsVM
            {
                NumberOfPackages = numberOfPackages,
                NumberOfAgencies = numberOfAgencies,
                NumberOfUsers = numberOfUsers,
                AverageRating = Math.Round(avgRating, 1)
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }


    }
}