using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NguyenBinhAnRazorPages.Pages
{
    public class AccessDeniedModel : BasePageModel
    {
        public void OnGet()
        {
            // Skip base authorization — this page should always be accessible
        }
    }
}
