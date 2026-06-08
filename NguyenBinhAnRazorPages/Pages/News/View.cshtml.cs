using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenBinhAn_A02_Business.Services;
using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAnRazorPages.Pages.News
{
    public class ViewModel : BasePageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;

        public ViewModel(INewsArticleService newsService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
        }

        public IEnumerable<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public string SearchTerm { get; set; } = string.Empty;
        public short? CategoryId { get; set; }

        public async Task OnGetAsync(string searchTerm = "", short categoryId = 0)
        {

            SearchTerm = searchTerm ?? string.Empty;
            CategoryId = categoryId > 0 ? categoryId : null;

            // Load categories for filter dropdown
            Categories = await _categoryService.GetActiveCategoriesAsync();

            // Load news articles based on filters
            if (!string.IsNullOrEmpty(SearchTerm) || CategoryId.HasValue)
            {
                if (CategoryId.HasValue && !string.IsNullOrEmpty(SearchTerm))
                {
                    var categoryNews = await _newsService.GetNewsByCategoryAsync(CategoryId.Value);
                    NewsArticles = categoryNews.Where(n =>
                        n.NewsTitle!.Contains(SearchTerm) ||
                        n.Headline.Contains(SearchTerm) ||
                        n.NewsContent!.Contains(SearchTerm));
                }
                else if (CategoryId.HasValue)
                {
                    NewsArticles = await _newsService.GetNewsByCategoryAsync(CategoryId.Value);
                }
                else
                {
                    NewsArticles = await _newsService.SearchNewsAsync(SearchTerm);
                }
            }
            else
            {
                NewsArticles = await _newsService.GetAllActiveNewsAsync();
            }

            // Lecturer role: only show active news (already filtered by GetAllActiveNewsAsync,
            // but enforce here in case search/category returned inactive items)
            if (IsLecturer)
            {
                NewsArticles = NewsArticles.Where(n => n.NewsStatus == true);
            }
        }
    }
}
