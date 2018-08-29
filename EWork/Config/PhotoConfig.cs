using System.Collections.Generic;

namespace EWork.Config
{
    public class PhotoConfig
    {
        public long MaxSize { get; set; }
        public string DefaultPhoto { get; set; }
        public string UsersPhotosPath { get; set; }
        public IEnumerable<string> AllowedExtensions { get; set; }
    }
}
