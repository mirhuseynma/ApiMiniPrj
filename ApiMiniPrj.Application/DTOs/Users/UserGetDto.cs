namespace ApiMiniPrj.Application.DTOs.Users
{
    public class UserGetDto
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } = [];

    }
}
