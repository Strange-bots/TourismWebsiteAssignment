using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TourismWebsiteAssignment.Filters
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] _roles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            // Support both:
            // [RoleAuthorize("Admin")] and [RoleAuthorize("Tourist", "Admin")]
            // AND also: [RoleAuthorize("Tourist,Admin")]
            _roles = (roles ?? Array.Empty<string>())
                .SelectMany(r => (r ?? "")
                    .Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(r => r.Trim())
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .ToArray();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null) return false;

            // Use Session login (matches your app)
            if (httpContext.Session?["UserId"] == null) return false;

            var role = (httpContext.Session?["RoleName"] as string ?? "").Trim();
            if (string.IsNullOrEmpty(role)) return false;

            // If no roles specified, just require login
            if (_roles.Length == 0) return true;

            return _roles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase));
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Not logged in -> go login
            if (filterContext.HttpContext.Session?["UserId"] == null)
            {
                filterContext.Result = new RedirectResult("~/LoginRegistration/Index");
                return;
            }

            // Logged in but wrong role -> 403
            filterContext.Result = new HttpStatusCodeResult(403, "Forbidden");
        }
    }
}
