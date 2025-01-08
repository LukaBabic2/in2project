using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using in2projekt.Models;
using in2projekt.Models.Data;
using in2projekt.Models.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;
using System.Text.RegularExpressions;

public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    public async Task<IActionResult> ListUsers()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId != 1)  
        {
            return RedirectToAction("Index", "Blogs");
        }

        var users = await _context.Users.ToListAsync();
        return View("ListUsers", users);
    }

    [HttpGet]
    public IActionResult AddUser()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddUser(UserViewModel model)
    {
        if (ModelState.IsValid)
        {
            
            if (!IsPasswordValid(model.Password))
            {
                ModelState.AddModelError("Password", "Password must be at least 8 characters long and contain at least one uppercase and one lowercase letter.");
                return View(model);
            }

           
            if (!IsEmailValid(model.Email))
            {
                ModelState.AddModelError("Email", "Please provide a valid email address.");
                return View(model);
            }

            
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = hashedPassword 
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListUsers");
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var model = new UserViewModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(UserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _context.Users.FindAsync(model.Id);
        if (user == null)
        {
            return NotFound();
        }

        user.Username = model.Username;
        user.Email = model.Email;

        
        if (!string.IsNullOrEmpty(model.Password))
        {
            if (!IsPasswordValid(model.Password))
            {
                ModelState.AddModelError("Password", "Password must be at least 8 characters long and contain at least one uppercase and one lowercase letter.");
                return View(model);
            }

            
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password); 
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("ListUsers");
    }

    [HttpGet]
    public async Task<IActionResult> RemoveUser(int id)
    {
        if (id == 1) 
        {
            TempData["ErrorMessage"] = "The admin user cannot be deleted.";
            return RedirectToAction("ListUsers");
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        return View(user);
    }

    [HttpPost, ActionName("RemoveUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveUserConfirmed(int id)
    {
        if (id == 1) 
        {
            TempData["ErrorMessage"] = "The admin user cannot be deleted.";
            return RedirectToAction("ListUsers");
        }

        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("ListUsers");
    }

   
    private bool IsPasswordValid(string password)
    {
       
        var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z]).{8,}$");
        return passwordRegex.IsMatch(password);
    }

   
    private bool IsEmailValid(string email)
    {
        
        var emailRegex = new Regex(@"^[^@]+@[^@]+\.[a-zA-Z]{2,}$");
        return emailRegex.IsMatch(email);
    }
}
