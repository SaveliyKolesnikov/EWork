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
        Task<IQueryable<Tag>> AddRangeAsync(IEnumerable<string> tags);

        /// <summary>
        /// Add new tags to db which gives them an id.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>Input tags with id from db</returns>
        Task<IQueryable<Tag>> AddRangeAsync(IEnumerable<Tag> tags);

        Task RemoveRangeAsync(IEnumerable<Tag> tags);

        IQueryable<Tag> FindByFirstLetters(string tagStart);
    }
}