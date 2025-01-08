using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace in2projekt.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        
        public ICollection<BlogPost> BlogPosts { get; set; }

        
        public ICollection<Comments> Comments { get; set; }
    }
}
