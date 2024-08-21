
using Microsoft.AspNetCore.Mvc.Rendering;
using BlogPage2.Models;
namespace BlogPage2.ViewModels
{
    public class PostDetailsViewModel
    {
        public Post Post { get; set; }
        
        public bool IsLiked { get; set; }

        public List<int> ReplyIds { get; set; }

    }
}
