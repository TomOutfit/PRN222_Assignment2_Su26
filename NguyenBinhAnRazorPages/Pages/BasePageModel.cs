using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Filters;
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

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var pageType = this.GetType();
            var pageRoute = context.ActionDescriptor.ViewEnginePath;

            bool isPublicPage = pageRoute == "/Login" || 
                               pageRoute == "/AccessDenied" || 
                               pageRoute == "/Index" || 
                               pageRoute == "/News/View" || 
                               pageRoute == "/News/Detail" || 
                               pageRoute == "/Error";

            if (!isPublicPage && !IsAuthenticated)
            {
                context.Result = new RedirectToPageResult("/Login");
                return;
            }

            if (IsAuthenticated)
            {
                // Check for admin-only pages
                if (pageType.GetCustomAttributes(typeof(AdminOnlyAttribute), false).Any())
                {
                    if (!IsAdmin)
                    {
                        context.Result = new RedirectToPageResult("/AccessDenied");
                        return;
                    }
                }

                // Check for staff pages (admin and staff only)
                if (pageType.GetCustomAttributes(typeof(StaffOnlyAttribute), false).Any())
                {
                    if (!IsAdmin && !IsStaff)
                    {
                        context.Result = new RedirectToPageResult("/AccessDenied");
                        return;
                    }
                }
            }

            await next();
        }

        public virtual void OnPageAuthorization()
        {
            // Keep as empty or backward-compatible helper. The main authorization is handled by OnPageHandlerExecutionAsync.
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AdminOnlyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class StaffOnlyAttribute : Attribute { }
}

