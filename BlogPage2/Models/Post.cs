using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogPage2.Models
{
    public class Post
    {
        public int Id { get; set; }

        [StringLength(int.MaxValue)]
        [Required]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        public string? Title { get; set; }

        [Display(Name = "Last Modified On: ")]
        [DataType(DataType.DateTime)]
        public DateTime? LastModifiedOn { get; set; }

        [Display(Name = "Created On: ")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedOn { get; set; }

        [StringLength(int.MaxValue)]
        public string? PostContent { get; set; }

        [StringLength(int.MaxValue)]
        public string? Image { get; set; }

        public ICollection<PostCategory>? Categories { get; set; }
        public ICollection<PostTag>? Tags { get; set; }
        public ICollection<Comment>? Comments { get; set; } 
        public ICollection<Like>? Likes { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }

        public ICollection<UserP>? UserPs { get; set; }

        public bool UserLiked(string userId)
        {
            return Likes?.Any(like => like.AppUser == userId) ?? false;
        }

        [Display(Name = "User")]
        public string AppUser { get; set; }
    }
}
