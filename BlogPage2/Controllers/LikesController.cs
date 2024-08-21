using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogPage2.Data;
using BlogPage2.Models;
using System.Net;
using Microsoft.AspNetCore.Identity;
using BlogPage2.Areas.Identity.Data;
using System.Security.Claims;
using Microsoft.Extensions.Hosting;
namespace BlogPage2.Controllers
{
    public class LikesController : Controller
    {
        private readonly BlogPage2Context _context;
        private readonly UserManager<BlogPage2User> _userManager;
        public LikesController(BlogPage2Context context, UserManager<BlogPage2User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Likes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Like.ToListAsync());
        }

        // GET: Likes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var like = await _context.Like
                .Include(p=>p.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (like == null)
            {
                return NotFound();
            }

            return View(like);
        }

        /*
        // GET: Likes/Create
        public IActionResult Create(int postId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserId = userId;
            var post = _context.Post.Find(postId);
            if (post == null)
            {
                return NotFound(); 
            }
            ViewData["PostId"] = new SelectList(new List<Post> { post }, "Id", "Title");
            return View();
        }

        // POST: Likes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,AppUser")] Like like)
        {
            if (ModelState.IsValid)
            {
                _context.Add(like);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Posts", new { id = like.PostId });
            }
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Title", like.PostId);
            return View(like);
        }
        */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int postId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingLike = _context.Like.FirstOrDefault(l => l.PostId == postId && l.AppUser == userId);

            if (existingLike != null)
            {
                _context.Like.Remove(existingLike);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Post unliked successfully.", isLiked = false });
            }
            else
            {
                var like = new Like
                {
                    PostId = postId,
                    AppUser = userId
                };

                if (ModelState.IsValid)
                {
                    _context.Add(like);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Post liked successfully.", isLiked = true });
                }

                return Json(new { success = false, message = "Failed to like the post." });
            }
        }


        // GET: Likes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var like = await _context.Like.FindAsync(id);
            if (like == null)
            {
                return NotFound();
            }
            return View(like);
        }

        // POST: Likes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostId,AppUser")] Like like)
        {
            if (id != like.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(like);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LikeExists(like.Id))
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
            return View(like);
        }

        // GET: Likes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var like = await _context.Like
                .FirstOrDefaultAsync(m => m.Id == id);
            if (like == null)
            {
                return NotFound();
            }

            return View(like);
        }

        // POST: Likes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var like = await _context.Like.FindAsync(id);
            if (like != null)
            {
                _context.Like.Remove(like);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LikeExists(int id)
        {
            return _context.Like.Any(e => e.Id == id);
        }
    }
}
