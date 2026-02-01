using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TourismWebsiteAssignment.Data;
using TourismWebsiteAssignment.Filters;
using TourismWebsiteAssignment.Models;

namespace TourismWebsiteAssignment.Controllers
{
    [RoleAuthorize]
    public class FeedbacksController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        // GET: Feedbacks
        [RoleAuthorize("Tourist","Admin")]
        public ActionResult Index()
        {
            // Base query
            var query = db.Feedbacks
                .Include(f => f.Tourist)
                .Include(f => f.Booking)
                .Include(f => f.Booking.BookingStatus)
                .Include(f => f.Booking.TourDate)
                .Include(f => f.Booking.TourDate.TravelPackage)
                .Include(f => f.Booking.TourDate.TravelPackage.TravelAgency)
                .AsQueryable();

            // Must be logged in
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "LoginRegistration");

            var loginName = (User.Identity.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(loginName))
                return RedirectToAction("Index", "LoginRegistration");

            // 1) Resolve logged-in identity -> (UserId, Role) from DB
            var me = db.Users
                .Where(u => u.Username == loginName /* OR u.Email == loginName */)
                .Select(u => new
                {
                    u.UserId,
                    Role = u.Role  // <-- CHANGE "Role" to your real column name
                })
                .FirstOrDefault();

            if (me == null || me.UserId == 0)
                return RedirectToAction("Index", "LoginRegistration");

            var role = (me.Role.RoleName ?? "").Trim();

            // 2) Admin sees all
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return View(query.OrderByDescending(f => f.SubmittedAt).ToList());
            }

            // 3) Tourist sees only own
            if (role.Equals("Tourist", StringComparison.OrdinalIgnoreCase))
            {
                var touristProfileId = db.TouristProfiles
                    .Where(t => t.UserId == me.UserId)
                    .Select(t => t.TouristProfileId)
                    .FirstOrDefault();

                if (touristProfileId == 0)
                    return RedirectToAction("Index", "LoginRegistration");

                var list = query
                    .Where(f => f.TouristProfileId == touristProfileId)
                    .OrderByDescending(f => f.SubmittedAt)
                    .ToList();

                return View(list);
            }

            // Unknown role
            return new HttpUnauthorizedResult();
        }




        // GET: Feedbacks/Create
        [RoleAuthorize("Tourist")]
        public ActionResult Create(int bookingId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            int userId = (int)Session["UserId"];

            int touristProfileId = db.TouristProfiles
                .Where(tp => tp.UserId == userId)
                .Select(tp => tp.TouristProfileId)
                .FirstOrDefault();

            if (touristProfileId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Tourist profile not found.");

            // Ensure the booking is owned by this tourist
            var booking = db.Bookings
                .Include(b => b.BookingStatus)
                .FirstOrDefault(b => b.BookingId == bookingId && b.TouristProfileId == touristProfileId);

            if (booking == null)
                return HttpNotFound("Booking not found or not yours.");

            // Optional rule: only allow confirmed bookings
            if (booking.BookingStatus?.StatusName != "Confirmed")
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Feedback allowed only for confirmed bookings.");

            // Optional: prevent duplicate feedback per booking
            bool already = db.Feedbacks.Any(f => f.BookingId == bookingId && f.TouristProfileId == touristProfileId);
            if (already)
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Feedback already submitted for this booking.");

            var model = new Feedback
            {
                BookingId = bookingId
                // TouristProfileId set on POST
                // SubmittedAt set on POST
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Tourist")]
        public ActionResult Create([Bind(Include = "BookingId,Rating,Comments")] Feedback feedback)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            int userId = (int)Session["UserId"];

            int touristProfileId = db.TouristProfiles
                .Where(tp => tp.UserId == userId)
                .Select(tp => tp.TouristProfileId)
                .FirstOrDefault();

            if (touristProfileId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Tourist profile not found.");

            // Ownership check
            bool ownsBooking = db.Bookings.Any(b => b.BookingId == feedback.BookingId && b.TouristProfileId == touristProfileId);
            if (!ownsBooking)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Invalid booking.");

            // server-controlled fields should not be validated from the form
            ModelState.Remove("SubmittedAt");
            ModelState.Remove("TouristProfileId");

            if (ModelState.IsValid)
            {
                feedback.TouristProfileId = touristProfileId;
                feedback.SubmittedAt = DateTime.Now;

                db.Feedbacks.Add(feedback);
                db.SaveChanges();

                return RedirectToAction("TouristViewBookings", "Bookings");
            }

            return View(feedback);

        }


        // GET: Feedbacks/Edit/5
        [RoleAuthorize("Tourist,Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            var role = (Session["RoleName"] as string ?? "").Trim();
            bool isAdmin = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

            Feedback feedback;

            if (isAdmin)
            {
                feedback = db.Feedbacks
                    .Include(f => f.Booking)
                    .Include(f => f.Tourist)
                    .FirstOrDefault(f => f.FeedbackId == id);
            }
            else
            {
                int userId = (int)Session["UserId"];

                int touristProfileId = db.TouristProfiles
                    .Where(tp => tp.UserId == userId)
                    .Select(tp => tp.TouristProfileId)
                    .FirstOrDefault();

                if (touristProfileId == 0)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Tourist profile not found.");

                feedback = db.Feedbacks
                    .Include(f => f.Booking)
                    .Include(f => f.Tourist)
                    .FirstOrDefault(f => f.FeedbackId == id && f.TouristProfileId == touristProfileId);
            }

            if (feedback == null)
                return HttpNotFound();
            return View(feedback);
           
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Tourist,Admin")]
        public ActionResult Edit([Bind(Include = "FeedbackId,Rating,Comments")] Feedback input)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            var role = (Session["RoleName"] as string ?? "").Trim();
            bool isAdmin = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

            Feedback feedback;

            if (isAdmin)
            {
                feedback = db.Feedbacks.FirstOrDefault(f => f.FeedbackId == input.FeedbackId);
            }
            else
            {
                int userId = (int)Session["UserId"];

                int touristProfileId = db.TouristProfiles
                    .Where(tp => tp.UserId == userId)
                    .Select(tp => tp.TouristProfileId)
                    .FirstOrDefault();

                if (touristProfileId == 0)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Tourist profile not found.");

                feedback = db.Feedbacks
                    .FirstOrDefault(f => f.FeedbackId == input.FeedbackId && f.TouristProfileId == touristProfileId);
            }

            if (feedback == null)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                feedback.Rating = input.Rating;
                feedback.Comments = input.Comments;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(feedback);
        }



        // GET: Feedbacks/Delete/5
        [RoleAuthorize("Tourist", "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
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
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [RoleAuthorize("Agent")]
        public ActionResult AgentOnlyView()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            int userId = (int)Session["UserId"];

            var feedback = db.Feedbacks
                .Include(p => p.Booking)
                .Include(p => p.Booking.TourDate)
                .Include(p => p.Booking.TourDate.TravelPackage)
                .Where(p =>
                    db.TravelAgencies.Any(a =>
                        a.AgencyId == p.Booking.TourDate.TravelPackage.AgencyId &&
                        a.UserId == userId
                    )
                )
                .OrderByDescending(p => p.SubmittedAt)
                .ToList();

            return View(feedback);
        }


    }
}
