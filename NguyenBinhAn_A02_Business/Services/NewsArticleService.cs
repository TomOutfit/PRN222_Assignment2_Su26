using NguyenBinhAn_A02_Data.Models;
using NguyenBinhAn_A02_Business.Repositories;

namespace NguyenBinhAn_A02_Business.Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly ITagRepository _tagRepository;

        public NewsArticleService(INewsArticleRepository newsArticleRepository, ITagRepository tagRepository)
        {
            _newsArticleRepository = newsArticleRepository;
            _tagRepository = tagRepository;
        }

        public async Task<IEnumerable<NewsArticle>> GetAllNewsAsync()
        {
            return await _newsArticleRepository.GetAllNewsAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetAllActiveNewsAsync()
        {
            return await _newsArticleRepository.GetActiveNewsAsync();
        }

        public async Task<NewsArticle?> GetNewsByIdAsync(string newsArticleId)
        {
            return await _newsArticleRepository.GetNewsWithTagsAsync(newsArticleId);
        }

        public async Task<NewsArticle> CreateNewsAsync(NewsArticle newsArticle, List<int> tagIds)
        {
            // Generate unique NewsArticleID via repository
            newsArticle.NewsArticleId = await _newsArticleRepository.GenerateNextIdAsync();

            // Add news article
            var createdNews = await _newsArticleRepository.AddAsync(newsArticle);

            // Add tags via repository
            if (tagIds != null && tagIds.Any())
            {
                await _newsArticleRepository.UpdateTagsAsync(createdNews.NewsArticleId, tagIds);
            }

            return createdNews;
        }

        public async Task<NewsArticle> UpdateNewsAsync(NewsArticle newsArticle, List<int> tagIds)
        {
            // Update tags via repository
            await _newsArticleRepository.UpdateTagsAsync(newsArticle.NewsArticleId, tagIds ?? new List<int>());

            // Update news article
            await _newsArticleRepository.UpdateAsync(newsArticle);
            return newsArticle;
        }

        public async Task<bool> DeleteNewsAsync(string newsArticleId)
        {
            var newsArticle = await _newsArticleRepository.GetByIdAsync(newsArticleId);
            if (newsArticle == null) return false;

            await _newsArticleRepository.DeleteAsync(newsArticle);
            return true;
        }

        public async Task<IEnumerable<NewsArticle>> SearchNewsAsync(string searchTerm)
        {
            return await _newsArticleRepository.SearchNewsAsync(searchTerm);
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsByCategoryAsync(short categoryId)
        {
            return await _newsArticleRepository.GetNewsByCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsHistoryAsync(short accountId)
        {
            return await _newsArticleRepository.GetNewsByCreatorAsync(accountId);
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsStatisticsAsync(DateTime? startDate, DateTime? endDate)
        {
            return await _newsArticleRepository.GetNewsInDateRangeAsync(startDate, endDate);
        }
    }
}
