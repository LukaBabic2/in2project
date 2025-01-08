using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace in2projekt.Models
{
    public class Comments
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        
        [Required]
        public int user_id { get; set; }

        
        [Required]
        [ForeignKey("BlogPosts")]
        public int blogpost_id { get; set; }

        
        public User User { get; set; }
        public BlogPost BlogPost { get; set; }
    }
}
