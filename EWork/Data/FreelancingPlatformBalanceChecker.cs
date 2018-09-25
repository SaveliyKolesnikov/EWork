using System.Threading.Tasks;
using EWork.Exceptions;
using EWork.Services.Interfaces;

namespace EWork.Data
{
    public class FreelancingPlatformBalanceChecker
    {
        private readonly IBalanceManager _balanceManager;

        public FreelancingPlatformBalanceChecker(IBalanceManager balanceManager)
            => _balanceManager = balanceManager;

        public async Task CheckAsync()
        {
            var balance = await _balanceManager.GetFreelancingPlatformBalanceAsync();
            if (balance is null)
                throw new DbNotInitializedBalanceException("The freelancing platform balance doesn't exist. Please, add it manually to database through writing its owner username to the config file.");
        }
    }
}
