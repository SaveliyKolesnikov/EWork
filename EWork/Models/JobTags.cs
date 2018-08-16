using System.ComponentModel.DataAnnotations;

namespace EWork.Models
{
    public class JobTags
    {
        public int JobId { get; set; }

        [Required]
        public Job Job { get; set; }

        public int TagId { get; set; }

        [Required]
        public Tag Tag { get; set; }
    }
}
