using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDuAn.Models;
using System.Security.Claims;

namespace QLDuAn.Controllers
{
    public class HomeController : Controller
    {
        private readonly QlduAnContext _context;

        public HomeController(QlduAnContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Báo cáo tiến độ dự án
            var duAnProgress = await _context.DuAns
                .Select(d => new {
                    d.TenDuAn,
                    TongCV = d.CongViecs.Count(),
                    HoanThanh = d.CongViecs.Count(cv => cv.TrangThai == "Hoàn thành")
                })
                .ToListAsync();

            // Khối lượng công việc từng tổ
            var toCongViec = await _context.ToChuyenMons
                .Select(to => new {
                    to.TenTo,
                    TongCV = to.CongViecs.Count(),
                    DangLam = to.CongViecs.Count(cv => cv.TrangThai == "Đang thực hiện"),
                    HoanThanh = to.CongViecs.Count(cv => cv.TrangThai == "Hoàn thành"),
                    TreHan = to.CongViecs.Count(cv => cv.TrangThai == "Trễ hạn")
                })
                .ToListAsync();

            ViewBag.DuAnProgress = duAnProgress;
            ViewBag.ToCongViec = toCongViec;

            ViewBag.SoNguoiDung = _context.NguoiDungs.Count();
            ViewBag.SoDuAn = _context.DuAns.Count();
            ViewBag.SoCongViec = _context.CongViecs.Count();
            ViewBag.SoTaiLieu = _context.TaiLieus.Count();

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            var user = await _context.NguoiDungs
                .Include(n => n.MaVaiTroNavigation)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.MaVaiTroNavigation.TenVaiTro ?? "User"),
                    new Claim("UserId", user.MaNguoiDung.ToString()),
                    new Claim("FullName", user.HoTen ?? "")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai email hoặc mật khẩu";
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login");

            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return RedirectToAction("Login");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                ViewBag.Error = "Mật khẩu hiện tại không đúng";
                return View();
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            ViewBag.Success = "Đổi mật khẩu thành công";
            return View();
        }

        [AllowAnonymous]
        public IActionResult AuthError()
        {
            return View();
        }
    }
}
