using Ab108Uniqlo.Extensions;
using Ab108Uniqlo.Models;
using Ab108Uniqlo.Services.Abstracts;
using Ab108Uniqlo.ViewModels.Auths;
using Ab108Uniqlo.Views.Account.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Ab108Uniqlo.Controllers;

public class AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, RoleManager<IdentityRole> _roleManager, IEmailService _service, IMemoryCache _cache) : Controller
{
    public async Task<IActionResult> Send()
    {
        /* SmtpClient client = new SmtpClient();
             client.Host = "smtp.gmail.com";
        client.Port = 587;
        client.EnableSsl = true;
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential("ilgarh-ab108@code.edu.az", "o s z s m o l z r n t t f m h c");
        MailAddress from = new MailAddress("ilgarh-ab108@code.edu.az", "Uniqlo");
        MailAddress to = new MailAddress("revanhacizade865@gmail.com\r\n");
        MailMessage message = new MailMessage(from, to);
        message.Subject = "kjdnskjbvsjk";
        message.Body = "<p>jksdjn ndkjsdnsdkn dvjnsnv nkjv</p>";
        message.IsBodyHtml = true;
        client.Send(message);*/
        return Ok();
    }
    private bool IsAuthenticated => HttpContext.User.Identity?.IsAuthenticated ?? false;
    public IActionResult Register()
    {
        if (IsAuthenticated) return RedirectToAction("Index", "Home");
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
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }
        }
        var roleResult = await _userManager.AddToRoleAsync(appUser, Roles.User.GetRole());
        if (!roleResult.Succeeded)
        {
            foreach (var err in roleResult.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }
            return View();
        }
        string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
        _service.SendEmailConfirmationAsync(appUser.Email, appUser.UserName, token);
        return Content("Email Snet");
    }
    public async Task<IActionResult> MyRolesMethod()
    {
        foreach (Roles item in Enum.GetValues(typeof(Roles)))
        {
            await _roleManager.CreateAsync(new IdentityRole(item.GetRole()));
        }
        return Ok();
    }
    public async Task<IActionResult> ForgetPassword()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ForgetPassword(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return View("EmailNotFound");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        _service.SendEmailConfirmationAsync(user.Email!, user.UserName!, token);
        return Content("LinkSent");
    }
    [HttpGet]
    public IActionResult ResetPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(string email, string token, string pass)
    {
        if (!ModelState.IsValid) return View();

        var entity = await _userManager.FindByEmailAsync(email);
        if (entity == null) return NotFound();
        var reset = await _userManager.ResetPasswordAsync(entity, token.Replace(' ', '+'), pass);
        return RedirectToAction(nameof(Register));
    }

    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginVM vm, string? ReturnUrl = null)
    {
        if (IsAuthenticated) return RedirectToAction("Index", "Home");
        if (!ModelState.IsValid) return View();
        AppUser? user = null;
        if (vm.UsernameOrEmail.Contains("@"))
        {
            user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
        }
        else
        {
            user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
        }
        if (user is null)
        {
            ModelState.AddModelError("", "Username Or Password is wrong!!!");
            return View();
        }
        var result = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Wait untill" + user.LockoutEnd.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (result.IsNotAllowed)
            {
                ModelState.AddModelError("", "Username or password is wrong");
            }
        }
        if (string.IsNullOrWhiteSpace(ReturnUrl))
        {
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction("Index", new { Controller = "Dashbard", Area = "Admin" });
            }
            return RedirectToAction("Index", "Home");
        }
        return LocalRedirect(ReturnUrl);
    }
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
    public async Task<IActionResult> VerifyEmail(int code, string user)
    {
        var entity = await _userManager.FindByNameAsync(user);
        if (entity is null) return BadRequest();
        int? cacheCode = _cache.Get<int>(entity.Id);
        if (!cacheCode.HasValue || cacheCode != code)
            return BadRequest();
        entity.EmailConfirmed = true;
        await _userManager.UpdateAsync(entity);
        _cache.Remove(entity.Id);
        await _signInManager.SignInAsync(entity, true);
        return RedirectToAction("Index", "Home");

    }
}