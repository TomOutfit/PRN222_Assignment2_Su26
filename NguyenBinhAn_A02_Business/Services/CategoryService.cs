using NguyenBinhAn_A02_Data.Models;
using NguyenBinhAn_A02_Business.Repositories;
namespace NguyenBinhAn_A02_Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(short categoryId)
        {
            return await _categoryRepository.GetByIdAsync(categoryId);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            return await _categoryRepository.AddAsync(category);
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            await _categoryRepository.UpdateAsync(category);
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(short categoryId)
        {
            if (!await CanDeleteCategoryAsync(categoryId))
                return false;

            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null) return false;

            await _categoryRepository.DeleteAsync(category);
            return true;
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _categoryRepository.GetActiveCategoriesAsync();
        }

        public async Task<bool> CanDeleteCategoryAsync(short categoryId)
        {
            if (await _categoryRepository.HasNewsArticlesAsync(categoryId))
                return false;

            var category = await _categoryRepository.GetCategoryWithChildrenAsync(categoryId);
            if (category != null && category.ChildCategories.Any())
                return false;

            return true;
        }
    }
}
