using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TourismWebsiteAssignment.Data;
using TourismWebsiteAssignment.Models;

namespace TourismWebsiteAssignment.Controllers
{
    public class BookingsController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        // GET: Bookings
        public ActionResult Index()
        {
            var bookings = db.Bookings.Include(b => b.BookingStatus).Include(b => b.TourDate).Include(b => b.Tourist);
            return View(bookings.ToList());
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {
            ViewBag.BookingStatusId = new SelectList(db.BookingStatus, "BookingStatusId", "StatusName");
            ViewBag.TourDateId = new SelectList(db.TourDates, "TourDateId", "Status");
            ViewBag.TouristProfileId = new SelectList(db.TouristProfiles, "TouristProfileId", "FullName");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingId,TouristProfileId,TourDateId,BookingStatusId,BookingDate,NumberOfGuests,TotalPrice,SpecialStatus")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BookingStatusId = new SelectList(db.BookingStatus, "BookingStatusId", "StatusName", booking.BookingStatusId);
            ViewBag.TourDateId = new SelectList(db.TourDates, "TourDateId", "Status", booking.TourDateId);
            ViewBag.TouristProfileId = new SelectList(db.TouristProfiles, "TouristProfileId", "FullName", booking.TouristProfileId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.BookingStatusId = new SelectList(db.BookingStatus, "BookingStatusId", "StatusName", booking.BookingStatusId);
            ViewBag.TourDateId = new SelectList(db.TourDates, "TourDateId", "Status", booking.TourDateId);
            ViewBag.TouristProfileId = new SelectList(db.TouristProfiles, "TouristProfileId", "FullName", booking.TouristProfileId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingId,TouristProfileId,TourDateId,BookingStatusId,BookingDate,NumberOfGuests,TotalPrice,SpecialStatus")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BookingStatusId = new SelectList(db.BookingStatus, "BookingStatusId", "StatusName", booking.BookingStatusId);
            ViewBag.TourDateId = new SelectList(db.TourDates, "TourDateId", "Status", booking.TourDateId);
            ViewBag.TouristProfileId = new SelectList(db.TouristProfiles, "TouristProfileId", "FullName", booking.TouristProfileId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var booking = db.Bookings.Find(id);
            if (booking == null) return HttpNotFound();

            // 1) Remove dependents first (adjust table names if yours differ)
            var payments = db.PaymentTransactions.Where(p => p.BookingId == id);
            db.PaymentTransactions.RemoveRange(payments);

            var feedbacks = db.Feedbacks.Where(f => f.BookingId == id);
            db.Feedbacks.RemoveRange(feedbacks);

            // Add more dependents here if you have them:
            // var tickets = db.Tickets.Where(t => t.BookingId == id);
            // db.Tickets.RemoveRange(tickets);

            // 2) Remove booking
            db.Bookings.Remove(booking);

            // 3) Save
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult DashboardList()
        {
            var bookings = db.Bookings
                .Include(b => b.TourDate)
                .Include(b => b.Tourist)
                .ToList();

            return PartialView("DashboardBookings", bookings);
        }
        public ActionResult OnlyEdit()
        {
            var bookings = db.Bookings
                .Include(b => b.BookingStatus)
                .Include(b => b.TourDate)
                .Include(b => b.Tourist);

            return View(bookings.ToList());
        }

        public async Task<ActionResult> AgentMyBookingsOnlyView()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            var role = (Session["RoleName"] as string ?? "").Trim();
            if (!role.Equals("Agent", StringComparison.OrdinalIgnoreCase))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            int userId = (int)Session["UserId"];

            var agency = await db.TravelAgencies.FirstOrDefaultAsync(a => a.UserId == userId);
            if (agency == null)
                return RedirectToAction("Create", "TravelAgencies");

            var bookings = await db.Bookings
                .Include(b => b.TourDate)
                .Include(b => b.TourDate.TravelPackage)
                .Include(b => b.Tourist)         // TouristProfile
                .Include(b => b.BookingStatus)
                .Where(b => b.TourDate.TravelPackage.AgencyId == agency.AgencyId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }
        public ActionResult TouristViewBookings()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            var role = (Session["RoleName"] as string ?? "").Trim();
            if (!role.Equals("Tourist", StringComparison.OrdinalIgnoreCase))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            int userId = (int)Session["UserId"];
            var bookings = db.Bookings.Include(b => b.BookingStatus).Include(b => b.TourDate).Include(b => b.Tourist).Where(b => b.TouristProfileId == userId);
            return View(bookings.ToList());
        }
        //Get: Bookings/TouristCreateBookings
        public ActionResult TouristCreateBookings(int packageId, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? Request.UrlReferrer?.ToString();
            ViewBag.PackageId = packageId;
             
            var dates = db.TourDates.Where(td => td.PackageId == packageId).ToList();
            ViewBag.TourDateId = new SelectList(dates, "TourDateId", "Status");

            return View(new Booking { NumberOfGuests = 1 });
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult TouristCreateBookings(
    [Bind(Include = "TourDateId,NumberOfGuests,SpecialStatus")] Booking booking,
    int packageId,
    string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PackageId = packageId;

            // Always repopulate dropdown when returning View()
            Func<SelectList> loadDates = () =>
            {
                var dates = db.TourDates
                    .Where(td => td.PackageId == packageId)
                    .ToList();

                return new SelectList(dates, "TourDateId", "Status", booking.TourDateId);
            };

            // 1) Validate TourDate belongs to selected Package
            var tourDateOk = db.TourDates.Any(td => td.TourDateId == booking.TourDateId && td.PackageId == packageId);
            if (!tourDateOk)
                ModelState.AddModelError("TourDateId", "Invalid tour date for the selected package.");

            // 2) Ensure required values that users may leave blank
            booking.SpecialStatus = (booking.SpecialStatus ?? "").Trim();
            if (string.IsNullOrWhiteSpace(booking.SpecialStatus))
                ModelState.AddModelError("SpecialStatus", "Special requests is required.");

            if (!ModelState.IsValid)
            {
                ViewBag.TourDateId = loadDates();
                return View(booking);
            }

            // 3) Set REQUIRED system fields server-side (critical)
            booking.BookingDate = DateTime.Today;
            booking.BookingStatusId = 1;

            // TouristProfileId MUST be set (example using Session)
            if (Session["UserId"] == null)
            {
                ModelState.AddModelError("", "You must be logged in as a tourist to book.");
                ViewBag.TourDateId = loadDates();
                return View(booking);
            }
            booking.TouristProfileId = (int)Session["UserId"];

            // TotalPrice MUST be set (compute from package)
            var pkg = db.TravelPackages.Find(packageId);
            if (pkg == null)
            {
                ModelState.AddModelError("", "Package not found.");
                ViewBag.TourDateId = loadDates();
                return View(booking);
            }
            booking.TotalPrice = pkg.PricePerPerson * booking.NumberOfGuests;

            try
            {
                db.Bookings.Add(booking);
                db.SaveChanges();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                // This prints the real SQL error message in debug
                var msg = ex.InnerException?.InnerException?.Message
                          ?? ex.InnerException?.Message
                          ?? ex.Message;

                ModelState.AddModelError("", msg);

                ViewBag.TourDateId = loadDates();
                return View(booking);
            }

            // Go to payment page for this booking
            return RedirectToAction("Create", "PaymentTransactions", new { bookingId = booking.BookingId });

        }



    }
}
