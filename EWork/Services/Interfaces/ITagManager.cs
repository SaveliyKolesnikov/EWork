using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;

namespace EWork.Services.Interfaces
{
    public interface ITagManager
    {
        /// <summary>
        /// Add new tags to db which gives them an id.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>Input tags with id from db</returns>
        Task<IQueryable<Tag>> AddTagsRangeAsync(IEnumerable<string> tags);

        /// <summary>
        /// Add new tags to db which gives them an id.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>Input tags with id from db</returns>
        Task<IQueryable<Tag>> AddTagsRangeAsync(IEnumerable<Tag> tags);

        Task RemoveTagsRangeAsync(IEnumerable<Tag> tags);
    }
}