namespace ApiMiniPrj.Application.DTOs.Users
{
    public class UserUpdateForAdminDto
    {
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsOrganizer { get; set; }

    }
}
