using NguyenBinhAn_A02_Data.Models;
using NguyenBinhAn_A02_Business.Repositories;

namespace NguyenBinhAn_A02_Business.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(short categoryId);
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(short categoryId);
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<bool> CanDeleteCategoryAsync(short categoryId);
    }
}
