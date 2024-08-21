using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogPage2.Data;
using BlogPage2.Models;
using BlogPage2.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using BlogPage2.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration.UserSecrets;
namespace BlogPage2.Controllers
{
    public class PostsController : Controller
    {
        private readonly BlogPage2Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<BlogPage2User> _userManager;
        public PostsController(BlogPage2Context context, IWebHostEnvironment webHostEnvironment, UserManager<BlogPage2User> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }


        // GET: Posts

        public async Task<IActionResult> Index(string postCategory, string postTag, string searchStringC, string searchStringT, string sortOrder)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.CurrentUserId = userId;
            IQueryable<Post> posts = _context.Post
                .Include(m => m.Tags).ThenInclude(m => m.Tag)
                .Include(m => m.Categories).ThenInclude(m => m.Category)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Include(p => p.UserPs);

            if (!string.IsNullOrEmpty(postTag) && postTag != "All Tags")
            {
                posts = posts.Where(p => p.Tags.Any(t => t.Tag.Title == postTag));
            }

            if (!string.IsNullOrEmpty(postCategory) && postCategory != "All Categories")
            {
                posts = posts.Where(p => p.Categories.Any(c => c.Category.Title == postCategory));
            }

            if (!string.IsNullOrEmpty(searchStringT))
            {
                posts = posts.Where(p => p.Tags.Any(pt => pt.Tag.Title.Contains(searchStringT)));
            }

            if (!string.IsNullOrEmpty(searchStringC))
            {
                posts = posts.Where(p => p.Categories.Any(pc => pc.Category.Title.Contains(searchStringC)));
            }

            switch (sortOrder)
            {
                case "likes_desc":
                    posts = posts.OrderByDescending(p => p.Likes.Count);
                    break;
                case "likes_asc":
                    posts = posts.OrderBy(p => p.Likes.Count);
                    break;
                case "comments_desc":
                    posts = posts.OrderByDescending(p => p.Comments.Count);
                    break;
                case "comments_asc":
                    posts = posts.OrderBy(p => p.Comments.Count);
                    break;
                default:
                    posts = posts.OrderBy(p => p.Title);
                    break;
            }

            var tagQuery = _context.PostTag.Select(pt => pt.Tag.Title).Distinct();
            var categoryQuery = _context.PostCategory.Select(pc => pc.Category.Title).Distinct();
            var postList = await posts.ToListAsync();
            var likedPosts = new Dictionary<int, bool>();
            foreach (var post in postList)
            {
                likedPosts[post.Id] = post.Likes.Any(l => l.AppUser == userId);
            }


            var VM = new PostCategoryTagViewModel
            {
                Tags = new SelectList(await tagQuery.ToListAsync()),
                Categories = new SelectList(await categoryQuery.ToListAsync()),
                Posts = postList,
                IsLiked = likedPosts
            };

            return View(VM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                viewModel.Post.AppUser = userId;
                if (viewModel.Post.File != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.Post.File.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    viewModel.Post.File.CopyTo(new FileStream(filePath, FileMode.Create));
                    viewModel.Post.Image = uniqueFileName;
                }

                viewModel.Post.CreatedOn = DateTime.Now;
                _context.Add(viewModel.Post);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(viewModel.NewTags))
                {
                    var newTags = viewModel.NewTags.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var tagTitle in newTags)
                    {
                        var trimmedTagTitle = tagTitle.Trim();
                        if (_context.Tag.Any(t => t.Title == trimmedTagTitle))
                        {
                            continue;
                        }

                        var newTag = new Tag { Title = trimmedTagTitle };
                        _context.Tag.Add(newTag);
                        await _context.SaveChangesAsync();
                        _context.PostTag.Add(new PostTag { PostId = viewModel.Post.Id, TagId = newTag.Id });
                    }
                    await _context.SaveChangesAsync();
                }

                if (viewModel.SelectedTags != null && viewModel.SelectedTags.Any())
                {
                    foreach (var tagId in viewModel.SelectedTags)
                    {
                        if (!_context.PostTag.Any(pt => pt.PostId == viewModel.Post.Id && pt.TagId == tagId))
                        {
                            _context.PostTag.Add(new PostTag { PostId = viewModel.Post.Id, TagId = tagId });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                if (!string.IsNullOrEmpty(viewModel.NewCategories))
                {
                    var newCategories = viewModel.NewCategories.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var categoryTitle in newCategories)
                    {
                        var trimmedCategoryTitle = categoryTitle.Trim();
                        if (_context.Category.Any(t => t.Title == trimmedCategoryTitle))
                        {
                            continue;
                        }

                        var newCategory = new Category { Title = trimmedCategoryTitle };
                        _context.Category.Add(newCategory);
                        await _context.SaveChangesAsync();
                        _context.PostCategory.Add(new PostCategory { PostId = viewModel.Post.Id, CategoryId = newCategory.Id });
                    }
                    await _context.SaveChangesAsync();
                }

                if (viewModel.SelectedCategories != null && viewModel.SelectedCategories.Any())
                {
                    foreach (var categoryId in viewModel.SelectedCategories)
                    {
                        if (!_context.PostCategory.Any(pc => pc.PostId == viewModel.Post.Id && pc.CategoryId == categoryId))
                        {
                            _context.PostCategory.Add(new PostCategory { PostId = viewModel.Post.Id, CategoryId = categoryId });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }


        // GET: Posts/Create
        public IActionResult Create()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserId = userId;
            var tags = _context.Tag.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Title
            }).ToList();

            var categories = _context.Category.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Title
            }).ToList();

