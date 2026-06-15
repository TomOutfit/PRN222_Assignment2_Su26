using Microsoft.AspNetCore.Mvc;
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
        public string StatusLabels { get; set; } = "[]";
        public string StatusData { get; set; } = "[]";
        public string TrendLabels { get; set; } = "[]";
        public string TrendData { get; set; } = "[]";

        private async Task<IEnumerable<NewsArticle>> GetFilteredNewsAsync(DateTime? startDate, DateTime? endDate)
        {
            IEnumerable<NewsArticle> news;
            if (startDate.HasValue || endDate.HasValue)
            {
                news = await _newsService.GetNewsStatisticsAsync(startDate, endDate);
            }
            else
            {
                news = await _newsService.GetAllNewsAsync();
            }

            if (IsStaff && !IsAdmin)
            {
                return news.Where(n => n.CreatedById == AccountId).ToList();
            }
            return news.ToList();
        }

        public async Task OnGetAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            OnPageAuthorization();
            StartDate = startDate;
            EndDate = endDate;

            // Load filtered news for the report
            NewsStatistics = await GetFilteredNewsAsync(startDate, endDate);
            
            TotalNewsArticles = NewsStatistics.Count();
            ActiveNewsArticles = NewsStatistics.Count(n => n.NewsStatus == true);
            
            var allCategories = await _categoryService.GetAllCategoriesAsync();
            TotalCategories = allCategories.Count();
            
            var allAccounts = await _accountService.GetAllAccountsAsync();
            TotalAccounts = allAccounts.Count();

            // Generate chart data
            GenerateChartData();
        }

        public async Task<IActionResult> OnGetExportCsvAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var news = await GetFilteredNewsAsync(startDate, endDate);
            
            var csvBuilder = new System.Text.StringBuilder();
            // Add UTF-8 BOM so Excel opens it correctly with Vietnamese characters
            csvBuilder.Append('\uFEFF');
            csvBuilder.AppendLine("ID,Title,Category,Creator,Created Date,Status");

            foreach (var item in news)
            {
                var id = EscapeCsv(item.NewsArticleId);
                var title = EscapeCsv(item.NewsTitle ?? "");
                var category = EscapeCsv(item.Category?.CategoryName ?? "");
                var creator = EscapeCsv(item.Creator?.AccountName ?? "");
                var createdDate = item.CreatedDate?.ToString("dd/MM/yyyy HH:mm") ?? "";
                var status = item.NewsStatus == true ? "Active" : "Inactive";

                csvBuilder.AppendLine($"{id},{title},{category},{creator},{createdDate},{status}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return File(bytes, "text/csv", $"NewsReport_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }

        public async Task<IActionResult> OnGetExportJsonAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var news = await GetFilteredNewsAsync(startDate, endDate);
            
            var exportData = news.Select(n => new
            {
                n.NewsArticleId,
                n.NewsTitle,
                n.Headline,
                CategoryName = n.Category?.CategoryName,
                CreatorName = n.Creator?.AccountName,
                CreatedDate = n.CreatedDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                Status = n.NewsStatus == true ? "Active" : "Inactive"
            });

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(exportData, options);
            return File(jsonBytes, "application/json", $"NewsReport_{DateTime.Now:yyyyMMddHHmmss}.json");
        }

        private string EscapeCsv(string val)
        {
            if (val.Contains(",") || val.Contains("\"") || val.Contains("\n") || val.Contains("\r"))
            {
                return $"\"{val.Replace("\"", "\"\"")}\"";
            }
            return val;
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

            // Status Chart Data
            var activeCount = NewsStatistics.Count(n => n.NewsStatus == true);
            var inactiveCount = NewsStatistics.Count(n => n.NewsStatus != true);
            StatusLabels = JsonSerializer.Serialize(new[] { "Active", "Inactive" });
            StatusData = JsonSerializer.Serialize(new[] { activeCount, inactiveCount });

            // Trend Chart Data
            var trendStats = NewsStatistics
                .Where(n => n.CreatedDate.HasValue)
                .GroupBy(n => n.CreatedDate!.Value.ToString("yyyy-MM-dd"))
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(g => g.Date)
                .ToList();

            TrendLabels = JsonSerializer.Serialize(trendStats.Select(t => t.Date));
            TrendData = JsonSerializer.Serialize(trendStats.Select(t => t.Count));
        }
    }
}
