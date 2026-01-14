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
    public class TravelAgenciesController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        // GET: TravelAgencies
        public async Task<ActionResult> Index()
        {
            var travelAgencies = db.TravelAgencies.Include(t => t.User);
            return View(await travelAgencies.ToListAsync());
        }

        // GET: TravelAgencies/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TravelAgency travelAgency = await db.TravelAgencies.FindAsync(id);
            if (travelAgency == null)
            {
                return HttpNotFound();
            }
            return View(travelAgency);
        }

        // GET: TravelAgencies/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FullName");
            return View();
        }

        // POST: TravelAgencies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AgencyId,UserId,AgencyName,LicenseNumber,ContactNumber,AgencyAddress,ContactPerson,PhoneNumber,AgencyDescription,LogoUrl")] TravelAgency travelAgency)
        {
            if (ModelState.IsValid)
            {
                db.TravelAgencies.Add(travelAgency);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "UserId", "FullName", travelAgency.UserId);
            return View(travelAgency);
        }

        // GET: TravelAgencies/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TravelAgency travelAgency = await db.TravelAgencies.FindAsync(id);
            if (travelAgency == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FullName", travelAgency.UserId);
            return View(travelAgency);
        }

        // POST: TravelAgencies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AgencyId,UserId,AgencyName,LicenseNumber,ContactNumber,AgencyAddress,ContactPerson,PhoneNumber,AgencyDescription,LogoUrl")] TravelAgency travelAgency)
        {
            if (ModelState.IsValid)
            {
                db.Entry(travelAgency).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FullName", travelAgency.UserId);
            return View(travelAgency);
        }

        // GET: TravelAgencies/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TravelAgency travelAgency = await db.TravelAgencies.FindAsync(id);
            if (travelAgency == null)
            {
                return HttpNotFound();
            }
            return View(travelAgency);
        }

        // POST: TravelAgencies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TravelAgency travelAgency = await db.TravelAgencies.FindAsync(id);
            db.TravelAgencies.Remove(travelAgency);
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
    }
}
