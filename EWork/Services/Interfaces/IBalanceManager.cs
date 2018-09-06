using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;

namespace EWork.Services.Interfaces
{
    public interface IBalanceManager
    {
        Task<Balance> GetFreelancingPlatformBalanceAsync();

        Task<bool> ReplinishMoneyAsync(Balance senderBalance, Balance recipientBalance, decimal amount);
    }
}
