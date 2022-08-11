using DohrniiBackoffice.Domain.Abstract;
using DohrniiBackoffice.Domain.Entities;
using DohrniiBackoffice.DTO.Request;
using DohrniiBackoffice.DTO.Response;
using DohrniiBackoffice.Helpers;
using DohrniiBackoffice.Models;
using DohrniiBackoffice.Models.Emails;
using Microsoft.AspNetCore.Mvc;

namespace DohrniiBackoffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : DefaultController<AuthController>
    {
        private readonly ITokenService _token;
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly IMailHelper _mailHelper;
        public AuthController(ITokenService token, IEmailVerificationRepository emailVerificationRepository, IMailHelper mailHelper)
        {
            _token = token;
            _emailVerificationRepository = emailVerificationRepository;
            _mailHelper = mailHelper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            try
            {
                var rst = await _token.LoginAsync(login.Email, login.Password);
                if(rst.Success)
                {
                    var user = _userRepository.FindBy(c=>c.Email == rst.Email).FirstOrDefault();
                    if(user != null)
                    {
                        if (string.IsNullOrEmpty(user.ProfileImage))
                        {
                            user.ProfileImage = $"{GetBasedUrl()}{AuthConstants.DefaultProfile}";
                        }
                        return Ok(new LoginResp { AccessToken = rst.Token, ExpiringDate = DateTime.Now.AddDays(1), User = _mapper.Map<UserResp>(user) });
                    }
                    else
                    {
                        return BadRequest(new ErrorResponse { Details = rst.ErrorMessage });
                    }
                }
                return BadRequest(new ErrorResponse { Details = rst.ErrorMessage });
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                return InternalServerError(new ErrorResponse { Details = ex.Message });
            }
        }


        [HttpPost("email")]
        public async Task<IActionResult> EmailCode([FromBody] EmailCodeDTO dto)
        {
            try
            {
                if (_app.IsValidEmailAddress(dto.Email))
                {
                    Random generator = new Random();
                    string code = generator.Next(0, 1000000).ToString("D6");
                    var email = _emailVerificationRepository.FindBy(c => c.Email.ToLower() == dto.Email.ToLower()).FirstOrDefault();
                    if(email != null)
                    {
                        if (email.Verified)
                        {
                            return Conflict(new ErrorResponse { Details = "A user with this email already exist" });
                        }
                        email.Code = code;
                        _emailVerificationRepository.Edit(email);
                        await _emailVerificationRepository.Save("System", _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                        //var emailTemplate = _mailHelper.GetEmailTemplate("Welcome", new WelcomeMail { Email = dto.Email, Name = dto.Email, Code = code });
                        var emailTemplate = AuthConstants.HtmlWelcomeTemplate.Replace("{{code}}", code);
                        var emailObj = new MailData(new List<string> { dto.Email }, "Welcome To Dohrnii Academy", emailTemplate);
                        await _mailHelper.SendAsync(emailObj, new CancellationToken());
                        return Ok(dto);
                    }
                    
                    var obj = new EmailVerification
                    {
                        Code = code,
                        Email = dto.Email,
                        Date = DateTime.UtcNow,
                        Device = dto.Device,
                        Region = dto.Region,
                        Verified = false
                    };
                    _emailVerificationRepository.Add(obj);
                    await _emailVerificationRepository.Save("System", _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                    //var template = _mailHelper.GetEmailTemplate("Welcome", new WelcomeMail { Email = dto.Email, Name= dto.Email, Code = code });

                    var template = AuthConstants.HtmlWelcomeTemplate.Replace("{{code}}", code);
                    
                    var emailData = new MailData(new List<string> { dto.Email }, "Welcome To Dohrnii Academy", template);
                    await _mailHelper.SendAsync(emailData, new CancellationToken());
                    return Ok(dto);
                }
                return BadRequest(new ErrorResponse { Details = "Invalid email address" });
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                return InternalServerError(new ErrorResponse { Details = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest register)
        {
            try
            {
                var email = _emailVerificationRepository.FindBy(c => c.Email.ToLower() == register.Email.ToLower() && c.Code == register.Code).FirstOrDefault();
                if(email != null)
                {
                    if (email.Verified)
                    {
                        return Conflict(new ErrorResponse { Details = "This cerification code has already been used!" });
                    }
                    var rst = await _token.RegisterAsync(register.Email, register.Password, register.Email);
                    if (rst.Success)
                    {
                        try
                        {
                            var user = new User
                            {
                                UserName = register.UserName,
                                Email = register.Email,
                                DateAdded = DateTime.Now,
                                FirstName = register.FirstName,
                                LastName = register.LastName,
                                UserRole = AuthConstants.UserRole
                            };
                            _userRepository.Add(user);
                            await _userRepository.Save("System", _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                            
                            email.Verified = true;
                            _emailVerificationRepository.Edit(email);
                            await _emailVerificationRepository.Save("System", _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                            if (string.IsNullOrEmpty(user.ProfileImage))
                            {
                                user.ProfileImage = $"{GetBasedUrl()}{AuthConstants.DefaultProfile}";
                            }
                            var login = await _token.LoginAsync(register.Email, register.Password);
                            if (login.Success)
                            {
                                return Ok(new LoginResp { AccessToken = login.Token, ExpiringDate = DateTime.Now.AddDays(1), User = _mapper.Map<UserResp>(user) });
                            }
                            return Ok(new LoginResp { AccessToken = String.Empty, ExpiringDate = DateTime.MinValue, User = _mapper.Map<UserResp>(user) });
                        }
                        catch (Exception ex)
                        {
                            await _token.DeleteUserAsync(register.Email);
                            return InternalServerError(new ErrorResponse { Details = ex.Message });
                        }
                    }
                    return BadRequest(new ErrorResponse { Details = rst.ErrorMessage });
                }
                return NotFound(new ErrorResponse { Details = "Invalid verification code!" });
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                return InternalServerError(new DTO.Response.ErrorResponse { Details = ex.Message });
            }

        }

        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO register)
        {
            try
            {
                var user = GetUser();
                if (user != null)
                {
                    var rst = await _token.ChangePassword(user.Email,register.OldPassword,register.NewPassword);
                    if (rst.IsSuccessful)
                    {
                        return Ok(rst);
                    }
                    return Conflict(new ErrorResponse { Details = rst.Message });
                }
                else
                {
                    return NotFound(new ErrorResponse { Details = "We can't find your account!" });
                }

            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                return InternalServerError(new DTO.Response.ErrorResponse { Details = ex.Message });
            }

        }
    }
}
