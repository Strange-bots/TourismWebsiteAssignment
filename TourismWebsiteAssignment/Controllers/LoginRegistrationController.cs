using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TourismWebsiteAssignment.Data;

namespace TourismWebsiteAssignment.Controllers
{
    public class LoginRegistrationController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        // GET: /LoginRegistration
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // POST: /LoginRegistration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string usernameOrEmail, string password)
        {
            // 1) Basic validation
            if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Please enter username/email and password.");
                return View();
            }

            string input = usernameOrEmail.Trim();

            // ❌ HASHING DISABLED
            // string hashedPassword = HashSha256(password);

            // 2) Find user by username OR email
            var user = await db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u =>
                    u.Username.Equals(input, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Equals(input, StringComparison.OrdinalIgnoreCase)
                );

            // 3) Validate credentials (PLAIN TEXT COMPARISON)
            if (user == null || user.Password != password)
            {
                ModelState.AddModelError("", "Invalid login credentials.");
                return View();
            }

            // 4) Create session
            Session["UserId"] = user.UserId;
            Session["FullName"] = user.FullName;
            Session["RoleName"] = user.Role != null ? user.Role.RoleName : null;

            // 5) Redirect based on role
            string role = (user.Role != null ? user.Role.RoleName : "").Trim();

            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Admin");

            if (role.Equals("Agent", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Agent");

            if (role.Equals("Tourist", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Tourist");

            // default Tourist
            return RedirectToAction("Index", "Home");
        }

        // GET: /LoginRegistration/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }

        /*
        ============================
        HASHING DISABLED FOR NOW
        ============================

        This method is intentionally commented.
        You can re-enable it later for production security.

        private static string HashSha256(string value)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                byte[] hash = sha.ComputeHash(bytes);

                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
        */

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
