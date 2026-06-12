using NguyenBinhAn_A02_Data.Models;
using NguyenBinhAn_A02_Business.Repositories;

namespace NguyenBinhAn_A02_Business.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _tagRepository.GetAllAsync();
        }

        public async Task<Tag?> GetTagByIdAsync(int tagId)
        {
            return await _tagRepository.GetByIdAsync(tagId);
        }

        public async Task<Tag> CreateTagAsync(Tag tag)
        {
            return await _tagRepository.AddAsync(tag);
        }

        public async Task<Tag> UpdateTagAsync(Tag tag)
        {
            await _tagRepository.UpdateAsync(tag);
            return tag;
        }

        public async Task<bool> DeleteTagAsync(int tagId)
        {
            var tag = await _tagRepository.GetByIdAsync(tagId);
            if (tag == null) return false;

            await _tagRepository.DeleteAsync(tag);
            return true;
        }

        public async Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm)
        {
            return await _tagRepository.SearchTagsAsync(searchTerm);
        }

        public async Task<Tag?> GetTagByNameAsync(string tagName)
        {
            return await _tagRepository.GetByNameAsync(tagName);
        }
    }
}
