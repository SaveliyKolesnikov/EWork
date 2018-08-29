using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWork.Services.Interfaces
{
    public interface IRandomStringGenerator
    {
        string RandomString(int length);
    }
}
