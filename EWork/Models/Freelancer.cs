using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWork.Models
{
    public class Freelancer : User
    {
        public List<Proposal> Proposals { get; set; }

        public List<FreelancerTags> Tags { get; set; }

        [NotMapped] public override string Role { get; } = "freelancer";
    }
}