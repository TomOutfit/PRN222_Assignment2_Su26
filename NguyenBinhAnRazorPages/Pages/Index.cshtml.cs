using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NguyenBinhAnRazorPages.Pages
{
    public class IndexModel : BasePageModel
    {
        public void OnGet()
        {
            OnPageAuthorization();
        }
    }
}
