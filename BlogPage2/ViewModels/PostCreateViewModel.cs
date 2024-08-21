using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BlogPage2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogPage2.ViewModels
{
    public class PostCreateViewModel
    {
        public Post Post { get; set; }
        public IEnumerable<SelectListItem>? TagList { get; set; }
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
        public List<int>? SelectedTags { get; set; }
        public List<int>? SelectedCategories { get; set; }

        [RegularExpression(@"^#([a-zA-Z'""\s-]+)( #([a-zA-Z'""\s-]+))*$", ErrorMessage = "Tags must start with '#' and be separated by a single space. Tags must not contain more than one '#' and must be properly separated by spaces.")]
        public string? NewTags { get; set; }

        public string? NewCategories { get; set; }
    }
}
