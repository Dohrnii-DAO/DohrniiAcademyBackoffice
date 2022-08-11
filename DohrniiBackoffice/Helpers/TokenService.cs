using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using DohrniiBackoffice.DTO;
using DohrniiBackoffice.Models;
using DohrniiBackoffice.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DohrniiBackoffice.DTO.Response;

namespace DohrniiBackoffice.Helpers
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwt;
        public TokenService(UserManager<IdentityUser> userManager, IOptions<JwtSettings> jwt, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthResult
                {
                    ErrorMessage = "Invalid login attempt!",
                    Success = false
                };  
            }
            var hasValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!hasValidPassword)
            {
                return new AuthResult
                {
                    ErrorMessage ="Invalid password or username!",
                    Success = false
                };
            }
            return await GetAuth(user);
        }


        public async Task<RegisterResponse> RegisterAsync(string email, string password, string username)
        {
            var user = new IdentityUser { UserName = username, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                
                var rst = await _roleManager.RoleExistsAsync("User");
                if (!rst)
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
                }
                await _userManager.AddToRoleAsync(user, "User");
                return new RegisterResponse
                {
                    Success = true,
                };
            }
            return new RegisterResponse
            {
                Success = false,
                ErrorMessage = result.Errors.ToList()[0].Description
            };
        }

        public async Task<bool> DeleteUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public async Task<ChangePasswordRespDTO> ChangePassword(string email, string oldpassword, string newpassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, oldpassword, newpassword);
                if (result.Succeeded)
                {
                    return new ChangePasswordRespDTO { IsSuccessful = true, Message = "Changed Successful" };
                }
                return new ChangePasswordRespDTO { IsSuccessful = false, Message = result.Errors.ToList()[0].Description };
            }
            return new ChangePasswordRespDTO { IsSuccessful = false, Message = "User not found!"};
        }

        public async Task<RegisterResponse> RegisterAdminAsync(string email, string password, string username)
        {
            var user = new IdentityUser { UserName = username, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {

                var rst = await _roleManager.RoleExistsAsync("Admin");
                if (!rst)
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
                }
                await _userManager.AddToRoleAsync(user, "Admin");
                return new RegisterResponse
                {
                    Success = true,
                };
            }
            return new RegisterResponse
            {
                Success = false,
                ErrorMessage = result.Errors.ToList()[0].Description
            };
        }

        private async Task<AuthResult> GetAuth(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwt.Secret);
            var claims = await GetValidClaims(user);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new AuthResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                Email = user.Email
            };

        }

        private async Task<List<Claim>> GetValidClaims(IdentityUser user)
        {
            IdentityOptions _options = new IdentityOptions();
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(_options.ClaimsIdentity.UserIdClaimType, user.Id),
            new Claim(_options.ClaimsIdentity.UserNameClaimType, user.UserName)
        };
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            claims.AddRange(userClaims);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            return claims;
        }

        
    }
}
