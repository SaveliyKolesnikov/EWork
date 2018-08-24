using System.ComponentModel.DataAnnotations;

namespace EWork.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        [StringLength(4096, MinimumLength = 1, ErrorMessage = "{0} length must be less then 4096")]
        public string Text { get; set; }

        [Required]
        public User Sender { get; set; }

        [Required]
        public User Receiver { get; set; }

    }
}
