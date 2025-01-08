using in2projekt.Models.ViewModels;
using in2projekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using in2projekt.Models.Data;
using Microsoft.AspNetCore.Identity;  // Import ASP.NET Core Identity
using Org.BouncyCastle.Crypto.Generators;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;  

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); 
        }

        
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == model.Username);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
        if (!isPasswordValid)
        {
            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        
        HttpContext.Session.SetInt32("UserId", user.Id);

        
        return RedirectToAction("Index", "Blogs"); 
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); 
        }

        
        var existingUser = await _context.Users
            .AnyAsync(u => u.Username == model.Username || u.Email == model.Email);

        if (existingUser)
        {
            ModelState.AddModelError("", "Username or Email is already taken.");
            return View(model);
        }

        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

        var user = new User
        {
            Username = model.Username,
            Password = hashedPassword, 
            Email = model.Email
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Registration successful!";
        return RedirectToAction("Login");
    }



    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }
}
