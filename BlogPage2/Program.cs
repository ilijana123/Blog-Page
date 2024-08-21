using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BlogPage2.Data;
using BlogPage2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using BlogPage2.Areas.Identity.Data;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;
using BlogPage2.Areas.Identity.Pages.Account;
using System.Security.Claims;
using BlogPage.Models;
using BlogPage2.Areas.Identity.Pages.Account;
var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
builder.Services.AddDbContext<BlogPage2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BlogPage2Context") ?? throw new InvalidOperationException("Connection string 'BlogPage2Context' not found.")));

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddRazorPages();
builder.Services.AddIdentity<BlogPage2User, IdentityRole>().AddEntityFrameworkStores<BlogPage2Context>().AddDefaultTokenProviders();
builder.Services.AddDirectoryBrowser();
// Add services to the container.
builder.Services.AddControllersWithViews()
 .AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.WriteIndented = true;
     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
 })
.AddRazorPagesOptions(options =>
{
    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
});

//Password Strength Setting
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;
    options.Password.RequiredUniqueChars = 6;
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.AllowedForNewUsers = true;
    // User settings
    options.User.RequireUniqueEmail = true;
    options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
});
//Setting the Account Login page
builder.Services.AddSingleton<IEmailSender, EmailSender>();
//builder.Services.AddSingleton<IWebHostEnvironment, WebHostEnvironment>();
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Posts}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();