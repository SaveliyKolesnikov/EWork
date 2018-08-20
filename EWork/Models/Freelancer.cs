using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWork.Models
{
    public class Freelancer : User
    {
        [Required]
        public List<Proposal> Proposals { get; set; }
        [NotMapped]
        public override string Role { get; } = "freelancer";
    }
}
