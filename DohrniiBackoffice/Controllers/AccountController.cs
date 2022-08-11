using DohrniiBackoffice.Domain.Abstract;
using DohrniiBackoffice.Domain.Entities;
using DohrniiBackoffice.Helpers;
using DohrniiBackoffice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DohrniiBackoffice.Controllers
{
    [Authorize(AuthenticationSchemes = AuthConstants.AuthSchemes, Roles = AuthConstants.AdminRole)]
    public class AccountController : DefaultController<HomeController>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAspNetUserRepository _user;

        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager,
            IAspNetUserRepository user)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _user = user;
        }
        public IActionResult Users()
        {
            //var aa = User.Identity.IsAuthenticated;
            var dt = _userRepository.FindBy(c => c.UserRole == AuthConstants.AdminRole).ToList();
            ViewBag.Msg = TempData["msg"];
            return View(dt);
        }

        [HttpPost]
        public async Task<IActionResult> Register(AdminRegistrationModel register)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new IdentityUser { UserName = register.Email, Email = register.Email, EmailConfirmed = true };
                    var result = await _userManager.CreateAsync(user, register.Password);
                    if (result.Succeeded)
                    {
                        var rst = await _roleManager.RoleExistsAsync(AuthConstants.AdminRole);
                        if (!rst)
                        {
                            await _roleManager.CreateAsync(new IdentityRole { Name = AuthConstants.AdminRole });
                        }
                        var resp = await _userManager.AddToRoleAsync(user, AuthConstants.AdminRole);
                        if (resp.Succeeded)
                        {
                            var userInfo = new User
                            {
                                UserName = register.UserName,
                                Email = register.Email,
                                DateAdded = DateTime.Now,
                                FirstName = register.FirstName,
                                LastName = register.LastName,
                                UserRole = AuthConstants.AdminRole
                            };
                            _userRepository.Add(userInfo);
                            await _userRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                        }
                        TempData["msg"] = _app.GetMsg(alert.success.ToString(), "User created successfully!");
                        return RedirectToAction(nameof(Users));
                    }
                    else
                    {
                        TempData["msg"] = _app.GetMsg(alert.warning.ToString(), result.Errors.FirstOrDefault()?.Description);
                        return RedirectToAction(nameof(Users));
                    }
                }
                else
                {
                    TempData["msg"] = _app.GetMsg(alert.warning.ToString(), ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage);
                    return RedirectToAction(nameof(Users));
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(Users));
            }

        }
    }
}
