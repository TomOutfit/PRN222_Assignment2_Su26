using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NguyenBinhAnRazorPages.Pages
{
    public class AccessDeniedModel : BasePageModel
    {
        public void OnGet()
        {
            OnPageAuthorization();
        }
    }
}
