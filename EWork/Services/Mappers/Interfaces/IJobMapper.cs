using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;
using EWork.Models.Json;

namespace EWork.Services.Mappers.Interfaces
{
    interface IJobMapper
    {
        JsonJob Map(Job job);
        IEnumerable<JsonJob> MapRange(IEnumerable<Job> jobs);
    }
}
