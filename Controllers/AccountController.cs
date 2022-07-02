using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpAuthenticationAndAuthorization.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CSharpAuthenticationAndAuthorization.Controllers
{
    public class AccountController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _usermanager;
        private readonly Microsoft.AspNetCore.Identity.SignInManager<IdentityUser> _signinmanager;

        public AccountController(UserManager<IdentityUser> usermanager,SignInManager<IdentityUser> signinmanager)
        {
            _usermanager = usermanager;
            _signinmanager = signinmanager;
        }
        public IActionResult Index()
        {
           
            return View();
        }
        public async Task<IActionResult> LogOut()
        {
            await _signinmanager.SignOutAsync();
            return RedirectToAction("index", "Home");
        }
        [AcceptVerbs("Get","Post")]
        [AllowAnonymous]
        public async Task<IActionResult>IsEmailExists(string email)
        {
            var user =await  _usermanager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"{email} is in use");
            }
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model,string returnUrl)
        {
            if (ModelState.IsValid)
            {                
                var result = await _signinmanager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        //if there is a returnUrl in address bar then after login the user will be redirected to that url
                       //To avoid openredirect attack we use LocalRedirect
                        return LocalRedirect(returnUrl);
                        // return Redirect(returnUrl);
                        //Another method of avoiding openurl attack
                        //if (Url.IsLocalUrl(returnUrl))
                        //{
                        //    return Redirect(returnUrl);
                        //}
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    
                }
                                
                        ModelState.AddModelError("", "Invalid user");
                    
                
            }

            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task <IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email
                };
                var result = await _usermanager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (_signinmanager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }
                    await _signinmanager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }

                }
            }
            //return View(model)
            return View();
        }
    }
}
