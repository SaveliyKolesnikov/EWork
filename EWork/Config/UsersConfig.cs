using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWork.Config
{
    public class UsersConfig
    {
        public IEnumerable<UserConfig> Administrators { get; set; }
        public IEnumerable<UserConfig> Moderators { get; set; }

        public class UserConfig
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
}
