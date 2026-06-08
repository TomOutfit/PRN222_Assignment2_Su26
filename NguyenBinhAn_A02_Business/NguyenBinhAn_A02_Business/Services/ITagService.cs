using NguyenBinhAn_A02_Data.Models;
using NguyenBinhAn_A02_Business.Repositories;

namespace NguyenBinhAn_A02_Business.Services
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetAllTagsAsync();
        Task<Tag?> GetTagByIdAsync(int tagId);
        Task<Tag> CreateTagAsync(Tag tag);
        Task<Tag> UpdateTagAsync(Tag tag);
        Task<bool> DeleteTagAsync(int tagId);
        Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm);
        Task<Tag?> GetTagByNameAsync(string tagName);
    }
}
