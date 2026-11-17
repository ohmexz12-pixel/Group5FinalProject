
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace firstASP.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // LOGIN PAGE
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    // LOGIN PROCESS
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string email,
        string password)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(
                "",
                "Please enter your email and password.");

            return View();
        }

        email = email.Trim().ToLowerInvariant();

        var result = await _signInManager.PasswordSignInAsync(
            email,
            password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                bool isAdmin =
                    await _userManager.IsInRoleAsync(user, "Admin");

                bool isStudent =
                    await _userManager.IsInRoleAsync(user, "Student");

                if (isAdmin || isStudent)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            await _signInManager.SignOutAsync();

            ModelState.AddModelError(
                "",
                "Your account does not have an assigned role.");

            return View();
        }

        ModelState.AddModelError(
            "",
            "Invalid email or password.");

        return View();
    }

    // STUDENT REGISTRATION PAGE
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // STUDENT REGISTRATION PROCESS
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        string email,
        string password,
        string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            ModelState.AddModelError(
                "",
                "Please complete all fields.");

            return View();
        }

        email = email.Trim().ToLowerInvariant();

        if (!System.Text.RegularExpressions.Regex.IsMatch(
            email,
            @"^[a-zA-Z0-9._%+-]+@([a-zA-Z0-9-]+\.)*benilde\.edu\.ph$"))
        {
            ModelState.AddModelError(
                "",
                "Please use your official Benilde email address.");

            return View();
        }

        if (password != confirmPassword)
        {
            ModelState.AddModelError(
                "",
                "Passwords do not match.");

            return View();
        }

        var existingUser =
            await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            ModelState.AddModelError(
                "",
                "This email is already registered.");

            return View();
        }

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(
            user,
            password);

        if (result.Succeeded)
        {
            var roleResult =
                await _userManager.AddToRoleAsync(user, "Student");

            if (roleResult.Succeeded)
            {
                return RedirectToAction("Login");
            }

            await _userManager.DeleteAsync(user);

            ModelState.AddModelError(
                "",
                "Unable to assign the Student role.");

            return View();
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(
                "",
                error.Description);
        }

        return View();
    }

    // LOGOUT
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Login");
    }
}
