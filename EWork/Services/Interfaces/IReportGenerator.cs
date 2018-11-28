using System.Collections.Generic;
using EWork.Models;

namespace EWork.Services.Interfaces
{
    public interface IReportGenerator
    {
        byte[] JobsReport(User receiver, IEnumerable<Job> jobs);
    }
}