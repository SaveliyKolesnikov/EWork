using System.ComponentModel.DataAnnotations;

namespace EWork.Models
{
    public class Balance
    {
        public int Id { get; set; }
        [DataType(DataType.Currency)]
        public decimal Money { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
