using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMiniPrj.Application.DTOs.Users
{
    public class UserUpdateDto
    {
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool AssignAdmin { get; set; }
    }
}
