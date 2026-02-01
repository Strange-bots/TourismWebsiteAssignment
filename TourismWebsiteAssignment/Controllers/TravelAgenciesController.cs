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
using TourismWebsiteAssignment.Filters;

namespace TourismWebsiteAssignment.Controllers
{
    [RoleAuthorize]
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
        [RoleAuthorize("Agent")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: TravelAgencies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
      [Bind(Include = "AgencyId,AgencyName,LicenseNumber,ContactNumber,AgencyAddress,ContactPerson,PhoneNumber,AgencyDescription,LogoUrl")]
    TravelAgency travelAgency)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            int userId = (int)Session["UserId"];

            if (ModelState.IsValid)
            {
                // Server-controlled assignment
                travelAgency.UserId = userId;

                db.TravelAgencies.Add(travelAgency);
                await db.SaveChangesAsync();

                return RedirectToAction("AgentMyAgency");
            }

            return View(travelAgency);
        }


        // GET: TravelAgencies/Edit/5
        [RoleAuthorize("Agent")]
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
                return RedirectToAction("AgentMyAgency");
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FullName", travelAgency.UserId);
            db.Entry(travelAgency).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("AgentMyAgency");
        }

        // GET: TravelAgencies/Delete/5
        [RoleAuthorize("Agent","Admin")]
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var travelAgency = await db.TravelAgencies.FindAsync(id);
            if (travelAgency == null)
                return HttpNotFound();

            db.TravelAgencies.Remove(travelAgency);
            await db.SaveChangesAsync();

            // ---- role comes from your DB using Session user id ----
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            int userId = (int)Session["UserId"];

            var roleName = await db.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.Role.RoleName)
                .FirstOrDefaultAsync();

            if (roleName == "Agent")
                return RedirectToAction("AgentMyAgency");

            if (roleName == "Admin")
                return RedirectToAction("OnlyEdit");

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
        public async Task<ActionResult> OnlyEdit()
        {
            var agencies = await db.TravelAgencies
                .Include(a => a.User)
                .ToListAsync();

            return View(agencies);
        }


        //Agent Travel Agencies by thier UserId
        [RoleAuthorize("Agent")]
        public ActionResult AgentMyAgency()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            int userId = (int)Session["UserId"];

            var agency = db.TravelAgencies
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .ToList();

            if (!agency.Any())
            {
                // either show empty page, or redirect to create
                return RedirectToAction("Create", "TravelAgencies");
            }
            return View(agency);
        }

    }
}
