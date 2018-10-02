using System.Threading.Tasks;
using EWork.Models;

namespace EWork.Services.Interfaces
{
    public interface IJobRecommender
    {
        Task RecommendAsync(Job job);
    }
}