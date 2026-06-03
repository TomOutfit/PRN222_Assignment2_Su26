using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using NguyenBinhAn_A02_Business.Services;
using NguyenBinhAn_A02_Data.Models;
using NguyenBinhAnRazorPages.Hubs;

namespace NguyenBinhAnRazorPages.Pages.News
{
    public class IndexModel : BasePageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly IHubContext<NewsHub> _hubContext;

        public IndexModel(INewsArticleService newsService, ICategoryService categoryService, ITagService tagService, IHubContext<NewsHub> hubContext)
        {
            _newsService = newsService;
            _categoryService = categoryService;
            _tagService = tagService;
            _hubContext = hubContext;
        }

        public IEnumerable<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Tag> Tags { get; set; } = new List<Tag>();
        public string SearchTerm { get; set; } = string.Empty;

        public int PageSize { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
        public int StartRecord { get; set; } = 1;
        public int EndRecord { get; set; } = 5;

        public string? EditId { get; set; }

        public async Task OnGetAsync(string searchTerm = "", int pageIndex = 1, string? editId = null)
        {
            EditId = editId;
            OnPageAuthorization();
            SearchTerm = searchTerm ?? string.Empty;
            CurrentPage = pageIndex < 1 ? 1 : pageIndex;

            // Load categories and tags for dropdowns
            Categories = await _categoryService.GetActiveCategoriesAsync();
            Tags = await _tagService.GetAllTagsAsync();

            // Load news articles
            IEnumerable<NewsArticle> allNews;
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                allNews = (await _newsService.SearchNewsAsync(SearchTerm)).OrderByDescending(n => n.CreatedDate);
            }
            else
            {
                allNews = (await _newsService.GetAllActiveNewsAsync()).OrderByDescending(n => n.CreatedDate);
            }

            TotalRecords = allNews.Count();
            TotalPages = (int)Math.Ceiling((double)TotalRecords / PageSize);
            if (CurrentPage > TotalPages) CurrentPage = TotalPages > 0 ? TotalPages : 1;

            StartRecord = TotalRecords > 0 ? (CurrentPage - 1) * PageSize + 1 : 0;
            EndRecord = Math.Min(CurrentPage * PageSize, TotalRecords);

            NewsArticles = allNews.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
        }

        public async Task<JsonResult> OnGetEditAsync(string id)
        {
            try
            {
                var news = await _newsService.GetNewsByIdAsync(id);
                if (news == null)
                {
                    return new JsonResult(new { success = false, message = "News article not found" });
                }

                var newsData = new
                {
                    success = true,
                    news = new
                    {
                        newsArticleId = news.NewsArticleId,
                        newsTitle = news.NewsTitle,
                        headline = news.Headline,
                        newsContent = news.NewsContent,
                        newsSource = news.NewsSource,
                        categoryId = news.CategoryId,
                        newsStatus = news.NewsStatus,
                        tags = news.NewsTags.Select(nt => nt.TagId).ToArray()
                    }
                };

                return new JsonResult(newsData);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostCreateAsync(NewsArticle newsArticle, List<int> TagIds)
        {
            try
            {
                if (!IsStaff && !IsAdmin)
                {
                    return new JsonResult(new { success = false, message = "Access denied" });
                }

                // Set creator
                if (AccountId.HasValue)
                {
                    newsArticle.CreatedById = (short)AccountId.Value;
                }

                var createdNews = await _newsService.CreateNewsAsync(newsArticle, TagIds);

                // Load full news with navigation properties for SignalR
                var fullNews = await _newsService.GetNewsByIdAsync(createdNews.NewsArticleId);
                await _hubContext.Clients.All.SendAsync("NewsCreated", fullNews);

                return new JsonResult(new { success = true, message = "News created successfully" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostUpdateAsync(NewsArticle newsArticle, List<int> TagIds)
        {
            try
            {
                if (!IsStaff && !IsAdmin)
                {
                    return new JsonResult(new { success = false, message = "Access denied" });
                }

                // Retrieve existing news article to preserve CreatedDate and CreatedById
                var existingNews = await _newsService.GetNewsByIdAsync(newsArticle.NewsArticleId);
                if (existingNews == null)
                {
                    return new JsonResult(new { success = false, message = "News article not found" });
                }

                // Update fields
                existingNews.NewsTitle = newsArticle.NewsTitle;
                existingNews.Headline = newsArticle.Headline;
                existingNews.NewsContent = newsArticle.NewsContent;
                existingNews.NewsSource = newsArticle.NewsSource;
                existingNews.CategoryId = newsArticle.CategoryId;
                existingNews.NewsStatus = newsArticle.NewsStatus;

                // Set updater
                if (AccountId.HasValue)
                {
                    existingNews.UpdatedById = (short)AccountId.Value;
                }

                await _newsService.UpdateNewsAsync(existingNews, TagIds);

                // Load full news with navigation properties for SignalR
                var fullNews = await _newsService.GetNewsByIdAsync(existingNews.NewsArticleId);
                await _hubContext.Clients.All.SendAsync("NewsUpdated", fullNews);

                return new JsonResult(new { success = true, message = "News updated successfully" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostDeleteAsync(string id)
        {
            try
            {
                if (!IsStaff && !IsAdmin)
                {
                    return new JsonResult(new { success = false, message = "Access denied" });
                }

                var result = await _newsService.DeleteNewsAsync(id);
                if (result)
                {
                    await _hubContext.Clients.All.SendAsync("NewsDeleted", id);
                    return new JsonResult(new { success = true, message = "News deleted successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "News article not found" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
