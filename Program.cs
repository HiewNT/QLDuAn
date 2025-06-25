using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QLDuAn.Models; // Thêm dòng này để nhận diện QlduAnContext

var builder = WebApplication.CreateBuilder(args);

// ✅ Đăng ký DbContext với chuỗi kết nối từ appsettings.json
builder.Services.AddDbContext<QlduAnContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Đăng ký dịch vụ MVC
builder.Services.AddControllersWithViews();
builder.Services.AddHostedService<AutoUpdateCongViec>();

// ✅ Kích hoạt Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Thêm dòng này để cấu hình authentication mặc định
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.AccessDeniedPath = "/Home/AuthError"; // Thêm dòng này
    });


var app = builder.Build();

// Middleware
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Đặt trước Authorization
app.UseAuthentication();
app.UseAuthorization();

// ✅ Route mặc định trỏ về Home/Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

// Route cho các controller khác, mặc định action=Index 
app.MapControllerRoute( name: "controllerDefault", pattern: "{controller}/{action=Index}/{id?}");

app.Run();
