using System;
using System.ComponentModel.DataAnnotations;

namespace EWork.Models
{
    public class Proposal
    {
        public int Id { get; set; }

        [Required]
        [StringLength(4096, MinimumLength = 30, ErrorMessage = "{0} length must be in the range 1..4096")]
        [Display(Name = "Proposal")]
        public string Text { get; set; }

        public DateTime SendDate { get; set; }
        public Freelancer Sender { get; set; }
        public Job Job { get; set; }
    }
}
