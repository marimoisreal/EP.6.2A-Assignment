using DataAccess.Context;
using EP._6._2A_Assignment.Data;
using EP._6._2A_Assignment.Factories;
using EP._6._2A_Assignment.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// DbContext for ApplicationDbContext 
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("DataAccess")));

// DbContext for Identity
builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("DataAccess")));


// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<IdentityContext>();


builder.Services.AddMemoryCache();

// repository register
builder.Services.AddScoped<ItemsInMemoryRepository>();
builder.Services.AddScoped<IItemsRepository, ItemsDbRepository>();
builder.Services.AddScoped<IItemsRepository, ItemsInMemoryRepository>();
builder.Services.AddRazorPages();

builder.Services.AddScoped<ImportItemFactory>();

var app = builder.Build(); 


// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// **Identity middleware**
app.UseAuthentication(); // added for the login 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();

// admin
var siteAdminEmail = "admin@site.com";
var siteAdminPassword = "Admin123!";

