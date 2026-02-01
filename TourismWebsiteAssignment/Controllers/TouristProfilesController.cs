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
    [Authorize]
    public class TouristProfilesController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        // GET: TouristProfiles
        public async Task<ActionResult> Index()
        {
            var touristProfiles = db.TouristProfiles.Include(t => t.User);
            return View(await touristProfiles.ToListAsync());
        }

        // GET: TouristProfiles/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TouristProfile touristProfile = await db.TouristProfiles.FindAsync(id);
            if (touristProfile == null)
            {
                return HttpNotFound();
            }
            return View(touristProfile);
        }

        // GET: TouristProfiles/Create
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            var role = (Session["RoleName"] as string ?? "").Trim();
            if (!role.Equals("Tourist", StringComparison.OrdinalIgnoreCase))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            int userId = (int)Session["UserId"];

            // Prevent duplicates: 1 user should have 1 profile
            bool alreadyHasProfile = db.TouristProfiles.Any(tp => tp.UserId == userId);
            if (alreadyHasProfile)
                return RedirectToAction("Index", "Tourist"); // or TouristProfiles/Details

            // Server sets UserId
            var model = new TouristProfile
            {
                UserId = userId
            };

            return View(model);
        }

        // POST: TouristProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: TouristProfiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "TouristProfileId,FullName,Gender,DateOfBirth,Address,Nationality,TravelPreferences,ProfileImageUrl")]
    TouristProfile touristProfile)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            var role = (Session["RoleName"] as string ?? "").Trim();
            if (!role.Equals("Tourist", StringComparison.OrdinalIgnoreCase))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            int userId = (int)Session["UserId"];

            // Prevent duplicates
            bool alreadyHasProfile = db.TouristProfiles.Any(tp => tp.UserId == userId);
            if (alreadyHasProfile)
                return RedirectToAction("Index", "Tourist");

            // SERVER-FILL UserId (ignore any client tampering)
            touristProfile.UserId = userId;

            touristProfile.CreatedAt = DateTime.Now;
            touristProfile.UpdatedAt = DateTime.Now;

            if (!ModelState.IsValid)
                return View(touristProfile);

            db.TouristProfiles.Add(touristProfile);
            await db.SaveChangesAsync();

            return RedirectToAction("Index", "Tourist");
        }


        // GET: TouristProfiles/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TouristProfile touristProfile = await db.TouristProfiles.FindAsync(id);
            if (touristProfile == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FullName", touristProfile.UserId);
            return View(touristProfile);
        }

        // POST: TouristProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TouristProfileId,FullName,Gender,DateOfBirth,Address,Nationality,TravelPreferences,ProfileImageUrl,UserId")] TouristProfile touristProfile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(touristProfile).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FullName", touristProfile.UserId);
            return View(touristProfile);
        }

        // GET: TouristProfiles/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TouristProfile touristProfile = await db.TouristProfiles.FindAsync(id);
            if (touristProfile == null)
            {
                return HttpNotFound();
            }
            return View(touristProfile);
        }

        // POST: TouristProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TouristProfile touristProfile = await db.TouristProfiles.FindAsync(id);
            db.TouristProfiles.Remove(touristProfile);
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
