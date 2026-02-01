using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using TourismWebsiteAssignment.Data;
using TourismWebsiteAssignment.Models;
using TourismWebsiteAssignment.Filters;
namespace TourismWebsiteAssignment.Controllers
{
    [RoleAuthorize("Admin","Tourist","Agent")]
    public class BookingCreateVmController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();
        // GET: BookingCreateVm
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFromModal(BookingCreateVm vm)
        {
            if (!ModelState.IsValid)
            {
                // For now, redirect back (or return a view showing errors).
                // For a smoother UX, you can return JSON and show validation inside modal.
                return RedirectToAction("Index", "TravelPackages");
            }

            // IMPORTANT: get TouristProfileId from logged-in user/session, not from the browser
            // Example (adjust to your auth/session setup):
            // int touristProfileId = (int)Session["TouristProfileId"];

            int touristProfileId = (int)Session["TouristProfileId"]; // adjust if your session key differs

            var booking = new Booking
            {
                TouristProfileId = touristProfileId,
                TourDateId = vm.TourDateId,
                BookingStatusId = vm.BookingStatusId,
                BookingDate = vm.BookingDate,
                NumberOfGuests = vm.NumberOfGuests,
                TotalPrice = vm.TotalPrice,
                SpecialStatus = vm.SpecialStatus
            };

            db.Bookings.Add(booking);
            db.SaveChanges();

            // Next step (optional): create PaymentTransaction record here
            return RedirectToAction("Details", "Bookings", new { id = booking.BookingId });
        }

    }
}