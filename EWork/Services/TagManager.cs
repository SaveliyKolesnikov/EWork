using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data;
using EWork.Models;
using EWork.Services.Interfaces;

namespace EWork.Services
{
    public class TagManager : ITagManager
    {
        private readonly IFreelancingPlatiformDbContext _db;

        public TagManager(IFreelancingPlatiformDbContext db) => _db = db;

        public async Task<IQueryable<Tag>> AddTagsRangeAsync(IEnumerable<string> inputTags)
        {
            var tagsInDb = _db.Tags;
            var commonTags = tagsInDb.Where(tag => inputTags.Any(t => t == tag.Text));
            var newTagsValues = inputTags.Where(tag => tagsInDb.All(t => tag != t.Text)).Distinct().ToArray();
            var newTags = newTagsValues.Select(tagValue => new Tag { Text = tagValue });
            await _db.Tags.AddRangeAsync(newTags);
            await _db.SaveChangesAsync();

            var newTagsFromBd = _db.Tags.Where(tag => newTagsValues.Contains(tag.Text));
            return commonTags.Union(newTagsFromBd);
        }

        public async Task<IQueryable<Tag>> AddTagsRangeAsync(IEnumerable<Tag> inputTags) =>
            await AddTagsRangeAsync(inputTags.Select(tag => tag.Text));
    }
}