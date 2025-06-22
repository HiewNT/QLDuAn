using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLDuAn.Helpers;
using QLDuAn.Models;

namespace QLDuAn.Controllers
{
    public class HomeController : Controller
    {
        private readonly QlduAnContext _context;

        public HomeController(QlduAnContext context)
        {
            _context = context;
        }

        // Trang đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            string hashedPassword = SecurityHelper.HashPassword(password);
            var user = _context.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == username && u.MatKhau == hashedPassword);

            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.MaNguoiDung);
                HttpContext.Session.SetString("UserName", user.HoTen ?? "");
                HttpContext.Session.SetString("Role", user.MaVaiTro.ToString());

                return RedirectToAction("Index", "DuAn");
            }

            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu";
            return View();
        }

        // Đăng xuất
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Đổi mật khẩu
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = _context.NguoiDungs.Find(userId);
            if (user == null)
                return RedirectToAction("Login");

            if (user.MatKhau != SecurityHelper.HashPassword(currentPassword))
            {
                ViewBag.Error = "Mật khẩu hiện tại không đúng";
                return View();
            }

            user.MatKhau = SecurityHelper.HashPassword(newPassword);
            _context.SaveChanges();

            ViewBag.Success = "Đổi mật khẩu thành công";
            return View();
        }
    }
}
