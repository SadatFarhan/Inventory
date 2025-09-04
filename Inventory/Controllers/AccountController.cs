using Inventory.Models;
using Inventory.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Inventory.Controllers
{
    [AllowAnonymous] // এই কন্ট্রোলারের অ্যাকশনগুলো সবার জন্য উন্মুক্ত
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register registerModel) // Use the ViewModel
        {
            if (ModelState.IsValid)
            {
                var user = new Users
                {
                    UserName = registerModel.UserName,
                    Email = registerModel.Email,
                    Password = registerModel.Password, // You should hash this!
                    Role = "User"
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(registerModel); // Now returns the correct type on failure
        }
        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login loginModel) // <-- Change parameter to use the correct Login model
        {
            // Check if the form data is valid according to the Login model's attributes
            if (!ModelState.IsValid)
            {
                return View(loginModel); // Return the loginModel with validation errors
            }

            var user = await _context.Users
                             .FirstOrDefaultAsync(u => (u.Email == loginModel.NameOrEmail || u.UserName == loginModel.NameOrEmail) && u.Password == loginModel.Password);

            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role),
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "User");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(loginModel); // <-- Return the correct Login model to the view on failure
        }

        // POST: Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET: Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}