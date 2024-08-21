using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogPage2.Data;
using BlogPage2.Models;
using BlogPage2.ViewModels;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BlogPage2.Areas.Identity.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
namespace BlogPage2.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly BlogPage2Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<BlogPage2User> _userManager;
        public CommentsController(BlogPage2Context context, IWebHostEnvironment webHostEnvironment, UserManager<BlogPage2User> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }
        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var BlogPage2Context = _context.Comment.Include(r => r.Post);
            return View(await BlogPage2Context.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(r => r.Post)
                .Include(r=>r.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }
        // GET: Comments/Create
        public IActionResult CreateR(int postId, int? parentCommentId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserId = userId;

            var post = _context.Post.Find(postId);
            if(post == null)
            {
                return NotFound();
            };
            ViewData["PostId"] = new SelectList(new List<Post> { post }, "Id", "Title");
            ViewData["ParentCommentId"] = parentCommentId;

            return View();
        }
        // GET: Comments/Create
        public IActionResult Create(int postId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserId = userId;

            var post = _context.Post.Find(postId);
            if (post == null)
            {
                return NotFound();
            };
            ViewData["PostId"] = new SelectList(new List<Post> { post }, "Id", "Title");

            return View();
        }
        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("PostId,AppUser,CommentContent,File")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CreatedOn = DateTime.Now;
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                comment.AppUser = userId;

                if (comment.File != null && comment.File.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + comment.File.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await comment.File.CopyToAsync(fileStream);
                    }

                    comment.Image = uniqueFileName;
                }

                _context.Add(comment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Posts", new { id = comment.PostId });
            }

            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Title", comment.PostId);
            return View(comment);
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateR([Bind("PostId,ParentCommentId,CommentContent,AppUser, File")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CreatedOn = DateTime.Now;

                if (comment.File != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + comment.File.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await comment.File.CopyToAsync(fileStream);
                    }

                    comment.Image = uniqueFileName;
                }

                if (comment.ParentCommentId != 0)
                {
                    var parentComment = await _context.Comment.Include(c => c.Comments)
                                                              .FirstOrDefaultAsync(c => c.Id == comment.ParentCommentId.Value);
                    if (parentComment != null)
                    {
                        parentComment.Comments.Add(comment);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Parent comment does not exist.");
                        return View(comment);
                    }
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Posts", new { id = comment.PostId });
            }

            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Title", comment.PostId);
            ViewData["ParentCommentId"] = comment.ParentCommentId;
            return View(comment);
        }
        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var viewModel = new CommentViewModel
            {
                Comment = comment,
                ExistingImagePath = comment.Image
            };
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Title", comment.PostId);
            return View(viewModel);
        }

        // POST: Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CommentViewModel viewModel, bool DeleteImage)
        {
            if (id != viewModel.Comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var existingComment = await _context.Comment.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

                    if (existingComment == null)
                    {
                        return NotFound();
                    }

                    if (DeleteImage && !string.IsNullOrEmpty(existingComment.Image))
                    {
                        string oldImagePath = Path.Combine(uploadsFolder, existingComment.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        viewModel.Comment.Image = null;
                    }
                    else if (viewModel.File != null)
                    {
                        if (!string.IsNullOrEmpty(existingComment.Image))
                        {
                            string oldImagePath = Path.Combine(uploadsFolder, existingComment.Image);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.File.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewModel.File.CopyToAsync(fileStream);
                        }

                        viewModel.Comment.Image = uniqueFileName;
                    }
                    else if (!string.IsNullOrEmpty(viewModel.ExistingImagePath))
                    {
                        viewModel.Comment.Image = viewModel.ExistingImagePath;
                    }

                    viewModel.Comment.CreatedOn = existingComment.CreatedOn;
                    viewModel.Comment.LastModifiedOn = DateTime.Now;

                    _context.Update(viewModel.Comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(viewModel.Comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Posts", new { id = viewModel.Comment.PostId });
            }
            return View(viewModel);
        }


        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(p=>p.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comment.FindAsync(id);
            var postId = comment.PostId;
            if (comment != null)
            {
                _context.Comment.Remove(comment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Posts", new { id = postId });
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.Id == id);
        }
    }
}
