using System.ComponentModel.DataAnnotations;

namespace EWork.Models
{
    public class FreelancerTags
    {
        [Required] public string FreelancerId { get; set; }
        [Required] public int TagId { get; set; }

        [Required] public Freelancer Freelancer { get; set; }
        [Required] public Tag Tag { get; set; }
    }
}