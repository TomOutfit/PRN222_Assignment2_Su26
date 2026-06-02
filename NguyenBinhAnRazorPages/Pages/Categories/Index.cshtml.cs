using System;
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

        public int PageSize { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
        public int StartRecord { get; set; } = 1;
        public int EndRecord { get; set; } = 5;

        public async Task OnGetAsync(string searchTerm = "", int page = 1)
        {
            SearchTerm = searchTerm ?? string.Empty;
            CurrentPage = page < 1 ? 1 : page;

            // Load all categories
            var allCategories = (await _categoryService.GetAllCategoriesAsync()).OrderBy(c => c.CategoryId).ToList();

            // Load parent categories for dropdown
            ParentCategories = allCategories.Where(c => c.ParentCategoryId == null);

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                allCategories = allCategories.Where(c =>
                    c.CategoryName.Contains(SearchTerm) ||
                    c.CategoryDesciption.Contains(SearchTerm)).ToList();
            }

            TotalRecords = allCategories.Count;
            TotalPages = (int)Math.Ceiling((double)TotalRecords / PageSize);
            if (CurrentPage > TotalPages) CurrentPage = TotalPages > 0 ? TotalPages : 1;

            StartRecord = TotalRecords > 0 ? (CurrentPage - 1) * PageSize + 1 : 0;
            EndRecord = Math.Min(CurrentPage * PageSize, TotalRecords);

            Categories = allCategories.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
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
