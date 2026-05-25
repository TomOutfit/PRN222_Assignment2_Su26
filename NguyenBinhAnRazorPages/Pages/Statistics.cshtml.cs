using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenBinhAn_A02_Business.Services;
using NguyenBinhAn_A02_Data.Models;
using System.Text.Json;

namespace NguyenBinhAnRazorPages.Pages
{
    public class StatisticsModel : BasePageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;
        private readonly IAccountService _accountService;

        public StatisticsModel(INewsArticleService newsService, ICategoryService categoryService, IAccountService accountService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
            _accountService = accountService;
        }

        public IEnumerable<NewsArticle> NewsStatistics { get; set; } = new List<NewsArticle>();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        // Summary Statistics
        public int TotalNewsArticles { get; set; }
        public int ActiveNewsArticles { get; set; }
        public int TotalCategories { get; set; }
        public int TotalAccounts { get; set; }
        
        // Chart Data
        public string CategoryLabels { get; set; } = "[]";
        public string CategoryData { get; set; } = "[]";
        public string CreatorLabels { get; set; } = "[]";
        public string CreatorData { get; set; } = "[]";

        public async Task OnGetAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            StartDate = startDate;
            EndDate = endDate;

            // Load ALL news for summary (both active and inactive for admin view)
            var allNews = await _newsService.GetAllNewsAsync();
            TotalNewsArticles = allNews.Count();
            ActiveNewsArticles = allNews.Count(n => n.NewsStatus == true);
            
            var allCategories = await _categoryService.GetAllCategoriesAsync();
            TotalCategories = allCategories.Count();
            
            var allAccounts = await _accountService.GetAllAccountsAsync();
            TotalAccounts = allAccounts.Count();

            // Load news statistics - if date range specified, filter by it, otherwise show all
            if (startDate.HasValue && endDate.HasValue)
            {
                NewsStatistics = await _newsService.GetNewsStatisticsAsync(startDate.Value, endDate.Value);
            }
            else
            {
                // Show all news when no date filter
                NewsStatistics = allNews;
            }

            // Generate chart data
            GenerateChartData();
        }

        private void GenerateChartData()
        {
            // Category Chart Data
            var categoryStats = NewsStatistics
                .Where(n => n.Category != null)
                .GroupBy(n => n.Category!.CategoryName)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            CategoryLabels = JsonSerializer.Serialize(categoryStats.Select(c => c.Category));
            CategoryData = JsonSerializer.Serialize(categoryStats.Select(c => c.Count));

            // Creator Chart Data
            var creatorStats = NewsStatistics
                .Where(n => n.Creator != null)
                .GroupBy(n => n.Creator!.AccountName)
                .Select(g => new { Creator = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToList();

            CreatorLabels = JsonSerializer.Serialize(creatorStats.Select(c => c.Creator));
            CreatorData = JsonSerializer.Serialize(creatorStats.Select(c => c.Count));
        }
    }
}
