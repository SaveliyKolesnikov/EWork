using System.ComponentModel.DataAnnotations;

namespace EWork.Models
{
    public class Balance
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Money { get; set; }

        [Required] public string UserId { get; set; }

        [Required] public User User { get; set; }
    }
}