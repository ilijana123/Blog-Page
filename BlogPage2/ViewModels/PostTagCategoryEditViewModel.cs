using Microsoft.AspNetCore.Mvc.Rendering;
using BlogPage2.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace BlogPage2.ViewModels
{
    public class PostTagCategoryEditViewModel
    {
        public Post Post { get; set; }
        public IEnumerable<int>? SelectedCategories { get; set; }
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
        public IEnumerable<int>? SelectedTags { get; set; }
        public IEnumerable<SelectListItem>? TagList { get; set; }
        public String? ExistingImagePath { get; set; }

        [RegularExpression(@"^#([a-zA-Z'""\s-]+)( #([a-zA-Z'""\s-]+))*$", ErrorMessage = "Tags must start with '#' and be separated by a single space. Tags must not contain more than one '#' and must be properly separated by spaces.")]
        public string? NewTag { get; set; }

        public string? NewCategory { get; set; }
        public bool DeleteImage { get; set; }

    }
}