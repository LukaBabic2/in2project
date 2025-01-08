using System.ComponentModel.DataAnnotations;

namespace in2projekt.Models
{
    public class BlogPostUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
