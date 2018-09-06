using System.Data.Common;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Data.Interfaces;
using EWork.Exceptions;
using EWork.Models;
using Microsoft.Extensions.Options;

namespace EWork.Data
{
    public class FreelancingPlatformBalanceChecker
    {
        private readonly IBalanceRepository _repository;
        private readonly IOptions<FreelancingPlatformConfig> _freelancingPlatformOptions;

        public FreelancingPlatformBalanceChecker(IBalanceRepository repository,
            IOptions<FreelancingPlatformConfig> freelancingPlatformOptions)
        {
            _repository = repository;
            _freelancingPlatformOptions = freelancingPlatformOptions;
        }

        public async Task CheckAsync()
        {
            var balanceId = _freelancingPlatformOptions.Value.BalanceId;
            if (await _repository.FindAsync(b => b.Id == balanceId) is null)
                throw new DbNotInitializedBalanceException("A freelancing platform balance doesn't exist. Please, add it manually to database and write its id to the config file.");
        }
    }
}
