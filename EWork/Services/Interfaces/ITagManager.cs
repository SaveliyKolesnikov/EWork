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
        /// <param name="inputTags"></param>
        /// <returns>Input tags with id from db</returns>
        Task<IQueryable<Tag>> AddTagsRangeAsync(IEnumerable<string> inputTags);

        /// <summary>
        /// Add new tags to db which gives them an id.
        /// </summary>
        /// <param name="inputTags"></param>
        /// <returns>Input tags with id from db</returns>
        Task<IQueryable<Tag>> AddTagsRangeAsync(IEnumerable<Tag> inputTags);
    }
}