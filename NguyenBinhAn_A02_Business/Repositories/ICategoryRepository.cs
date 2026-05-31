using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAn_A02_Business.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<bool> HasNewsArticlesAsync(short categoryId);
        Task<IEnumerable<Category>> GetParentCategoriesAsync();
        Task<IEnumerable<Category>> GetChildCategoriesAsync(short parentCategoryId);
        Task<Category?> GetCategoryWithChildrenAsync(short categoryId);
    }
}
