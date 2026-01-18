using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TourismWebsiteAssignment.Data;
using TourismWebsiteAssignment.Models;

namespace TourismWebsiteAssignment.Controllers
{
    public class FeedbacksController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        // GET: Feedbacks
        public ActionResult Index()
        {
            var feedbacks = db.Feedbacks
                .Include(f => f.Booking)
                .Include(f => f.Tourist);

            return View(feedbacks.ToList());
        }

        // GET: Feedbacks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Feedback feedback = db.Feedbacks.Find(id);

            if (feedback == null)
                return HttpNotFound();

            return View(feedback);
        }

        // GET: Feedbacks/Create
        public ActionResult Create()
        {
            ViewBag.BookingId = new SelectList(db.Bookings, "BookingId", "SpecialStatus");
            ViewBag.TouristId = new SelectList(db.TouristProfiles, "TouristProfileId", "FullName");
            return View();
        }

        // POST: Feedbacks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TouristId,BookingId,Rating,Comments")] Feedback feedback)
        {
            // 1️⃣ Validate booking exists
            var booking = db.Bookings.Find(feedback.BookingId);
            if (booking == null)
            {
                ModelState.AddModelError("BookingId", "Invalid booking.");
            }
            else
            {
                // 2️⃣ Validate booking belongs to tourist
                if (booking.TouristProfileId != feedback.TouristId)
                {
                    ModelState.AddModelError("BookingId", "This booking does not belong to you.");
                }

                // 3️⃣ Validate booking status (assuming 2 is the "Completed" status in BookingStatus table)
                if (booking.BookingStatusId != 2)  // Make sure `2` corresponds to "Completed" status in your system
                {
                    ModelState.AddModelError("BookingId", "Feedback allowed only after booking is completed.");
                }

                // 4️⃣ Prevent duplicate feedback
                bool alreadyExists = db.Feedbacks.Any(f =>
                    f.BookingId == feedback.BookingId &&
                    f.TouristId == feedback.TouristId);

                if (alreadyExists)
                {
                    ModelState.AddModelError("", "Feedback already submitted for this booking.");
                }
            }

            if (ModelState.IsValid)
            {
                // 5️⃣ Set SubmittedAt automatically
                feedback.SubmittedAt = DateTime.Now;

                db.Feedbacks.Add(feedback);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Reload dropdowns if validation fails
            ViewBag.BookingId = new SelectList(db.Bookings, "BookingId", "SpecialStatus", feedback.BookingId);
            ViewBag.TouristId = new SelectList(db.TouristProfiles, "TouristProfileId", "FullName", feedback.TouristId);

            return View(feedback);
        }

        // GET: Feedbacks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Feedback feedback = db.Feedbacks.Find(id);

            if (feedback == null)
                return HttpNotFound();

            ViewBag.BookingId = new SelectList(db.Bookings, "BookingId", "SpecialStatus", feedback.BookingId);
            ViewBag.TouristId = new SelectList(db.TouristProfiles, "TouristProfileId", "FullName", feedback.TouristId);

            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FeedbackId,TouristId,BookingId,Rating,Comments,SubmittedAt")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feedback).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BookingId = new SelectList(db.Bookings, "BookingId", "SpecialStatus", feedback.BookingId);
            ViewBag.TouristId = new SelectList(db.TouristProfiles, "TouristProfileId", "FullName", feedback.TouristId);

            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Feedback feedback = db.Feedbacks.Find(id);

            if (feedback == null)
                return HttpNotFound();

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Feedback feedback = db.Feedbacks.Find(id);
            db.Feedbacks.Remove(feedback);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
