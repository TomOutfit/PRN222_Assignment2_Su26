using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenBinhAn_A02_Business.Services;
using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAnRazorPages.Pages.News
{
    public class DetailModel : BasePageModel
    {
        private readonly INewsArticleService _newsService;

        public DetailModel(INewsArticleService newsService)
        {
            _newsService = newsService;
        }

        public NewsArticle NewsArticle { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var article = await _newsService.GetNewsByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            // Lecturers and Guests (unauthenticated) can only view active news articles
            if ((!IsAuthenticated || IsLecturer) && article.NewsStatus != true)
            {
                return NotFound();
            }

            NewsArticle = article;
            return Page();
        }
    }
}
