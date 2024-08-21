using Microsoft.AspNetCore.Http;
using BlogPage2.Models;

namespace BlogPage2.ViewModels
{
    public class CommentViewModel
    {
        public Comment Comment { get; set; }
        public IFormFile? File { get; set; }
        public string? ExistingImagePath { get; set; }

        public bool DeleteImage { get; set; }
    }
}