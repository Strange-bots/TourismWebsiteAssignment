using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TourismWebsiteAssignment.Data;
using TourismWebsiteAssignment.Models;
using TourismWebsiteAssignment.Filters;

namespace TourismWebsiteAssignment.Controllers
{
    [RoleAuthorize]
    public class UsersController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        // GET: Users
        [RoleAuthorize("Admin")]
        public async Task<ActionResult> Index()
        {
            var users = db.Users.Include(u => u.Role);
            return View(await users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        [RoleAuthorize("Admin")]
        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(
                db.Roles.Where(r => r.RoleName != "Admin"),
                "RoleId",
                "RoleName"
            );
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]
        public async Task<ActionResult> Create([Bind(Include = "UserId,FullName,Email,Username,Password,RoleId")] User user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RoleId = new SelectList(
                    db.Roles.Where(r => r.RoleName != "Admin"),
                    "RoleId",
                    "RoleName",
                    user.RoleId
                );
                return View(user);
            }

            // (Recommended) prevent duplicates
            bool usernameExists = await db.Users.AnyAsync(u => u.Username == user.Username);
            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Username already exists.");
                ViewBag.RoleId = new SelectList(
                    db.Roles.Where(r => r.RoleName != "Admin"),
                    "RoleId",
                    "RoleName",
                    user.RoleId
                );
                return View(user);
            }

            bool emailExists = await db.Users.AnyAsync(u => u.Email == user.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email already exists.");
                ViewBag.RoleId = new SelectList(
                    db.Roles.Where(r => r.RoleName != "Admin"),
                    "RoleId",
                    "RoleName",
                    user.RoleId
                );
                return View(user);
            }

            // ✅ hash password before saving
            user.Password = HashMd5(user.Password);

            db.Users.Add(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Same hashing method you used in LoginRegistrationController
        private static string HashMd5(string value)
        {
            if (value == null) value = "";
            value = value.Trim();

            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
                byte[] hash = md5.ComputeHash(bytes);

                var sb = new System.Text.StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }


        // GET: Users/Edit/5
        [RoleAuthorize("Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = await db.Users.FindAsync(id);
            if (user == null) return HttpNotFound();

            // IMPORTANT: never send hashed password to UI
            user.Password = "";

            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]
        public async Task<ActionResult> Edit([Bind(Include = "UserId,FullName,Email,Username,Password,RoleId")] User form)
        {
            // Password is not required on Edit
            ModelState.Remove("Password");

            if (!ModelState.IsValid)
            {
                ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", form.RoleId);
                return View(form);
            }

            var userInDb = await db.Users.FindAsync(form.UserId);
            if (userInDb == null) return HttpNotFound();

            userInDb.FullName = form.FullName;
            userInDb.Email = form.Email;
            userInDb.Username = form.Username;
            userInDb.RoleId = form.RoleId;

            if (!string.IsNullOrWhiteSpace(form.Password))
                userInDb.Password = HashMd5(form.Password);

            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        // GET: Users/Delete/5
        [RoleAuthorize("Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = await db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id.Value);

            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
                return RedirectToAction("Index");

            // Optional safety: prevent deleting your last admin or currently logged-in user
            // int? currentUserId = Session["UserId"] as int?;
            // if (currentUserId.HasValue && currentUserId.Value == id)
            // {
            //     TempData["Error"] = "You cannot delete your own account while logged in.";
            //     return RedirectToAction("Delete", new { id });
            // }

            try
            {
                db.Users.Remove(user);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                // Most likely FK constraint: TouristProfile / Booking / Feedback etc references this User
                TempData["Error"] = "Cannot delete this user because related records exist (profiles/bookings/feedback). Delete related records first.";
                return RedirectToAction("Delete", new { id });
            }
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
