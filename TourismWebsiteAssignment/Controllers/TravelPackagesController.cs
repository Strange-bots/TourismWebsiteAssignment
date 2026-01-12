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
    public class TravelPackagesController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();
        private object travelpackages;

        // GET: TravelPackages
        public async Task<ActionResult> Index()
        {
            var travelPackages = db.TravelPackages.Include(t => t.TravelAgency);
            return View(await travelPackages.ToListAsync());
        }

        // GET: TravelPackages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TravelPackage travelPackage = await db.TravelPackages.FindAsync(id);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PackageId,AgencyId,PackageTitle,PackageDescription,Destination,PricePerPerson,GroupMaxSize,Inclusions,Exclusions,ItineraryDetails,TermsAndConditions,CreatedAt,UpdatedAt")] TravelPackage travelPackage)
        {
            if (ModelState.IsValid)
            {
                db.TravelPackages.Add(travelPackage);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AgencyId = new SelectList(db.TravelAgencies, "AgencyId", "AgencyName", travelPackage.AgencyId);
            return View(travelPackage);
        }

        // GET: TravelPackages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TravelPackage travelPackage = await db.TravelPackages.FindAsync(id);
            if (travelPackage == null)
            {
                return HttpNotFound();
            }
            ViewBag.AgencyId = new SelectList(db.TravelAgencies, "AgencyId", "AgencyName", travelPackage.AgencyId);
            return View(travelPackage);
        }

        // POST: TravelPackages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PackageId,AgencyId,PackageTitle,PackageDescription,Destination,PricePerPerson,GroupMaxSize,Inclusions,Exclusions,ItineraryDetails,TermsAndConditions,CreatedAt,UpdatedAt")] TravelPackage travelPackage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(travelPackage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AgencyId = new SelectList(db.TravelAgencies, "AgencyId", "AgencyName", travelPackage.AgencyId);
            return View(travelPackage);
        }

        // GET: TravelPackages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TravelPackage travelPackage = await db.TravelPackages.FindAsync(id);
            if (travelPackage == null)
            {
                return HttpNotFound();
            }
            return View(travelPackage);
        }

        // POST: TravelPackages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TravelPackage travelPackage = await db.TravelPackages.FindAsync(id);
            db.TravelPackages.Remove(travelPackage);
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

        public ActionResult DashboardList()
        {
            var traverlpackages = db.TravelPackages.Include(t => t.TravelAgency).ToList();

            return PartialView("DashboardBookings", travelpackages);
        }
    }
}
