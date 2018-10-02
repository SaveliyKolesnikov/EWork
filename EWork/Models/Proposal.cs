using System;
using System.ComponentModel.DataAnnotations;

namespace EWork.Models
{
    public class Proposal
    {
        public int Id { get; set; }

        [Required]
        [StringLength(4096, MinimumLength = 30, ErrorMessage = "{0} length must be in the range 30..4096")]
        [Display(Name = "Proposal")]
        public string Text { get; set; }

        [Required] public DateTime SendDate { get; set; }
        [Required] public Freelancer Sender { get; set; }
        [Required] public Job Job { get; set; }
    }
}
