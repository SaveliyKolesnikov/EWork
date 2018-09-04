using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWork.Models.Json
{
    public class JsonUser
    {
        public JsonUser(string userName, string photoUrl)
        {
            UserName = userName;
            PhotoUrl = photoUrl;
        }

        public string UserName { get; }
        public string PhotoUrl { get; set; }
    }
}
