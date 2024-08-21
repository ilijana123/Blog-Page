using Microsoft.AspNetCore.Mvc.Rendering;
using BlogPage2.Models;
namespace BlogPage2.ViewModels
{
    public class PostCategoryTagViewModel
    {
        public IList<Post> Posts { get; set; }
        public SelectList Categories{ get; set; }

        public SelectList Tags { get; set; }
        public string PostTag { get; set; }
        public string PostCategory { get; set; }
        public string SearchStringC { get; set; }
        public string SearchStringT { get; set; }

        public string SortOrder { get; set; }

        public Dictionary<int, bool> IsLiked { get; set; }

    }
}
