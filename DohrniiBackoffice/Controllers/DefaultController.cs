using AutoMapper;
using DohrniiBackoffice.Domain.Abstract;
using DohrniiBackoffice.Domain.Entities;
using DohrniiBackoffice.DTO.Response;
using DohrniiBackoffice.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

namespace DohrniiBackoffice.Controllers
{
    public class DefaultController<T> : Controller where T : DefaultController<T>
    {
        private ILogger<T> logger;
        private IImageHelper imageHelper;
        private IActionContextAccessor accessor;
        private IHttpContextAccessor ca;
        private IMapper mapper;
        private IAppUtil app;
        private IUserRepository userRepository;

        protected ILogger<T> _Logger => logger ?? (logger = HttpContext?.RequestServices.GetService<ILogger<T>>());
        protected IImageHelper _imageHelper => imageHelper ?? (imageHelper = HttpContext?.RequestServices.GetService<IImageHelper>());
        protected IActionContextAccessor _accessor => accessor ?? (accessor = HttpContext?.RequestServices.GetService<IActionContextAccessor>());
        protected IHttpContextAccessor _ca => ca ?? (ca = HttpContext?.RequestServices.GetService<IHttpContextAccessor>());
        protected IMapper _mapper => mapper ?? (mapper = HttpContext?.RequestServices.GetService<IMapper>());
        protected IAppUtil _app => app ?? (app = HttpContext?.RequestServices.GetService<IAppUtil>());
        protected IUserRepository _userRepository => userRepository ?? (userRepository = HttpContext?.RequestServices.GetService<IUserRepository>());



        [NonAction]
        public InternalServerErrorObjectResult InternalServerError(ErrorResponse value)
        {
            return new InternalServerErrorObjectResult(value);
        }

        [NonAction]
        public InternalServerErrorObjectResult BadRequestError(ErrorResponse value)
        {
            return new InternalServerErrorObjectResult(value);
        }

        [NonAction]
        public string GetUserName()
        {
            string username = "Guest";
            try
            {
                if (_ca.HttpContext.User.Identity.IsAuthenticated)
                {
                    username = _ca.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                }
            }
            catch (Exception) { }
            return username;
        }

        [NonAction]
        public User? GetUser()
        {
            try
            {
                var username = GetUserName();
                if(username != "Guest")
                {
                    var user = _userRepository.FindBy(c => c.Email.ToLower() == username.ToLower()).FirstOrDefault();
                    return user;
                }
            }
            catch (Exception) { }
            return null;
        }

        [NonAction]
        public string GetBasedUrl()
        {
            string basrUrl = "";
            try
            {
                HttpRequest request = _ca.HttpContext.Request;
                basrUrl = $"{request.Scheme}://{request.Host.Value}";
            }
            catch (Exception) { }
            return basrUrl;
        }
    }

    public class BadRequestObjectResult : ObjectResult
    {
        public BadRequestObjectResult(ErrorResponse value) : base(value)
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(ErrorResponse value) : base(value)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