            var viewModel = new PostCreateViewModel
            {
                Post = new Post(),
                TagList = tags,
                CategoryList = categories
            };

            return View(viewModel);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(m=>m.Tags).ThenInclude(m=>m.Tag)
                .Include(m=>m.Categories).ThenInclude(m=>m.Category)
                .Include(m=>m.Comments)
                .Include(m=>m.UserPs)
                .Include(m=>m.Likes)
                .FirstOrDefaultAsync(m => m.Id == id);
            var replyIds = post.Comments
               .Where(c => c.Comments.Any())
               .SelectMany(c => c.Comments.Select(r => r.Id))
               .ToList();
            if (post == null)
            {
                return NotFound();
            }
            var isLiked = _context.Like.Any(l => l.PostId == id && l.AppUser == userId);
            var viewmodel = new PostDetailsViewModel()
            {
                Post = post,
                IsLiked = isLiked,
                ReplyIds = replyIds
            };
            return View(viewmodel);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(m => m.Tags)
                .Include(m => m.Categories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var tags = _context.Tag.OrderBy(t => t.Title).ToList();
            var categories = _context.Category.OrderBy(c => c.Title).ToList();

            var viewmodel = new PostTagCategoryEditViewModel
            {
                Post = post,
                TagList = new MultiSelectList(tags, "Id", "Title"),
                SelectedTags = post.Tags.Select(m => m.TagId),
                CategoryList = new MultiSelectList(categories, "Id", "Title"),
                SelectedCategories = post.Categories.Select(m => m.CategoryId),
                ExistingImagePath = post.Image
            };
            return View(viewmodel);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostTagCategoryEditViewModel viewmodel, bool DeleteImage)
        {
            if (id != viewmodel.Post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPost = await _context.Post.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    if (existingPost == null)
                    {
                        return NotFound();
                    }
                    if (DeleteImage && !string.IsNullOrEmpty(existingPost.Image))
                    {
                        string oldImagePath = Path.Combine(uploadsFolder, existingPost.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        viewmodel.Post.Image = null;
                    }
                    else if (viewmodel.Post.File != null)
                    {
                        
                        if (!string.IsNullOrEmpty(viewmodel.ExistingImagePath))
                        {
                            string oldImagePath = Path.Combine(uploadsFolder, viewmodel.ExistingImagePath);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewmodel.Post.File.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewmodel.Post.File.CopyToAsync(fileStream);
                        }
                        viewmodel.Post.Image = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.Post.Image = viewmodel.ExistingImagePath;
                    }

                    viewmodel.Post.CreatedOn = existingPost.CreatedOn;
                    viewmodel.Post.LastModifiedOn = DateTime.Now;

                    _context.Update(viewmodel.Post);

                    var existingTags = await _context.PostTag.Where(s => s.PostId == id).ToListAsync();
                    var newTagIds = viewmodel.SelectedTags ?? new List<int>();
                    var tagsToAdd = newTagIds.Except(existingTags.Select(t => t.TagId)).ToList();
                    var tagsToRemove = existingTags.Where(t => !newTagIds.Contains(t.TagId)).ToList();

                    foreach (var tag in tagsToRemove)
                    {
                        _context.PostTag.Remove(tag);
                    }

                    foreach (var tagId in tagsToAdd)
                    {
                        _context.PostTag.Add(new PostTag { PostId = id, TagId = tagId });
                    }

                    if (!string.IsNullOrWhiteSpace(viewmodel.NewTag))
                    {
                        var tagNames = viewmodel.NewTag.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                                       .Where(t => t.StartsWith("#"))
                                                       .Distinct(StringComparer.OrdinalIgnoreCase);

                        foreach (var tagName in tagNames)
                        {
                            var tag = await _context.Tag.FirstOrDefaultAsync(t => t.Title.ToLower() == tagName.ToLower());
                            if (tag == null)
                            {
                                tag = new Tag { Title = tagName };
                                _context.Tag.Add(tag);
                                await _context.SaveChangesAsync();
                            }

                            if (!existingTags.Any(et => et.TagId == tag.Id))
                            {
                                _context.PostTag.Add(new PostTag { PostId = id, TagId = tag.Id });
                            }
                        }
                    }

                    var existingCategories = await _context.PostCategory.Where(s => s.PostId == id).ToListAsync();
                    var newCategoryIds = viewmodel.SelectedCategories ?? new List<int>();
                    var categoriesToAdd = newCategoryIds.Except(existingCategories.Select(c => c.CategoryId)).ToList();
                    var categoriesToRemove = existingCategories.Where(c => !newCategoryIds.Contains(c.CategoryId)).ToList();

                    foreach (var category in categoriesToRemove)
                    {
                        _context.PostCategory.Remove(category);
                    }

                    foreach (var categoryId in categoriesToAdd)
                    {
                        _context.PostCategory.Add(new PostCategory { PostId = id, CategoryId = categoryId });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(viewmodel.Post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewmodel);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(m => m.Tags).ThenInclude(m => m.Tag)
                .Include(m => m.Categories).ThenInclude(m => m.Category)
                .Include(m=>m.Comments)
                .Include(m=>m.UserPs)
                .Include(m=>m.Likes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            if (post != null)
            {
                _context.Post.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
