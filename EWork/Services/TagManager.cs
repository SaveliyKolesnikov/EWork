using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Data.Interfaces;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EWork.Services
{
    public class TagManager : ITagManager
    {
        private readonly IFreelancingPlatformDbContext _db;

        public TagManager(IFreelancingPlatformDbContext db) => _db = db;

        public async Task<IQueryable<Tag>> AddRangeAsync(IEnumerable<string> tags)
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

        public Task<IQueryable<Tag>> AddRangeAsync(IEnumerable<Tag> tags) =>
            AddRangeAsync(tags.Select(tag => tag.Text));

        public async Task RemoveRangeAsync(IEnumerable<Tag> tags)
        {
            var notUsedTags = tags.Where(tag => _db.Jobs.All(j => j.JobTags.All(jt => jt.Tag.Id != tag.Id)) &&
                                                           _db.Freelancers.All(f => f.Tags.All(ft => ft.Tag.Id != tag.Id)));

            _db.Tags.RemoveRange(notUsedTags);
            await _db.SaveChangesAsync();
        }

        public IQueryable<Tag> FindByFirstLetters(string tagStart) =>
            _db.Tags.Where(tag => tag.Text.StartsWith(tagStart));
    }
}