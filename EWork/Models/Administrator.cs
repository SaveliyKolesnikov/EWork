namespace EWork.Models
{
    public class Administrator : User
    {
        public override string Role { get; } = "administrator";
    }
}
