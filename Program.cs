using Microsoft.EntityFrameworkCore;
using QLDuAn.Models; // Thêm dòng này để nhận diện QlduAnContext

var builder = WebApplication.CreateBuilder(args);

// ✅ Đăng ký DbContext với chuỗi kết nối từ appsettings.json
builder.Services.AddDbContext<QlduAnContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Đăng ký dịch vụ MVC
builder.Services.AddControllersWithViews();

// ✅ Kích hoạt Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Middleware
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Đặt trước Authorization
app.UseAuthorization();

// ✅ Middleware kiểm tra đăng nhập
app.Use(async (context, next) =>
{
    var path = context.Request.Path.ToString().ToLower();

    // Các đường dẫn được truy cập tự do
    var allowedPaths = new[]
    {
        "/home/login",
        "/home/logout",
        "/home/changepassword",
        "/css", "/js", "/lib", "/images"
    };

    bool isAllowed = allowedPaths.Any(p => path.StartsWith(p));
    if (!isAllowed && string.IsNullOrEmpty(context.Session.GetString("UserName")))
    {
        context.Response.Redirect("/home/login");
        return;
    }

    await next();
});

// ✅ Route mặc định trỏ về Home/Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
