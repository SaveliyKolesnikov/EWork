using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EWork.Models
{
    public class Freelancer : User
    {
        [Required]
        public List<Offer> Offers { get; set; }
    }
}
