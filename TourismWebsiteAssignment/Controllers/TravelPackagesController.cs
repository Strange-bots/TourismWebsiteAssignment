using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PackageId,AgencyId,PackageTitle,PackageDescription,Destination,PricePerPerson,GroupMaxSize,Inclusions,Exclusions,ItineraryDetails,TermsAndConditions")] TravelPackage travelPackage)
        {
            if (ModelState.IsValid)
            {
                db.TravelPackages.Add(travelPackage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AgencyId = new SelectList(db.TravelAgencies, "AgencyId", "AgencyName", travelPackage.AgencyId);
            return View(travelPackage);
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
            ViewBag.AgencyId = new SelectList(db.TravelAgencies, "AgencyId", "AgencyName", travelPackage.AgencyId);
            return View(travelPackage);
        }

        // POST: TravelPackages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PackageId,AgencyId,PackageTitle,PackageDescription,Destination,PricePerPerson,GroupMaxSize,Inclusions,Exclusions,ItineraryDetails,TermsAndConditions")] TravelPackage travelPackage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(travelPackage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AgencyId = new SelectList(db.TravelAgencies, "AgencyId", "AgencyName", travelPackage.AgencyId);
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
            TravelPackage travelPackage = db.TravelPackages.Find(id);
            db.TravelPackages.Remove(travelPackage);
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
    }
}
