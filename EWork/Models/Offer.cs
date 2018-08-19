using System.ComponentModel.DataAnnotations;

namespace EWork.Models
{
    public class Offer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(4096, MinimumLength = 30, ErrorMessage = "{0} length must be in the range 1..20")]
        [Display(Name = "Offer")]
        public string Text { get; set; }

        public Freelancer Sender { get; set; }

        public Job Job { get; set; }
    }
}
