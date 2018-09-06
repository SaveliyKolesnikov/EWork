using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Config;
using EWork.Data.Interfaces;
using EWork.Exceptions;
using EWork.Models;
using EWork.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EWork.Services
{
    public class BalanceManager : IBalanceManager
    {
        private readonly IBalanceRepository _repository;
        private readonly IOptions<FreelancingPlatformConfig> _freelancingPlatformOptions;

        public BalanceManager(IBalanceRepository repository, IOptions<FreelancingPlatformConfig> freelancingPlatformOptions)
        {
            _repository = repository;
            _freelancingPlatformOptions = freelancingPlatformOptions;
        }

        public Task<Balance> FindAsync(Predicate<Balance> predicate) => _repository.FindAsync(predicate);

        public Task<Balance> GetFreelancingPlatformBalanceAsync() =>
            _repository.FindAsync(b => b.Id == _freelancingPlatformOptions.Value.BalanceId);

        public async Task<bool> TransferMoneyAsync(Balance senderBalance, Balance recipientBalance, decimal amount)
        {
            if (senderBalance is null)
                throw new ArgumentNullException(nameof(senderBalance));

            if (recipientBalance is null)
                throw new ArgumentNullException(nameof(recipientBalance));

            if (amount <= 0)
                throw new ArgumentException("Amount must be more than 0", nameof(amount));
            if (senderBalance.Money < amount)
                throw new NotEnoughMoneyException($"User {senderBalance.User.Name} hasn't got enough money for this operation");

            senderBalance.Money -= amount;
            recipientBalance.Money += amount;
            await _repository.UpdateRangeAsync(new[] {senderBalance, recipientBalance});

            return true;
        }
    }
}
