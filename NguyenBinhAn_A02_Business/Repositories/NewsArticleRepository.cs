using Microsoft.EntityFrameworkCore;
using NguyenBinhAn_A02_Data;
using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAn_A02_Business.Repositories
{
    public class NewsArticleRepository : GenericRepository<NewsArticle>, INewsArticleRepository
    {
        public NewsArticleRepository(FUNewsManagementDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<NewsArticle>> GetActiveNewsAsync()
        {
            return await _context.NewsArticles
                .Where(n => n.NewsStatus == true)
                .Include(n => n.Category)
                .Include(n => n.Creator)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsByCategoryAsync(short categoryId)
        {
            return await _context.NewsArticles
                .Where(n => n.CategoryId == categoryId && n.NewsStatus == true)
                .Include(n => n.Category)
                .Include(n => n.Creator)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsByCreatorAsync(short creatorId)
        {
            return await _context.NewsArticles
                .Where(n => n.CreatedById == creatorId)
                .Include(n => n.Category)
                .Include(n => n.Creator)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> SearchNewsAsync(string searchTerm)
        {
            return await _context.NewsArticles
                .Where(n => n.NewsTitle!.Contains(searchTerm) || 
                           n.Headline.Contains(searchTerm) || 
                           n.NewsContent!.Contains(searchTerm))
                .Include(n => n.Category)
                .Include(n => n.Creator)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsInDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.NewsArticles
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .Include(n => n.Category)
                .Include(n => n.Creator)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<NewsArticle?> GetNewsWithTagsAsync(string newsArticleId)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Creator)
                .Include(n => n.Updater)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .FirstOrDefaultAsync(n => n.NewsArticleId == newsArticleId);
        }

        public override async Task<NewsArticle> AddAsync(NewsArticle entity)
        {
            entity.CreatedDate = DateTime.Now;
            await _context.NewsArticles.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public override async Task UpdateAsync(NewsArticle entity)
        {
            entity.ModifiedDate = DateTime.Now;
            _context.NewsArticles.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GenerateNextIdAsync()
        {
            var maxId = await _context.NewsArticles
                .Where(n => n.NewsArticleId.All(char.IsDigit))
                .Select(n => int.Parse(n.NewsArticleId))
                .DefaultIfEmpty(0)
                .MaxAsync();
            return (maxId + 1).ToString();
        }

        public async Task UpdateTagsAsync(string newsArticleId, List<int> tagIds)
        {
            var existingTags = await _context.NewsTags
                .Where(nt => nt.NewsArticleId == newsArticleId)
                .ToListAsync();

            _context.NewsTags.RemoveRange(existingTags);

            if (tagIds != null && tagIds.Any())
            {
                var newsTags = tagIds.Select(tagId => new NewsTag
                {
                    NewsArticleId = newsArticleId,
                    TagId = tagId
                }).ToList();

                await _context.NewsTags.AddRangeAsync(newsTags);
            }

            await _context.SaveChangesAsync();
        }
    }
}
