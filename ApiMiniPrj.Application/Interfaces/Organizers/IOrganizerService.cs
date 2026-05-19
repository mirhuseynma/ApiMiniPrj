using ApiMiniPrj.Application.DTOs.Organizers;
using Microsoft.AspNetCore.Http;

namespace ApiMiniPrj.Application.Interfaces.Organizers
{
    public interface IOrganizerService
    {
        Task CreateOrganizerAsync(OrganizerCreateDto createOrganizerDto);
        Task DeleteOrganizerAsync(int id);
        Task UpdateOrganizerAsync(int id, OrganizerUpdateDto updateOrganizerDto);
        Task<GetOrganizerDto> GetOrganizerByIdAsync(int id);
        Task OrganizerUploadLogo(int id, IFormFile logo);
        Task<IEnumerable<GetOrganizerDto>> GetAllOrganizersAsync();
    }
}
