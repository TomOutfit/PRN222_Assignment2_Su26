using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;

namespace NguyenBinhAnRazorPages.Pages
{
    public class BasePageModel : PageModel
    {
        protected const int AdminRole = 0;
        protected const int StaffRole = 1;
        protected const int LecturerRole = 2;

        public string? UserEmail => HttpContext.Session.GetString("UserEmail");
        public string? UserName => HttpContext.Session.GetString("UserName");
        public int? AccountId => HttpContext.Session.GetInt32("AccountId");
        public int? AccountRole => HttpContext.Session.GetInt32("AccountRole");

        public bool IsAdmin => AccountRole == AdminRole;
        public bool IsStaff => AccountRole == StaffRole;
        public bool IsLecturer => AccountRole == LecturerRole;
        public bool IsAuthenticated => !string.IsNullOrEmpty(UserEmail);

        public virtual void OnPageAuthorization()
        {
            // Check if user is authenticated
            if (!IsAuthenticated)
            {
                Response.Redirect("/Login");
                return;
            }

            // Check if page requires specific role
            var pageType = this.GetType();
            
            // Check for admin-only pages
            if (pageType.GetCustomAttributes(typeof(AdminOnlyAttribute), false).Any())
            {
                if (!IsAdmin)
                {
                    Response.Redirect("/AccessDenied");
                    return;
                }
            }

            // Check for staff pages (admin and staff only)
            if (pageType.GetCustomAttributes(typeof(StaffOnlyAttribute), false).Any())
            {
                if (!IsAdmin && !IsStaff)
                {
                    Response.Redirect("/AccessDenied");
                    return;
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AdminOnlyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class StaffOnlyAttribute : Attribute { }
}
