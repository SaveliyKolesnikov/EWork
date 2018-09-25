using System.Threading.Tasks;
using EWork.Config;
using EWork.Exceptions;
using EWork.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EWork.Data
{
    public class FreelancingPlatformBalanceChecker
    {
        private readonly IBalanceManager _balanceManager;
        private readonly IOptions<FreelancingPlatformConfig> _freelancingPlatformOptions;

        public FreelancingPlatformBalanceChecker(IBalanceManager balanceManager,
            IOptions<FreelancingPlatformConfig> freelancingPlatformOptions)
        {
            _balanceManager = balanceManager;
            _freelancingPlatformOptions = freelancingPlatformOptions;
        }

        public async Task CheckAsync()
        {
            var freelancingPlatformBalance = await _balanceManager.GetFreelancingPlatformBalanceAsync();
            if (freelancingPlatformBalance is null)
                throw new DbNotInitializedBalanceException("A freelancing platform balance doesn't exist. Please, add it manually to database by writing its owner username to the config file.");
        }
    }
}
