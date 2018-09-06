using System;
using System.Linq;
using EWork.Services.Interfaces;

namespace EWork.Services
{
    public class RandomStringGenerator : IRandomStringGenerator
    {
        private readonly Random _random = new Random(DateTime.Now.Millisecond);

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
