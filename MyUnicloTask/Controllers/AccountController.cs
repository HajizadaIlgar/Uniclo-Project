using Ab108Uniqlo.Models;
using Ab108Uniqlo.ViewModels.Auths;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ab108Uniqlo.Controllers;

public class AccountController(UserManager<AppUser> _userManager) : Controller
{
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        AppUser appUser = new AppUser
        {
            Fullname = vm.Fullname,
            Email = vm.Email,
            UserName = vm.Username,
        };
        var result = await _userManager.CreateAsync(appUser, vm.Password);
        if (result.Succeeded)
        {
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }
        }
        if (!ModelState.IsValid)
        {
            return View();
        }
        return View();
    }
    public IActionResult Login()
    {
        return View();
    }

}