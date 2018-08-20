using System.ComponentModel.DataAnnotations.Schema;

namespace EWork.Models
{
    public class Employer : User
    {
        [NotMapped]
        public override string Role { get; } = "employer";
    }
}
