using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EWork.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "{0} length must be in the range 1..20")]
        [Display(Name="Tag text")]
        public string Text { get; set; }
        public List<JobTags> JobTags { get; set; }
    }
}
