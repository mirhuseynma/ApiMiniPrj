
namespace ApiMiniPrj.Application.Interfaces.User
{
    public interface IUserService
    {
        Task UpdateAsync(string email, UserUpdateDto userUpdateDto);
        Task<List<UserGetDto>> GetAllAsync();
        Task<UserGetDto> GetByEmailAsync(string email);
        Task DeleteAsync(string email);
        Task UpdateForAdminAsync(string email, UserUpdateForAdminDto userUpdateForAdminDto);
    }
}
