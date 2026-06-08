using Microsoft.EntityFrameworkCore;
using NguyenBinhAn_A02_Data;
using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAn_A02_Business.Repositories
{
    public class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        public TagRepository(FUNewsManagementDbContext context) : base(context)
        {
        }

        public async Task<Tag?> GetByNameAsync(string tagName)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(t => t.TagName == tagName);
        }

        public async Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm)
        {
            return await _context.Tags
                .Where(t => t.TagName!.Contains(searchTerm) || t.Note!.Contains(searchTerm))
                .OrderBy(t => t.TagName)
                .ToListAsync();
        }
    }
}
