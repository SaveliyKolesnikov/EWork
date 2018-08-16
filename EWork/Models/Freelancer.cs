using System.Collections.Generic;

namespace EWork.Models
{
    public class Freelancer : User
    {
        public List<Offer> Offers { get; set; }
    }
}
