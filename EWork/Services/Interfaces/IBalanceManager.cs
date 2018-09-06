using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;

namespace EWork.Services.Interfaces
{
    public interface IBalanceManager
    {
        Task<Balance> FindAsync(Predicate<Balance> predicate);

        Task<Balance> GetFreelancingPlatformBalanceAsync();

        Task<bool> ReplenishBalanceAsync(Balance balance, decimal amount);

        Task<bool> TransferMoneyAsync(Balance senderBalance, Balance recipientBalance, decimal amount);
    }
}
