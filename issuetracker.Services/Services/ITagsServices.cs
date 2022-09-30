using issuetracker.Entities;

namespace issuetracker.Services;

public interface ITagsServices
{
	Task<IEnumerable<Tag>> GetAllTagsAsync();
	Task<Tag> GetTagByIdAsync(Guid id);
	Task<Tag> CreateTagAsync(Tag tag);
	Task UpdateTagByIdAsync(Guid id, Tag tag);
	Task DeleteTagByIdAsync(Guid id);
}