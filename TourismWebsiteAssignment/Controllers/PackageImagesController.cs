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
    public class PackageImagesController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        // GET: PackageImages
        public ActionResult Index()
        {
            var packageImages = db.PackageImages.Include(p => p.TravelPackage);
            return View(packageImages.ToList());
        }

        // GET: PackageImages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageImage packageImage = db.PackageImages.Find(id);
            if (packageImage == null)
            {
                return HttpNotFound();
            }
            return View(packageImage);
        }

        // GET: PackageImages/Create
        public ActionResult Create()
        {
            ViewBag.PackageId = new SelectList(db.TravelPackages, "PackageId", "PackageTitle");
            return View();
        }

        // POST: PackageImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ImageId,PackageId,ImageURL,UploadedAt,Caption")] PackageImage packageImage)
        {
            if (ModelState.IsValid)
            {
                db.PackageImages.Add(packageImage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PackageId = new SelectList(db.TravelPackages, "PackageId", "PackageTitle", packageImage.PackageId);
            return View(packageImage);
        }

        // GET: PackageImages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageImage packageImage = db.PackageImages.Find(id);
            if (packageImage == null)
            {
                return HttpNotFound();
            }
            ViewBag.PackageId = new SelectList(db.TravelPackages, "PackageId", "PackageTitle", packageImage.PackageId);
            return View(packageImage);
        }

        // POST: PackageImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ImageId,PackageId,ImageURL,UploadedAt,Caption")] PackageImage packageImage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(packageImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PackageId = new SelectList(db.TravelPackages, "PackageId", "PackageTitle", packageImage.PackageId);
            return View(packageImage);
        }

        // GET: PackageImages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageImage packageImage = db.PackageImages.Find(id);
            if (packageImage == null)
            {
                return HttpNotFound();
            }
            return View(packageImage);
        }

        // POST: PackageImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PackageImage packageImage = db.PackageImages.Find(id);
            db.PackageImages.Remove(packageImage);
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
