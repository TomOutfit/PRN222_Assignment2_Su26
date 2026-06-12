using Microsoft.EntityFrameworkCore;
using NguyenBinhAn_A02_Data;
using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAn_A02_Business.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(FUNewsManagementDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive == true)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<bool> HasNewsArticlesAsync(short categoryId)
        {
            return await _context.NewsArticles
                .AnyAsync(n => n.CategoryId == categoryId);
        }

        public async Task<IEnumerable<Category>> GetParentCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == null)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetChildCategoriesAsync(short parentCategoryId)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == parentCategoryId)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryWithChildrenAsync(short categoryId)
        {
            return await _context.Categories
                .Include(c => c.ChildCategories)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }
    }
}
