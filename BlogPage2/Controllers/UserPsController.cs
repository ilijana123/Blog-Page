using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogPage2.Data;
using BlogPage2.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using BlogPage2.Areas.Identity.Data;

namespace BlogPage2.Controllers
{
    public class UserPsController : Controller
    {
        private readonly BlogPage2Context _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<BlogPage2User> _userManager;
        public UserPsController(BlogPage2Context context, IHttpContextAccessor httpContextAccessor, UserManager<BlogPage2User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.GetUserId();
        }

        // GET: UserPs
        public async Task<IActionResult> Index()
        {
            var userPs = await _context.UserP.Include(u => u.Post).ToListAsync();
            return View(userPs);
        }

        // GET: UserPs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userP = await _context.UserP
                .Include(u=>u.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userP == null)
            {
                return NotFound();
            }

            return View(userP);
        }

        // GET: UserPs/Create
        public IActionResult Create(int? postId)
        {
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Title");

            UserP userP = new UserP();

            if (postId.HasValue)
            {
                userP.Post = _context.Post.FirstOrDefault(p => p.Id == postId.Value);
            }

            return View(userP);
        }

        // POST: UserPs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PostId,AppUser")] UserP userP)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userP.AppUser = userId;
            if (ModelState.IsValid)
            {
                var post = await _context.Post.FindAsync(userP.PostId);
                if (post == null)
                {
                    ModelState.AddModelError("", "The selected post does not exist.");
                    ViewData["PostId"] = new SelectList(_context.Post, "Id", "Title", userP.PostId);
                    return View(userP);
                }

                userP.Post = post; 

                var existingUserPost = await _context.UserP
                    .FirstOrDefaultAsync(ub => ub.PostId == userP.PostId && ub.AppUser == userP.AppUser);

                if (existingUserPost == null)
                {
                    _context.Add(userP);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "User already has saved this post.");
                    ViewData["PostId"] = new SelectList(_context.Post, "Id", "Title", userP.PostId);
                    return View(userP);
                }
            }

            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Title", userP.PostId);
            return View(userP);
        }


        // GET: UserPs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userP = await _context.UserP.FindAsync(id);
            if (userP == null)
            {
                return NotFound();
            }
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Title", userP.PostId);
            return View(userP);
        }

        // POST: UserPs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostId,AppUser")] UserP userP)
        {
            if (id != userP.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userP);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserPExists(userP.Id))
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
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Title", userP.PostId);
            return View(userP);
        }

        // GET: UserPs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userP = await _context.UserP
                .Include(p=>p.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userP == null)
            {
                return NotFound();
            }

            return View(userP);
        }

        // POST: UserPs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userP = await _context.UserP.FindAsync(id);
            if (userP != null)
            {
                _context.UserP.Remove(userP);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserPExists(int id)
        {
            return _context.UserP.Any(e => e.Id == id);
        }

    }
}
