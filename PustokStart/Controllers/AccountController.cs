﻿using System.Drawing.Text;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using PustokStart.DAL;
using PustokStart.Models;
using PustokStart.ViewModels;

namespace PustokStart.Controllers
{
    public class AccountController : Controller
    {
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly PustokContext _context;

		public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,PustokContext context)
        {
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
		}
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(MemberLoginViewModel memberVM,string returnUrl=null)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Email or password wrong");
               return View();
            }
            AppUser user = await _userManager.FindByEmailAsync(memberVM.Email);
            if (user == null || user.IsAdmin)
            {
				ModelState.AddModelError("", "Email or password wrong");
                return View();

			}
            var result = await _signInManager.PasswordSignInAsync(user, memberVM.Password, false, false);
            if (!result.Succeeded)
            {
				ModelState.AddModelError("", "Username or password incorrect");
				return View();
			}
            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("index", "home");
			
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(MemberRegisterViewModel registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if(await _userManager.Users.AnyAsync(x=>x.UserName==registerVM.UserName)) 
            {
                ModelState.AddModelError("UserName", "UserName is already used");
                return View();
            }
			if (await _userManager.Users.AnyAsync(x => x.Email == registerVM.Email))
			{
				ModelState.AddModelError("Email", "Email is already used");
				return View();
			}
            AppUser user= new AppUser()
            {
                FullName=registerVM.FullName,
                Email=registerVM.Email,
                UserName=registerVM.UserName,
                IsAdmin=false,

            };
            var result =await _userManager.CreateAsync(user,registerVM.Password);
            if (!result.Succeeded) 
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
				return View();
			}

           await _userManager.AddToRoleAsync(user, "Member");
            //await _signInManager.SignInAsync(user,false);

            return RedirectToAction("login", "account");



        }
        public async Task<IActionResult> Logout()
        {
           await _signInManager.SignOutAsync();
            return RedirectToAction("login","account");
        }


        [Authorize(Roles ="Member")]
        public async Task<IActionResult> Profile()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) {
                await _signInManager.SignOutAsync();
                return RedirectToAction("login");
            }
            AccountProfileViewModel vm=new AccountProfileViewModel()
            {
                Profile = new ProfileEditViewModel()
                {
                    FullName=user.FullName,
                    Email=user.Email,
                    UserName=user.UserName,
                    Address=user.Address,
                    Phone=user.Phone
                },
                Orders=_context.Orders.Include(x=>x.OrderItems).ThenInclude(x=>x.Book).Where(x=>x.AppUserId==user.Id).ToList(),
               
            };
           return View(vm);
        }
		[Authorize(Roles = "Member")]
		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileEditViewModel profileVM)
        
        {
			AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (!ModelState.IsValid)
            {
                AccountProfileViewModel vm=new AccountProfileViewModel()
                {
                    Profile= profileVM
				};
                return View(vm);
            }
            if(profileVM.CurrentPassword!=null &&( profileVM.NewPassword==null || profileVM.CurrentPassword==null))
            {
                ModelState.AddModelError("", "Fill all cell");
				AccountProfileViewModel vm = new AccountProfileViewModel()
				{
					Profile = profileVM
				};
				return View(vm);

			}
        
          
  
          
            user.FullName=profileVM.FullName;
            user.Email=profileVM.Email;
            user.UserName=profileVM.UserName;
            user.Address=profileVM.Address;
            user.Phone=profileVM.Phone;

           var result =await _userManager.UpdateAsync(user);
			if (profileVM.CurrentPassword != null && profileVM.ConfirmPassword == profileVM.NewPassword)
			{

				var passResult = _userManager.ChangePasswordAsync(user, profileVM.CurrentPassword, profileVM.NewPassword);
				var test = _signInManager.SignInAsync(user, false);
			}
			if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
				AccountProfileViewModel vm = new AccountProfileViewModel()
				{
					Profile = profileVM
				};
                await _signInManager.SignInAsync(user,false);
				return View(vm);
            }
            return RedirectToAction("profile");
        }

        public IActionResult ForgetPassword()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel passwordVM)
        {
            if(!ModelState.IsValid) return View();
            AppUser user =await _userManager.FindByEmailAsync(passwordVM.Email);
            if(user == null || user.IsAdmin) { ModelState.AddModelError("Email", "Email not found"); return View(); }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string url = Url.Action("resetpassword", "account", new {email=passwordVM.Email,token=token},Request.Scheme);
            return Json(new {url=url});

          
        }
        public async Task<IActionResult> ResetPassword(string email,string token)
        {
			AppUser user = await _userManager.FindByEmailAsync(email);
			if (user == null || user.IsAdmin || !await _userManager.VerifyUserTokenAsync(user,_userManager.Options.Tokens.PasswordResetTokenProvider,"ResetPassword",token)) { ModelState.AddModelError("Email", "Email not found"); return RedirectToAction("login"); }
            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();

		}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetVM)
        {
			AppUser user = await _userManager.FindByEmailAsync(resetVM.Email);
			if (user == null || user.IsAdmin ) { ModelState.AddModelError("Email", "Email not found"); return RedirectToAction("login"); }
           var result =await _userManager.ResetPasswordAsync(user, resetVM.Token,resetVM.Password);
            if (!result.Succeeded)
            {
                return RedirectToAction("login");
            }
			return RedirectToAction("login");
        }
    }
}
