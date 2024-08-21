using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlogPage2.Areas.Identity.Data;
using BlogPage2.Models;

namespace BlogPage2.Data
{
    public class BlogPage2Context : IdentityDbContext<BlogPage2User>
    {
        public BlogPage2Context(DbContextOptions<BlogPage2Context> options)
            : base(options)
        {
        }

        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Comment> Comment { get; set; } = default!;
        public DbSet<Like> Like { get; set; } = default!;
        public DbSet<Post> Post { get; set; } = default!;
        public DbSet<Tag> Tag { get; set; } = default!;
        public DbSet<UserP> UserP { get; set; } = default!;
        public DbSet<PostCategory> PostCategory { get; set; }
        public DbSet<PostTag> PostTag { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        
        
    }
}
