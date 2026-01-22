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
    public class TravelPackagesController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        // GET: TravelPackages
        public ActionResult Index()
        {
            var travelPackages = db.TravelPackages.Include(t => t.TravelAgency);
            return View(travelPackages.ToList());
        }

        // GET: TravelPackages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TravelPackage travelPackage = db.TravelPackages.Find(id);
            if (travelPackage == null)
            {
                return HttpNotFound();
            }
            return View(travelPackage);
        }

        // GET: TravelPackages/Create
        public ActionResult Create()
        {
            ViewBag.AgencyId = new SelectList(db.TravelAgencies, "AgencyId", "AgencyName");
            return View();
        }

        // POST: TravelPackages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: TravelPackages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TravelPackage travelPackage)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AgencyId = new SelectList(db.TravelAgencies, "AgencyId", "AgencyName", travelPackage.AgencyId);
                return View(travelPackage);
            }

            // Set timestamps
            travelPackage.CreatedAt = DateTime.Now;
            travelPackage.UpdatedAt = DateTime.Now;

            db.TravelPackages.Add(travelPackage);
            db.SaveChanges(); // generates PackageId

            // Redirect to TourDates/Create with the new PackageId
            return RedirectToAction("Create", "TourDates", new { packageId = travelPackage.PackageId });
        }



        // GET: TravelPackages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TravelPackage travelPackage = db.TravelPackages.Find(id);
            if (travelPackage == null)
            {
                return HttpNotFound();
            }

            ViewBag.AgencyId = new SelectList(
                db.TravelAgencies,
                "AgencyId",
                "AgencyName",
                travelPackage.AgencyId
            );

            return View(travelPackage); // ✅ THIS was missing
        }


        // POST: TravelPackages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TravelPackage travelPackage)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AgencyId = new SelectList(
                    db.TravelAgencies,
                    "AgencyId",
                    "AgencyName",
                    travelPackage.AgencyId
                );
                return View(travelPackage);
            }

            db.Entry(travelPackage).State = EntityState.Modified;
            db.SaveChanges();

            // stay on the same page
            ViewBag.AgencyId = new SelectList(
                db.TravelAgencies,
                "AgencyId",
                "AgencyName",
                travelPackage.AgencyId
            );

            return View(travelPackage);
        }



        // GET: TravelPackages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TravelPackage travelPackage = db.TravelPackages.Find(id);
            if (travelPackage == null)
            {
                return HttpNotFound();
            }
            return View(travelPackage);
        }

        // POST: TravelPackages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var pkg = db.TravelPackages.Find(id);
            if (pkg == null) return HttpNotFound();

            // TourDates for this package
            var tourDateIds = db.TourDates
                .Where(td => td.PackageId == id)
                .Select(td => td.TourDateId)
                .ToList();

            // Bookings for those TourDates
            var bookingIds = db.Bookings
                .Where(b => tourDateIds.Contains(b.TourDateId))
                .Select(b => b.BookingId)
                .ToList();

            // 1) Delete PaymentTransactions that reference those bookings (if table exists)
            var payments = db.PaymentTransactions.Where(p => bookingIds.Contains(p.BookingId));
            db.PaymentTransactions.RemoveRange(payments);

            // 2) Delete Feedbacks that reference those bookings (if table exists)
            var feedbacks = db.Feedbacks.Where(f => bookingIds.Contains(f.BookingId));
            db.Feedbacks.RemoveRange(feedbacks);

            // 3) Delete Bookings
            var bookings = db.Bookings.Where(b => tourDateIds.Contains(b.TourDateId));
            db.Bookings.RemoveRange(bookings);

            // 4) Delete TourDates
            var tourDates = db.TourDates.Where(td => td.PackageId == id);
            db.TourDates.RemoveRange(tourDates);

            // 5) Delete PackageImages
            var images = db.PackageImages.Where(pi => pi.PackageId == id);
            db.PackageImages.RemoveRange(images);

            // 6) Delete the package
            db.TravelPackages.Remove(pkg);

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
        public async Task<ActionResult> OnlyView()
        {
            var travelPackages = await db.TravelPackages
                .Include(t => t.TravelAgency)
                .ToListAsync();

            return View(travelPackages);
        }


        //AGENT - MY PACKAGES
        public async Task<ActionResult> AgentMyPackages()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            var role = (Session["RoleName"] as string ?? "").Trim();
            if (!role.Equals("Agent", StringComparison.OrdinalIgnoreCase))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            int userId = (int)Session["UserId"];

            // Check if agent has an agency profile first
            var agency = await db.TravelAgencies.FirstOrDefaultAsync(a => a.UserId == userId);
            if (agency == null)
            {
                // Agent must create their agency profile before packages
                return RedirectToAction("Create", "TravelAgencies");
            }

            var packages = await db.TravelPackages
                .Where(p => p.AgencyId == agency.AgencyId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            // If none, either show empty view or redirect to create
            if (!packages.Any())
            {
                return RedirectToAction("Create", "TravelPackages");
            }
            return View(packages);
        }

        //Tourist - Browse Packages
                public async Task<ActionResult> TouristViewPackages()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");
            var role = (Session["RoleName"] as string ?? "").Trim();
            if (!role.Equals("Tourist", StringComparison.OrdinalIgnoreCase))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            var packages = await db.TravelPackages
                .Include(p => p.TravelAgency)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(packages);
        }
    }
}
