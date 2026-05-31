using NguyenBinhAn_A02_Data.Models;
using NguyenBinhAn_A02_Business.Repositories;

namespace NguyenBinhAn_A02_Business.Services
{
    public interface INewsArticleService
    {
        Task<IEnumerable<NewsArticle>> GetAllNewsAsync();
        Task<IEnumerable<NewsArticle>> GetAllActiveNewsAsync();
        Task<NewsArticle?> GetNewsByIdAsync(string newsArticleId);
        Task<NewsArticle> CreateNewsAsync(NewsArticle newsArticle, List<int> tagIds);
        Task<NewsArticle> UpdateNewsAsync(NewsArticle newsArticle, List<int> tagIds);
        Task<bool> DeleteNewsAsync(string newsArticleId);
        Task<IEnumerable<NewsArticle>> SearchNewsAsync(string searchTerm);
        Task<IEnumerable<NewsArticle>> GetNewsByCategoryAsync(short categoryId);
        Task<IEnumerable<NewsArticle>> GetNewsHistoryAsync(short accountId);
        Task<IEnumerable<NewsArticle>> GetNewsStatisticsAsync(DateTime startDate, DateTime endDate);
    }
}
