using Microsoft.AspNetCore.Http;

namespace ApiMiniPrj.Application.DTOs.Events
{
    public class EventBannerImageUploadDto
    {
        public IFormFile? BannerImage { get; set; }
    }
}
