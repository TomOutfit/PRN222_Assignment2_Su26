using System;
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

        public int PageSize { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
        public int StartRecord { get; set; } = 1;
        public int EndRecord { get; set; } = 5;

        public async Task OnGetAsync(string searchTerm = "", int page = 1)
        {
            if (!AccountId.HasValue) return;

            SearchTerm = searchTerm ?? string.Empty;
            CurrentPage = page < 1 ? 1 : page;

            IEnumerable<NewsArticle> allNews;
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                allNews = (await _newsService.GetNewsHistoryAsync((short)AccountId.Value))
                    .Where(n => n.NewsTitle!.Contains(SearchTerm) ||
                                n.Headline.Contains(SearchTerm) ||
                                n.NewsContent!.Contains(SearchTerm))
                    .OrderByDescending(n => n.CreatedDate)
                    .ToList();
            }
            else
            {
                allNews = (await _newsService.GetNewsHistoryAsync((short)AccountId.Value))
                    .OrderByDescending(n => n.CreatedDate)
                    .ToList();
            }

            TotalRecords = allNews.Count();
            TotalPages = (int)Math.Ceiling((double)TotalRecords / PageSize);
            if (CurrentPage > TotalPages) CurrentPage = TotalPages > 0 ? TotalPages : 1;

            StartRecord = TotalRecords > 0 ? (CurrentPage - 1) * PageSize + 1 : 0;
            EndRecord = Math.Min(CurrentPage * PageSize, TotalRecords);

            NewsArticles = allNews.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
        }
    }
}
