using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDuAn.Models;
using System.Security.Claims;

namespace QLDuAn.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly QlduAnContext _context;

        public HomeController(QlduAnContext context)
        {
            _context = context;
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {

            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role != "Admin")
                return RedirectToAction("Profile", "Home");
            // Báo cáo tiến độ dự án
            var duAnProgress = await _context.DuAns
                .Select(d => new {
                    d.TenDuAn,
                    d.TrangThai,
                    TongCV = d.CongViecs.Count(),
                    HoanThanh = d.CongViecs.Count(cv => cv.TrangThai == "Hoàn thành")
                })
                .ToListAsync();


            ViewBag.DuAnProgress = duAnProgress;

            ViewBag.SoNguoiDung = _context.NguoiDungs.Count();
            ViewBag.SoTo = _context.ToChuyenMons.Count();
            ViewBag.SoDuAn = _context.DuAns.Count();
            ViewBag.SoCongViec = _context.CongViecs.Count();
            ViewBag.SoTaiLieu = _context.TaiLieus.Count();

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (userRole == "Admin")
                    return RedirectToAction("Index", "Home");
                else
                    return RedirectToAction("Profile", "Home");
            }

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

                // Nếu có returnUrl hợp lệ → ưu tiên dùng
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                // Nếu không có returnUrl → phân nhánh theo vai trò
                if (user.MaVaiTroNavigation.TenVaiTro == "Admin")
                    return RedirectToAction("Index", "Home");
                else
                    return RedirectToAction("Profile", "Home");
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


        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

            if (userId == 0)
            {
                return Unauthorized();
            }

            var user = await _context.NguoiDungs
                .Include(u => u.MaVaiTroNavigation)
                .Include(u => u.MaToNavigation)
                .FirstOrDefaultAsync(u => u.MaNguoiDung == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Lấy danh sách dự án mà user đang quản lý
            var duAnQuanLy = await _context.DuAns
                .Where(d => d.NguoiPhuTrach == userId)
                .Select(d => new {
                    d.MaDuAn,
                    d.TenDuAn,
                    d.TrangThai,
                    d.NgayBatDau,
                    d.NgayKetThuc,
                    TongCongViec = d.CongViecs.Count(),
                    CongViecHoanThanh = d.CongViecs.Count(cv => cv.TrangThai == "Hoàn thành"),
                    TienDo = d.CongViecs.Count() > 0 ?
                        (int)Math.Round((double)d.CongViecs.Count(cv => cv.TrangThai == "Hoàn thành") / d.CongViecs.Count() * 100) : 0
                })
                .ToListAsync();

            ViewBag.DuAnQuanLy = duAnQuanLy;

            return View(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(string HoTen, string Email, string SoDienThoai = null)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

                if (userId == 0)
                {
                    return Json(new { success = false, message = "Không xác định được người dùng" });
                }

                var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.MaNguoiDung == userId);

                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng" });
                }

                // Kiểm tra email đã tồn tại (trừ email của chính user)
                var existingUser = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.Email == Email && u.MaNguoiDung != userId);

                if (existingUser != null)
                {
                    return Json(new { success = false, message = "Email đã được sử dụng bởi người dùng khác" });
                }

                // Cập nhật thông tin
                user.HoTen = HoTen?.Trim();
                user.Email = Email?.Trim();

                // Nếu có trường SoDienThoai trong model
                // user.SoDienThoai = SoDienThoai?.Trim();

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật thông tin thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserStats()
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

                if (userId == 0)
                {
                    return Json(new { success = false });
                }

                // Thống kê công việc
                var congViecDangThucHien = await _context.CongViecs
                    .CountAsync(cv => cv.MaNguoiDung == userId &&
                                     (cv.TrangThai == "Đang thực hiện" || cv.TrangThai == "Chưa bắt đầu"));

                var congViecHoanThanh = await _context.CongViecs
                    .CountAsync(cv => cv.MaNguoiDung == userId && cv.TrangThai == "Hoàn thành");

                var tongCongViec = congViecDangThucHien + congViecHoanThanh;
                var tiLeHoanThanh = tongCongViec > 0 ? (int)Math.Round((double)congViecHoanThanh / tongCongViec * 100) : 0;

                // Thống kê thông báo chưa đọc
                var thongBaoChuaDoc = await _context.ThongBaos
                    .CountAsync(tb => tb.MaNguoiDung == userId && (tb.DaDoc == false || tb.DaDoc == null));

                return Json(new
                {
                    success = true,
                    congViecDangThucHien = congViecDangThucHien,
                    congViecHoanThanh = congViecHoanThanh,
                    thongBaoChuaDoc = thongBaoChuaDoc,
                    tiLeHoanThanh = tiLeHoanThanh
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
