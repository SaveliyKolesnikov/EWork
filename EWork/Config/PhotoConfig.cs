using System.Collections.Generic;

namespace EWork.Config
{
    public class PhotoConfig
    {
        public string DefaultPhoto { get; set; }
        public string UsersPhotosPath { get; set; }
        public IEnumerable<string> AllowedExtensions { get; set; }
    }
}
