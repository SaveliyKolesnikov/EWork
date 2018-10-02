using System.ComponentModel.DataAnnotations.Schema;

namespace EWork.Models
{
    public class Employer : User
    {
        public override string Role { get; } = "employer";
    }
}