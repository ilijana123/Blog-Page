using System.ComponentModel.DataAnnotations;
using System.IO;

namespace BlogPage2.Models
{
    public class Category
    {
        public int Id { get; set; }

        [StringLength(30)]
        [Required]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        public string? Title { get; set; }

        public ICollection<PostCategory>? Posts { get; set; }

    }
}
