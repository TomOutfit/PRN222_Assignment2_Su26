using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenBinhAn_A02_Business.Services;
using NguyenBinhAn_A02_Data.Models;
using System.Text.Json;
using ClosedXML.Excel;

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

        public async Task<IActionResult> OnGetExportExcelAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var news = await GetFilteredNewsAsync(startDate, endDate);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("News Report");

            // ── Title row ──
            var titleCell = ws.Cell(1, 1);
            titleCell.Value = "News Report";
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontSize = 16;
            titleCell.Style.Font.FontColor = XLColor.FromHtml("#1e3a5f");
            ws.Range(1, 1, 1, 6).Merge();

            // ── Date range row ──
            var rangeLabel = startDate.HasValue || endDate.HasValue
                ? $"Period: {startDate?.ToString("dd/MM/yyyy") ?? "All"} – {endDate?.ToString("dd/MM/yyyy") ?? "Present"}"
                : "Period: All Time";
            ws.Cell(2, 1).Value = rangeLabel;
            ws.Cell(2, 1).Style.Font.Italic = true;
            ws.Cell(2, 1).Style.Font.FontColor = XLColor.FromHtml("#6b7280");
            ws.Range(2, 1, 2, 6).Merge();

            // ── Generated row ──
            ws.Cell(3, 1).Value = $"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Cell(3, 1).Style.Font.Italic = true;
            ws.Cell(3, 1).Style.Font.FontColor = XLColor.FromHtml("#9ca3af");
            ws.Range(3, 1, 3, 6).Merge();

            // ── Header row (row 5) ──
            string[] headers = { "#", "Article ID", "Title", "Category", "Creator", "Created Date", "Status" };
            for (int col = 1; col <= headers.Length; col++)
            {
                var cell = ws.Cell(5, col);
                cell.Value = headers[col - 1];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e3a5f");
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#93c5fd");
            }

            // ── Data rows ──
            var newsList = news.ToList();
            for (int i = 0; i < newsList.Count; i++)
            {
                var item = newsList[i];
                int row = i + 6;
                bool isEven = i % 2 == 1;

                var rowBg = isEven ? XLColor.FromHtml("#f0f4ff") : XLColor.White;

                ws.Cell(row, 1).Value = i + 1;
                ws.Cell(row, 2).Value = item.NewsArticleId ?? "";
                ws.Cell(row, 3).Value = item.NewsTitle ?? "";
                ws.Cell(row, 4).Value = item.Category?.CategoryName ?? "";
                ws.Cell(row, 5).Value = item.Creator?.AccountName ?? "";
                ws.Cell(row, 6).Value = item.CreatedDate?.ToString("dd/MM/yyyy HH:mm") ?? "";

                var statusCell = ws.Cell(row, 7);
                bool isActive = item.NewsStatus == true;
                statusCell.Value = isActive ? "Active" : "Inactive";
                statusCell.Style.Font.Bold = true;
                statusCell.Style.Font.FontColor = isActive ? XLColor.FromHtml("#166534") : XLColor.FromHtml("#991b1b");
                statusCell.Style.Fill.BackgroundColor = isActive ? XLColor.FromHtml("#dcfce7") : XLColor.FromHtml("#fee2e2");
                statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                for (int col = 1; col <= 7; col++)
                {
                    var cell = ws.Cell(row, col);
                    if (col != 7) cell.Style.Fill.BackgroundColor = rowBg;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#e5e7eb");
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                }
            }

            // ── Summary row ──
            int summaryRow = newsList.Count + 6;
            ws.Cell(summaryRow, 1).Value = $"Total: {newsList.Count} articles";
            ws.Cell(summaryRow, 1).Style.Font.Bold = true;
            ws.Cell(summaryRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#dbeafe");
            ws.Range(summaryRow, 1, summaryRow, 7).Merge();

            // ── Column widths ──
            ws.Column(1).Width = 6;   // #
            ws.Column(2).Width = 16;  // Article ID
            ws.Column(3).Width = 45;  // Title
            ws.Column(4).Width = 22;  // Category
            ws.Column(5).Width = 22;  // Creator
            ws.Column(6).Width = 18;  // Created Date
            ws.Column(7).Width = 12;  // Status
            ws.Row(5).Height = 22;

            // ── Freeze header ──
            ws.SheetView.FreezeRows(5);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"NewsReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
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
