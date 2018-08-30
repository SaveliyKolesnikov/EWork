using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EWork.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Range(0d, 10d, ErrorMessage = "{0} must be in the range 0..10")]
        [Display(Name = "Rating")]
        public double Value { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "{0} length must be in the range 1..20")]
        [Display(Name = "Tag text")]
        public string Text { get; set; }

        public User User { get; set; }

        public User Sender { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime SendDate { get; set; }
    }
}
