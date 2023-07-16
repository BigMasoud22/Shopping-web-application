using DataBase;
using DataBase.Services;
using DataBase.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Utility;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
var optionsBuilder = builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
       .AddEntityFrameworkStores<Context>()
       .AddDefaultTokenProviders();

#region Model services
builder.Services.AddScoped<IProductService, ProductServices>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderServices>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartItemService>();
builder.Services.AddScoped<IUsersService, UserService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.GetTempPath()));
#endregion
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.ConfigureApplicationCookie(t =>
{
    t.LoginPath = $"/Identity/Account/Login";
    t.LogoutPath = $"/Identity/Account/Logout";
    t.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddMemoryCache();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Main/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseFileServer(new FileServerOptions()
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), @"node_modules")),
    RequestPath = new PathString("/node_modules"),
    EnableDirectoryBrowsing = true
});
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
//چک میکنیم که آیا کاربری با نقش ادمین اصلی در دیتا بیس هست یا نه 
//اولین لاگین باید به نام ادمین اصلی سایت باشد
var service = new UserService();
if (!service.IsSuperAdminExitst())
{
    app.MapControllerRoute(
        name: "Admin",
        pattern: "{area=Admin}/{controller=Account}/{action=LoginSuperAdmin}/{id?}");
}
else
{
    app.MapControllerRoute(
        name: "default",
        pattern: "{area=Main}/{controller=Home}/{action=Index}");
}
app.Run();

