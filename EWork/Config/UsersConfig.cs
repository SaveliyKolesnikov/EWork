using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace EWork.Config
{
    public class UsersConfig
    {
        public List<UserConfig> Administrators{ get; set; }
        public List<UserConfig> Moderators { get; set; }

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
