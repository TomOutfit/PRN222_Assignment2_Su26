using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenBinhAn_A02_Business.Services;
using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAnRazorPages.Pages.Categories
{
    public class IndexModel : BasePageModel
    {
        private readonly ICategoryService _categoryService;

        public IndexModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Category> ParentCategories { get; set; } = new List<Category>();
        public string SearchTerm { get; set; } = string.Empty;

        public async Task OnGetAsync(string searchTerm = "")
        {
            SearchTerm = searchTerm ?? string.Empty;
            
            // Load all categories
            Categories = await _categoryService.GetAllCategoriesAsync();
            
            // Load parent categories for dropdown
            ParentCategories = Categories.Where(c => c.ParentCategoryId == null);

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                Categories = Categories.Where(c => 
                    c.CategoryName.Contains(SearchTerm) || 
                    c.CategoryDesciption.Contains(SearchTerm));
            }
        }

        public async Task<JsonResult> OnPostCreateAsync(Category category)
        {
            try
            {
                if (!IsStaff)
                {
                    return new JsonResult(new { success = false, message = "Access denied. Only Staff can manage categories." });
                }

                var createdCategory = await _categoryService.CreateCategoryAsync(category);
                return new JsonResult(new { success = true, message = "Category created successfully" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostUpdateAsync(Category category)
        {
            try
            {
                if (!IsStaff)
                {
                    return new JsonResult(new { success = false, message = "Access denied. Only Staff can manage categories." });
                }

                await _categoryService.UpdateCategoryAsync(category);
                return new JsonResult(new { success = true, message = "Category updated successfully" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostDeleteAsync(short id)
        {
            try
            {
                if (!IsStaff)
                {
                    return new JsonResult(new { success = false, message = "Access denied. Only Staff can manage categories." });
                }

                var result = await _categoryService.DeleteCategoryAsync(id);
                if (result)
                {
                    return new JsonResult(new { success = true, message = "Category deleted successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Cannot delete category. It may be associated with news articles or have child categories." });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
