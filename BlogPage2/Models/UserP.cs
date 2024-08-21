using System.ComponentModel.DataAnnotations;
using System.IO;

namespace BlogPage2.Models
{
    public class UserP
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        [StringLength(500)]
        public string AppUser { get; set; }

        public Post? Post { get; set; } 

    }
}
