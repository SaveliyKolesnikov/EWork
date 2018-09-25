using System.ComponentModel.DataAnnotations;

namespace EWork.Models
{
    public class JobTags
    {
        [Required] public int JobId { get; set; }
        [Required] public int TagId { get; set; }

        [Required] public Job Job { get; set; }
        [Required] public Tag Tag { get; set; }
    }
}