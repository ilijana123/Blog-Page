using System.ComponentModel.DataAnnotations;
using System.IO;

namespace BlogPage2.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [RegularExpression(@"^#.*", ErrorMessage = "Title must start with a #")]
        [StringLength(30)]
        public string Title { get; set; }
        public ICollection<PostTag>? Posts { get; set; }
    }
}
