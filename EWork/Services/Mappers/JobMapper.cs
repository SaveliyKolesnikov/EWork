﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;
using EWork.Models.Json;
using EWork.Services.Mappers.Interfaces;

namespace EWork.Services.Mappers
{
    public class JobMapper : IJobMapper
    {
        public JsonJob Map(Job job) => 
            new JsonJob
            {
                Title = job.Title,
                Description = job.Description,
                Budget = job.Budget,
                CreationDate = job.CreationDate,
                Tags = string.Join(' ', job.JobTags.Select(jt => jt.Tag.Text)),
                EmployerRating = job.Employer.Reviews.Count == 0 ? 0d : job.Employer.Reviews.Average(r => r.Value)
            };

        public IEnumerable<JsonJob> MapRange(IEnumerable<Job> jobs) => jobs.Select(Map);
    }
}
