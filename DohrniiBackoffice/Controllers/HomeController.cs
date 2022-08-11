using DohrniiBackoffice.Domain.Abstract;
using DohrniiBackoffice.Helpers;
using DohrniiBackoffice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DohrniiBackoffice.Controllers
{
    [Authorize(AuthenticationSchemes = AuthConstants.AuthSchemes)]
    public class HomeController : DefaultController<HomeController>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAspNetUserRepository _user;

        public HomeController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager,
            IAspNetUserRepository user)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _user = user;
        }


        [AllowAnonymous()]
        public IActionResult login(string ReturnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Users", "Account");
            }
            else
            {
                ViewBag.ReturnUrl = ReturnUrl;
                return View(new LoginModel());
            }
        }

        [HttpPost]

        [AllowAnonymous()]
        public async Task<IActionResult> login(LoginModel model)
        {
            ViewBag.ReturnUrl = model.ReturnUrl;
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.password, true, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Users", "Account");
                }
                else
                {
                    ViewBag.Msg = _app.GetMsg(alert.success.ToString(), "Invalid login attempt!");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { msg = ex.Message });
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(login));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous()]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        public IActionResult just()
        {
            return View();
        }

        [AllowAnonymous()]
        public IActionResult privacy()
        {
            return View();
        }


    }
}