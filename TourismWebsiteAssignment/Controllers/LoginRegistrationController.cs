using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using TourismWebsiteAssignment.Data;
using TourismWebsiteAssignment.Models;

namespace TourismWebsiteAssignment.Controllers
{
    [AllowAnonymous]
    public class LoginRegistrationController : Controller
    {
        private readonly TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        // GET: /LoginRegistration
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /LoginRegistration
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string usernameOrEmail, string password, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Please enter username/email and password.");
                return View();
            }

            string input = usernameOrEmail.Trim();
            string incomingHash = HashMd5(password); // ✅ hash incoming password

            var user = await db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u =>
                    u.Username.Equals(input, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Equals(input, StringComparison.OrdinalIgnoreCase)
                );

            // ✅ compare hash-to-hash (case-insensitive)
            if (user == null || !string.Equals(user.Password, incomingHash, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Invalid login credentials.");
                return View();
            }

            // session (optional)
            Session["UserId"] = user.UserId;
            Session["FullName"] = user.FullName;
            Session["RoleName"] = user.Role?.RoleName;

            // makes [Authorize] work
            FormsAuthentication.SetAuthCookie(user.Username, false);

            // If user was redirected to login from a protected page, go back there
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // otherwise redirect based on role
            string role = (user.Role?.RoleName ?? "").Trim();

            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Admin");

            if (role.Equals("Agent", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Agent");

            if (role.Equals("Tourist", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Tourist");

            return RedirectToAction("Index", "Home");
        }

        // GET: /LoginRegistration/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        // MD5 hashing (assignment use; not production secure)
        private static string HashMd5(string value)
        {
            if (value == null) value = "";
            value = value.Trim();

            using (var md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                byte[] hash = md5.ComputeHash(bytes);

                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2")); // lowercase hex
                return sb.ToString();
            }
        }

        // GET: /LoginRegistration/Registration
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Registration()
        {
            ViewBag.RoleId = new SelectList(
                db.Roles.Where(r => r.RoleName != "Admin"),
                "RoleId",
                "RoleName"
            );
            return View();
        }

        // POST: /LoginRegistration/Registration
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Registration([Bind(Include = "UserId,FullName,Email,Username,Password,RoleId")] User user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", user.RoleId);
                return View(user);
            }

            // Optional: basic duplicate checks (recommended)
            bool usernameExists = await db.Users.AnyAsync(u => u.Username == user.Username);
            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Username already exists.");
                ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", user.RoleId);
                return View(user);
            }

            bool emailExists = await db.Users.AnyAsync(u => u.Email == user.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email already exists.");
                ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", user.RoleId);
                return View(user);
            }

            // ✅ hash before saving
            user.Password = HashMd5(user.Password);

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
