using NguyenBinhAn_A02_Data.Models;

namespace NguyenBinhAn_A02_Business.Repositories
{
    public interface ITagRepository : IGenericRepository<Tag>
    {
        Task<Tag?> GetByNameAsync(string tagName);
        Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm);
    }
}
