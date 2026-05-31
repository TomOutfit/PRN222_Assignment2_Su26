using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAn_A02_Business.Repositories
{
    public interface INewsArticleRepository : IGenericRepository<NewsArticle>
    {
        Task<IEnumerable<NewsArticle>> GetAllNewsAsync();
        Task<IEnumerable<NewsArticle>> GetActiveNewsAsync();
        Task<IEnumerable<NewsArticle>> GetNewsByCategoryAsync(short categoryId);
        Task<IEnumerable<NewsArticle>> GetNewsByCreatorAsync(short creatorId);
        Task<IEnumerable<NewsArticle>> SearchNewsAsync(string searchTerm);
        Task<IEnumerable<NewsArticle>> GetNewsInDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<NewsArticle?> GetNewsWithTagsAsync(string newsArticleId);
        Task<string> GenerateNextIdAsync();
        Task UpdateTagsAsync(string newsArticleId, List<int> tagIds);
    }
}
