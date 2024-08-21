using System.ComponentModel.DataAnnotations;

namespace BlogPage2.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        [StringLength(500)]

        [Display(Name = "User")]
        public string AppUser { get; set; }

        public Post? Post { get; set; } 
    }
}
