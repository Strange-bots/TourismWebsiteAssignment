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
    public class TourDatesController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        // GET: TourDates
        public async Task<ActionResult> Index()
        {
            var tourDates = db.TourDates.Include(t => t.TravelPackage);
            return View(await tourDates.ToListAsync());
        }

        // GET: TourDates/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TourDate tourDate = await db.TourDates.FindAsync(id);
            if (tourDate == null)
            {
                return HttpNotFound();
            }
            return View(tourDate);
        }

        // GET: TourDates/Create
        public ActionResult Create(int? packageId)
        {
            if (packageId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "PackageId is required.");

            var model = new TourDate
            {
                PackageId = packageId.Value,
                Status = "Active"
            };

            return View(model);
        }



        // POST: TourDates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TourDateId,PackageId,StartDate,EndDate,AvailableSlots,TotalSlots,Status,PriceAdjustment")] TourDate tourDate)
        {
            if (ModelState.IsValid)
            {
                db.TourDates.Add(tourDate);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PackageId = new SelectList(db.TravelPackages, "PackageId", "PackageTitle", tourDate.PackageId);
            return View(tourDate);
        }

        // GET: TourDates/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TourDate tourDate = await db.TourDates.FindAsync(id);
            if (tourDate == null)
            {
                return HttpNotFound();
            }
            ViewBag.PackageId = new SelectList(db.TravelPackages, "PackageId", "PackageTitle", tourDate.PackageId);
            return View(tourDate);
        }

        // POST: TourDates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TourDateId,PackageId,StartDate,EndDate,AvailableSlots,TotalSlots,Status,PriceAdjustment")] TourDate tourDate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tourDate).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PackageId = new SelectList(db.TravelPackages, "PackageId", "PackageTitle", tourDate.PackageId);
            return View(tourDate);
        }

        // GET: TourDates/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TourDate tourDate = await db.TourDates.FindAsync(id);
            if (tourDate == null)
            {
                return HttpNotFound();
            }
            return View(tourDate);
        }

        // POST: TourDates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TourDate tourDate = await db.TourDates.FindAsync(id);
            db.TourDates.Remove(tourDate);
            await db.SaveChangesAsync();
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

        //My Tour Dates
        public async Task<ActionResult> AgentMyTours()
        {
            // Must be logged in
            if (Session["UserId"] == null) return RedirectToAction("Index", "LoginRegistration");

            // Must be Agent
            var role = (Session["RoleName"] as string ?? "").Trim();
            if (!role.Equals("Agent", StringComparison.OrdinalIgnoreCase))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            int userId = (int)Session["UserId"];

            // Find this agent's agency
            var agency = await db.TravelAgencies.FirstOrDefaultAsync(a => a.UserId == userId);
            if (agency == null)
            {
                // Agent has no agency profile yet
                return RedirectToAction("Create", "TravelAgencies");
            }

            // Tour dates for packages owned by this agency
            var tourDates = await db.TourDates
                .Include(td => td.TravelPackage)
                .Where(td => td.TravelPackage.AgencyId == agency.AgencyId)
                .OrderBy(td => td.StartDate)
                .ToListAsync();

            return View(tourDates);
        }
    }
}
