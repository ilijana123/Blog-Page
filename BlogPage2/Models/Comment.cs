using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogPage2.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public int? ParentCommentId { get; set; }

        [Display(Name = "Last Modified On")]
        [DataType(DataType.DateTime)]
        public DateTime? LastModifiedOn { get; set; }


        [Display(Name = "Created On")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedOn { get; set; }

        [StringLength(int.MaxValue)]
        public string? CommentContent { get; set; }

        [StringLength(500)]
        [Display(Name = "User")]
        public string AppUser { get; set; }

        [StringLength(int.MaxValue)]
        public string? Image { get; set; }

        public ICollection<Comment>? Comments { get; set; } = new List<Comment>();

        public Comment? ParentComment { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }

        public Post? Post { get; set; }
    }
}
