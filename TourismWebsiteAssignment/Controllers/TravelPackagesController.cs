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
using TourismWebsiteAssignment.Filters;

namespace TourismWebsiteAssignment.Controllers
{
    [RoleAuthorize]
    public class TravelPackagesController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        [AllowAnonymous]
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
        [RoleAuthorize("Agent")]
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            int userId = (int)Session["UserId"];

            var agencyId = db.TravelAgencies
                .Where(a => a.UserId == userId)
                .Select(a => a.AgencyId)
                .FirstOrDefault();

            if (agencyId == 0)
                return HttpNotFound(); // or redirect to agency creation

            var model = new TravelPackage
            {
                AgencyId = agencyId
            };

            return View(model);
        }


        // POST: TravelPackages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: TravelPackages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TravelPackage travelPackage)
        {
            // Auth
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            int userId = (int)Session["UserId"];

            // Find the single agency owned by this logged-in user
            int agencyId = db.TravelAgencies
                .Where(a => a.UserId == userId)
                .Select(a => a.AgencyId)
                .FirstOrDefault();

            if (agencyId == 0)
                return HttpNotFound(); // or RedirectToAction("Create", "TravelAgencies")

            // SECURITY: never trust posted AgencyId
            travelPackage.AgencyId = agencyId;

            // Model validation AFTER setting AgencyId
            if (!ModelState.IsValid)
            {
                // No dropdown in Option B, so just return view
                return View(travelPackage);
            }

            // Set timestamps
            travelPackage.CreatedAt = DateTime.Now;
            travelPackage.UpdatedAt = DateTime.Now;

            db.TravelPackages.Add(travelPackage);
            db.SaveChanges();

            return RedirectToAction("Create", "TourDates", new { packageId = travelPackage.PackageId });
        }





        // GET: TravelPackages/Edit/5
        public ActionResult Edit(int? id)
        {

            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            int userId = (int)Session["UserId"];

            // Only fetch package owned by this user's agency
            var travelPackage = db.TravelPackages
                .Include(p => p.TravelAgency)
                .FirstOrDefault(p =>
                    p.PackageId == id &&
                    p.TravelAgency.UserId == userId
                );

            if (travelPackage == null)
                return HttpNotFound();

            // No Agency dropdown in Option B
            return View(travelPackage);
        }



        // POST: TravelPackages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TravelPackage travelPackage)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            int userId = (int)Session["UserId"];

            // Get agency owned by this user
            int agencyId = db.TravelAgencies
                .Where(a => a.UserId == userId)
                .Select(a => a.AgencyId)
                .FirstOrDefault();

            if (agencyId == 0)
                return HttpNotFound();

            // Force ownership
            travelPackage.AgencyId = agencyId;

            if (!ModelState.IsValid)
                return View(travelPackage);

            travelPackage.UpdatedAt = DateTime.Now;

            db.Entry(travelPackage).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("AgentMyPackages");
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
            // Auth
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            int userId = (int)Session["UserId"];
            var role = (Session["RoleName"] as string ?? "").Trim();

            bool isAdmin = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            bool isAgent = role.Equals("Agent", StringComparison.OrdinalIgnoreCase);

            if (!isAdmin && !isAgent)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            // Load package + agency to verify ownership for Agent
            var pkg = db.TravelPackages
                .Include(p => p.TravelAgency)
                .FirstOrDefault(p => p.PackageId == id);

            if (pkg == null) return HttpNotFound();

            // Ownership check: Agents can only delete their own packages
            if (isAgent)
            {
                if (pkg.TravelAgency == null || pkg.TravelAgency.UserId != userId)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

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

            // 1) Delete PaymentTransactions
            db.PaymentTransactions.RemoveRange(
                db.PaymentTransactions.Where(p => bookingIds.Contains(p.BookingId))
            );

            // 2) Delete Feedbacks
            db.Feedbacks.RemoveRange(
                db.Feedbacks.Where(f => bookingIds.Contains(f.BookingId))
            );

            // 3) Delete Bookings
            db.Bookings.RemoveRange(
                db.Bookings.Where(b => tourDateIds.Contains(b.TourDateId))
            );

            // 4) Delete TourDates
            db.TourDates.RemoveRange(
                db.TourDates.Where(td => td.PackageId == id)
            );

            // 5) Delete PackageImages
            db.PackageImages.RemoveRange(
                db.PackageImages.Where(pi => pi.PackageId == id)
            );

            // 6) Delete the package
            db.TravelPackages.Remove(pkg);

            db.SaveChanges();

            // Redirect based on role
            if (isAgent)
                return RedirectToAction("AgentMyPackages", "TravelPackages");

            // Admin
            return RedirectToAction("OnlyView", "TravelPackages");
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
                .Include(t => t.PackageImages)
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
                .Include(p => p.PackageImages)
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
        public ActionResult FindWayBack()
        {
            var role = (Session["RoleName"] as string ?? "").Trim();

            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("OnlyView", "TravelPackages");
            }

            if (role.Equals("Agent", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("AgentMyPackages", "TravelPackages");
            }

            // fallback for unauthenticated / unknown role
            return RedirectToAction("Index", "Home");
        }

    }
}
