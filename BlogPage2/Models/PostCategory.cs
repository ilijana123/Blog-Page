using System.ComponentModel.DataAnnotations;
using System.IO;

namespace BlogPage2.Models
{
    public class PostCategory
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post? Post { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

    }
}
