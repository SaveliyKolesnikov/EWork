using System.Collections.Generic;
using EWork.Models;
using EWork.Models.Json;

namespace EWork.Services.Mappers.Interfaces
{
    public interface IJobMapper
    {
        JsonJob Map(Job job);
        IEnumerable<JsonJob> MapRange(IEnumerable<Job> jobs);
    }
}
