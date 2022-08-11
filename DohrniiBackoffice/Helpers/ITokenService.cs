using DohrniiBackoffice.DTO.Response;
using DohrniiBackoffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DohrniiBackoffice.Helpers
{
    public interface ITokenService
    {
        Task<RegisterResponse> RegisterAsync(string email, string password, string username);
        Task<RegisterResponse> RegisterAdminAsync(string email, string password, string username);
        Task<AuthResult> LoginAsync(string email, string password);
        Task<bool> DeleteUserAsync(string email);
        Task<ChangePasswordRespDTO> ChangePassword(string email, string oldpassword, string newpassword);
    }
}
