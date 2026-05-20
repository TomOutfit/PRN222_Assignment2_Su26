using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenBinhAn_A02_Business.Services;
using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAnRazorPages.Pages.News
{
    public class HistoryModel : BasePageModel
    {
        private readonly INewsArticleService _newsService;

        public HistoryModel(INewsArticleService newsService)
        {
            _newsService = newsService;
        }

        public IEnumerable<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
        public string SearchTerm { get; set; } = string.Empty;

        public async Task OnGetAsync(string searchTerm = "")
        {
            if (!AccountId.HasValue) return;

            SearchTerm = searchTerm ?? string.Empty;

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                var allNews = await _newsService.GetNewsHistoryAsync((short)AccountId.Value);
                NewsArticles = allNews.Where(n => 
                    n.NewsTitle!.Contains(SearchTerm) || 
                    n.Headline.Contains(SearchTerm) || 
                    n.NewsContent!.Contains(SearchTerm)).ToList();
            }
            else
            {
                NewsArticles = await _newsService.GetNewsHistoryAsync((short)AccountId.Value);
            }
        }
    }
}
