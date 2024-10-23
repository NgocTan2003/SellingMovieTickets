using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Services;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;
using System.Data;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<AppUserModel, IdentityRole>()
         .AddEntityFrameworkStores<DataContext>()
         .AddDefaultTokenProviders();

// add sql 
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// add email sender
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSession();
app.UseStaticFiles();

// Set culture to 'vi-VN' for dd/MM/yyyy format
var defaultDateCulture = "vi-VN";
var ci = new CultureInfo(defaultDateCulture);

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ci),
    SupportedCultures = new List<CultureInfo> { ci },
    SupportedUICultures = new List<CultureInfo> { ci }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();

app.UseAuthentication(); // ktra đăng nhập
app.UseAuthorization(); // phân quyền

// Middleware cho việc đã login nhưng cố bấm Back quay về
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (context.User.Identity.IsAuthenticated && !context.User.IsInRole(Role.Customer))
    {
        if (path.StartsWith("/Admin/User/Login", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Redirect("/Admin/HomeDashboard");
            return;
        }
    }
    await next();
});

// Middleware xử lý lỗi 404
app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "Admin",
        areaName: "Admin",
        pattern: "Admin/{controller=HomeDashboard}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
