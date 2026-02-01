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
using TourismWebsiteAssignment.Filters;

namespace TourismWebsiteAssignment.Controllers
{
    [RoleAuthorize("Agent")]
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
        // GET: PackageImages/Create or PackageImages/Create?packageId=10
        public ActionResult Create(int? packageId)
        {
            if (packageId.HasValue)
            {
                var pkg = db.TravelPackages.Find(packageId.Value);
                if (pkg == null) return HttpNotFound();

                // Preselect package and allow you to hide dropdown in the view if you want
                ViewBag.PackageId = new SelectList(db.TravelPackages, "PackageId", "PackageTitle", packageId.Value);
                ViewBag.FixedPackageId = packageId.Value;

                return View(new PackageImage { PackageId = packageId.Value });
            }

            ViewBag.PackageId = new SelectList(db.TravelPackages, "PackageId", "PackageTitle");
            return View();
        }

        // POST: PackageImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ImageId,PackageId,ImageURL,Caption")] PackageImage packageImage)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PackageId = new SelectList(db.TravelPackages, "PackageId", "PackageTitle", packageImage.PackageId);
                ViewBag.FixedPackageId = packageImage.PackageId;
                return View(packageImage);
            }

            packageImage.UploadedAt = DateTime.Now; // IMPORTANT if your model requires it
            db.PackageImages.Add(packageImage);
            db.SaveChanges();

            // Keep flow: go back to package page (choose what fits your system)
            return RedirectToAction("Details", "TravelPackages", new { id = packageImage.PackageId });
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
        public ActionResult Edit([Bind(Include = "ImageId,PackageId,ImageURL,Caption")] PackageImage packageImage)
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
