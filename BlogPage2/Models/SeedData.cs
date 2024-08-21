using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BlogPage2.Areas.Identity.Data;
using BlogPage2.Data;
using BlogPage2.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPage.Models
{
    public class SeedData
    {
        
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<BlogPage2User>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            BlogPage2User user = await UserManager.FindByEmailAsync("admin@blogpage.com");
            if (user == null)
            {
                var User = new BlogPage2User();
                User.Email = "admin@blogpage.com";
                User.UserName = "admin@blogpage.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }
        
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new BlogPage2Context(
                serviceProvider.GetRequiredService<
                    DbContextOptions<BlogPage2Context>>()))
            
            {
                CreateUserRoles(serviceProvider).Wait();

                if (context.Tag.Any() || context.Like.Any() || context.UserP.Any() || context.Comment.Any() || context.Post.Any() || context.Category.Any())
                {
                    return; // DB has been seeded
                }

                context.Tag.AddRange(
                    new Tag { Title = "#fashionable" },
                    new Tag { Title = "#foodlover" }
                );
                context.SaveChanges();

                context.Category.AddRange(
                    new Category { Title = "Fashion" },
                    new Category { Title = "Food" },
                    new Category { Title = "Sport" }
                );
                context.SaveChanges();

                context.Post.AddRange(
                    new Post
                    {
                        Title = "Dresses",
                        LastModifiedOn = DateTime.Parse("2022-11-15 13:45"),
                        CreatedOn = DateTime.Parse("2021-03-14 21:45"),
                        PostContent = "New dresses for the summer season",
                        Image = "https://www.sherrihill.com/dw/image/v2/BDBR_PRD/on/demandware.static/-/Sites-storefront-catalog-sherrihill/default/dw94c7f7a3/53417_spring_2020_collection.jpg?sw=700&sh=700"
                    },
                    new Post
                    {
                        Title = "T-Shirts",
                        LastModifiedOn = DateTime.Parse("2023-03-14 12:25"),
                        CreatedOn = DateTime.Parse("2020-05-14 17:45"),
                        PostContent = "New T-Shirts",
                        Image = "https://burst.shopifycdn.com/photos/womens-tshirts.jpg?width=373&format=pjpg&exif=0&iptc=0"
                    },
                    new Post
                    {
                        Title = "Shoes",
                        LastModifiedOn = DateTime.Parse("2022-03-17 13:32"),
                        CreatedOn = DateTime.Parse("2018-07-17 18:44"),
                        PostContent = "Active wear.",
                        Image = "https://baccabucci.com/cdn/shop/products/MG_5242_900x.jpg?v=1633514122"
                    },
                    new Post
                    {
                        Title = "Pizza",
                        LastModifiedOn = DateTime.Parse("2024-06-06 11:45"),
                        CreatedOn = DateTime.Parse("2024-06-05 10:45"),
                        PostContent = "Delicious",
                        Image = "https://images.unsplash.com/photo-1670757781705-9b6cb1ad0ca6?q=80&w=851&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"
                    },
                    new Post
                    {
                        Title = "Sushi",
                        LastModifiedOn = DateTime.Parse("2024-06-27 13:22"),
                        CreatedOn = DateTime.Parse("2024-06-20 13:33"),
                        PostContent = "Sushi goals",
                        Image = "https://cdn.prod.website-files.com/60414b21f1ffcdbb0d5ad688/660c3354769535386361ebf6_K_wxFZValqlK-cAVQY34WQ_aYvpq4krW6c4YKh5bCtRakQFCxydyMGG2sApOXPmh4kWY0WvNLEe2LOJLs02GS1zKwxT6j2IQxaNvjgnoSrFdjpO_A9qxIHTfj7jUUf_qqb8Tu0lPwz6v9ratOjo3xS0.jpeg"
                    },
                    new Post
                    {
                        Title = "Milkshake",
                        LastModifiedOn = DateTime.Parse("2024-03-14 09:45"),
                        CreatedOn = DateTime.Parse("2023-03-19 20:45"),
                        PostContent = "Life is better with a milkshake in hand.",
                        Image = "https://www.dessertfortwo.com/wp-content/uploads/2022/08/How-to-Make-a-Milkshake-11-735x1103.jpg"
                    },
                    new Post
                    {
                        Title = "Basketball",
                        LastModifiedOn = DateTime.Parse("2023-07-14 21:45"),
                        CreatedOn = DateTime.Parse("2022-08-18 22:22"),
                        PostContent = "Obstacles don't have to stop you.",
                        Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/73/Chicago_Bulls_and_New_Jersey_Nets%2C_March_28%2C_1991.jpg/640px-Chicago_Bulls_and_New_Jersey_Nets%2C_March_28%2C_1991.jpg"
                    },
                    new Post
                    {
                        Title = "Football",
                        LastModifiedOn = DateTime.Parse("2005-03-14 13:40"),
                        CreatedOn = DateTime.Parse("2001-03-14 09:40"),
                        PostContent = "Train hard.",
                        Image = "https://img.olympics.com/images/image/private/t_s_pog_staticContent_hero_xl_2x/f_auto/primary/ngdjbafv3twathukjbq2"
                    }
                );
                context.SaveChanges();

                context.PostCategory.AddRange(
                    new PostCategory { PostId = 1, CategoryId = 1 },
                    new PostCategory { PostId = 2, CategoryId = 1 },
                    new PostCategory { PostId = 3, CategoryId = 1 },
                    new PostCategory { PostId = 4, CategoryId = 2 },
                    new PostCategory { PostId = 5, CategoryId = 2 },
                    new PostCategory { PostId = 6, CategoryId = 2 },
                    new PostCategory { PostId = 7, CategoryId = 3 }
                );
                context.SaveChanges();

                context.PostTag.AddRange(
                    new PostTag { PostId = 1, TagId = 1 },
                    new PostTag { PostId = 4, TagId = 2 }
                );
                context.SaveChanges();

                context.UserP.AddRange(
                    new UserP { PostId = 1, AppUser = "ilijana123@gmail.com" },
                    new UserP { PostId = 1, AppUser = "ilijana123@gmail.com" },
                    new UserP { PostId = 2, AppUser = "ilijana123@gmail.com" },
                    new UserP { PostId = 3, AppUser = "ilijana123@gmail.com" },
                    new UserP { PostId = 4, AppUser = "ilijana123@gmail.com" },
                    new UserP { PostId = 5, AppUser = "ilijana123@gmail.com" }
                );
                context.SaveChanges();

                context.Like.AddRange(
                    new Like { PostId = 1, AppUser = "ilijana123@gmail.com" },
                    new Like { PostId = 1, AppUser = "ilijana123@gmail.com" }
                );
                context.SaveChanges();

                context.Comment.AddRange(
                    new Comment
                    {
                        CommentContent = "Great.",
                        LastModifiedOn = DateTime.Parse("2000-03-14 13:40"),
                        CreatedOn = DateTime.Parse("2001-03-14 09:40"),
                        PostId = 1
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
