using in2projekt.Models;
using in2projekt.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class BlogsController : Controller
{
    private readonly ApplicationDbContext _context;

    public BlogsController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public IActionResult Index(string searchTerm)
    {
        var blogPosts = _context.BlogPosts.AsQueryable();

        
        if (!string.IsNullOrEmpty(searchTerm))
        {
            blogPosts = blogPosts.Where(b => b.Title.Contains(searchTerm));
        }

        
        return View(blogPosts.ToList());
    }

    
    [HttpGet]
    public IActionResult Create()
    {
        return View(new BlogPostCreateDTO());  
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BlogPostCreateDTO dto)
    {
        
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            
            return RedirectToAction("Login", "Account");
        }

        if (ModelState.IsValid)
        {
            
            var post = new BlogPost
            {
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                user_id = userId.Value 
            };

            
            _context.BlogPosts.Add(post);
            _context.SaveChanges();  

            return RedirectToAction("Index");  
        }

        
        return View(dto); 
    }





    
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var post = _context.BlogPosts.FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            return NotFound();  
        }

        var dto = new BlogPostUpdateDTO
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content
        };

        return View(dto);  
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, BlogPostUpdateDTO dto)
    {
        if (ModelState.IsValid)
        {
            var existingPost = _context.BlogPosts.FirstOrDefault(p => p.Id == id);
            if (existingPost == null)
            {
                return NotFound();
            }

            
            existingPost.Title = dto.Title;
            existingPost.Content = dto.Content;
            existingPost.UpdatedAt = DateTime.UtcNow;

            _context.BlogPosts.Update(existingPost);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        return View(dto);
    }


    
    [HttpGet]
    public IActionResult Delete(int id)
    {
        var post = _context.BlogPosts.FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            return NotFound();  
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue || post.user_id != userId)
        {
            return Unauthorized(); 
        }

        return View(post);  
    }

    public IActionResult ConfirmDelete(int id)
    {
        var post = _context.BlogPosts.FirstOrDefault(p => p.Id == id);

        if (post == null)
        {
            return NotFound(); 
        }

        return View(post); 
    }

    
    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var post = _context.BlogPosts.FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            return NotFound(); 
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue || post.user_id != userId)
        {
            return Unauthorized(); 
        }

        
        _context.BlogPosts.Remove(post);
        _context.SaveChanges();

        
        return RedirectToAction("Index");  
    }



    
    [HttpPost]
    public IActionResult AddComment(int postId, string content)
    {
        
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); 
        }

        var post = _context.BlogPosts.FirstOrDefault(p => p.Id == postId);
        if (post != null)
        {
            var comment = new Comments
            {
                Content = content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                blogpost_id = postId,
                user_id = userId.Value 
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();
        }

        return RedirectToAction("Details", new { id = postId });
    }


   
    public IActionResult Details(int id)
    {
        var post = _context.BlogPosts
            .Include(p => p.Comments) 
            .ThenInclude(c => c.User) 
            .FirstOrDefault(p => p.Id == id);

        if (post == null)
            return NotFound();

        return View(post); 
    }

}
