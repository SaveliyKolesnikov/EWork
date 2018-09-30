using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;
using EWork.Services.Interfaces;

namespace EWork.Services
{
    public class TagManager : ITagManager
    {
        private readonly IFreelancingPlatiformDbContext _db;

        public TagManager(IFreelancingPlatiformDbContext db) => _db = db;

        public async Task<IQueryable<Tag>> AddTagsRangeAsync(IEnumerable<string> tags)
        {
            if (tags is null)
                throw new ArgumentNullException(nameof(tags));

            var inputTagsEnum = tags as string[] ?? tags.ToArray();
            if (inputTagsEnum.FirstOrDefault() is null)
                return Enumerable.Empty<Tag>().AsQueryable();

            var tagsInDb = _db.Tags;
            var commonTags = tagsInDb.Where(tag => inputTagsEnum.Any(t => t == tag.Text));
            var newTagsValues = inputTagsEnum.Where(tag => tagsInDb.All(t => tag != t.Text)).Distinct().ToArray();
            var newTags = newTagsValues.Select(tagValue => new Tag { Text = tagValue });
            await _db.Tags.AddRangeAsync(newTags);
            await _db.SaveChangesAsync();

            var newTagsFromBd = _db.Tags.Where(tag => newTagsValues.Contains(tag.Text));
            return commonTags.Union(newTagsFromBd);
        }

        public Task<IQueryable<Tag>> AddTagsRangeAsync(IEnumerable<Tag> tags) =>
            AddTagsRangeAsync(tags.Select(tag => tag.Text));

        public async Task RemoveTagsRangeAsync(IEnumerable<Tag> tags)
        {
            var notUsedTags = tags.Where(tag => _db.Jobs.All(j => j.JobTags.All(jt => jt.Tag.Id != tag.Id)) &&
                                                           _db.Freelancers.All(f => f.Tags.All(ft => ft.Tag.Id != tag.Id)));

            _db.Tags.RemoveRange(notUsedTags);
            await _db.SaveChangesAsync();
        }
    }
}