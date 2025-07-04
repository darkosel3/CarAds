﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CarAds.Models;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CarAds.Controllers
{
    public class UserController : Controller
    {

        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Register(User user)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        var appUser = new ApplicationUser
        //        {
        //            UserName = user.Name,
        //            Email = user.Email
        //        };

        //        var result = await _userManager.CreateAsync(appUser, user.Password);

        //        if (result.Succeeded)
        //        {
        //            ViewBag.Message = "User created successfully!"; 
        //        }
        //        else
        //        {
        //            foreach (var error in result.Errors)
        //                ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //    }
        //    return RedirectToAction("Login", "Account");
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                var appUser = new ApplicationUser
                {
                    UserName = user.Name,
                    Email = user.Email
                };

                var result = await _userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                {
                    // korisnik je uspešno kreiran i upisan u MongoDB
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // ako nesto nije u redu, vrati registracioni formular sa greškama
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new ApplicationRole() { Name = userRole.RoleName });
                if (result.Succeeded)
                {
                    ViewBag.Message = "Role created successfully!";
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                 }
            }
            return View();
        }
    }
}
