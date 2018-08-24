namespace EWork.Models
{
    public class Moderator : User
    {
        public override string Role { get; } = "moderator";
    }
}
