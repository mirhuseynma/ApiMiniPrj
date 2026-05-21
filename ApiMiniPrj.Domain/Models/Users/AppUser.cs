
namespace ApiMiniPrj.Domain.Models.Users
{
    public class AppUser : IdentityUser
    {
        public string ?FirstName { get; set; }
        public string ?LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
