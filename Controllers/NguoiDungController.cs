using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLDuAn.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QLDuAn.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NguoiDungController : Controller
    {
        private readonly QlduAnContext _context;

        public NguoiDungController(QlduAnContext context)
        {
            _context = context;
        }

        // GET: NguoiDung/Index (Danh sách người dùng)
        public async Task<IActionResult> Index(int? maVaiTro, int? maTo)
        {
            // Giả định: Chỉ quản trị viên (MaVaiTro = 1) được truy cập
            var userVaiTro = 1; // Lấy từ User.Identity
            if (userVaiTro != 1)
            {
                return Forbid("Chỉ quản trị viên được phép truy cập.");
            }

            var nguoiDungQuery = _context.NguoiDungs
                .Include(n => n.MaVaiTroNavigation)
                .Include(n => n.MaToNavigation)
                .AsQueryable();

            if (maVaiTro.HasValue)
            {
                nguoiDungQuery = nguoiDungQuery.Where(n => n.MaVaiTro == maVaiTro);
            }

            if (maTo.HasValue)
            {
                nguoiDungQuery = nguoiDungQuery.Where(n => n.MaTo == maTo);
            }

            var nguoiDungs = await nguoiDungQuery.ToListAsync();
            ViewBag.VaiTroList = new SelectList(await _context.VaiTros.ToListAsync(), "MaVaiTro", "TenVaiTro");
            ViewBag.ToList = new SelectList(await _context.ToChuyenMons.ToListAsync(), "MaTo", "TenTo");
            return View(nguoiDungs);
        }

        // GET: NguoiDung/Create
        public IActionResult Create()
        {
            ViewBag.VaiTroList = new SelectList(_context.VaiTros, "MaVaiTro", "TenVaiTro");
            ViewBag.ToList = new SelectList(_context.ToChuyenMons, "MaTo", "TenTo");
            return View();
        }

        // POST: NguoiDung/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NguoiDung nguoiDung, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("Password", "Mật khẩu không được để trống.");
            }
            else if (_context.NguoiDungs.Any(n => n.Email == nguoiDung.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
            }
            else
            {
                nguoiDung.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                nguoiDung.TrangThai = nguoiDung.TrangThai ?? true; // Mặc định kích hoạt
                _context.Add(nguoiDung);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo tài khoản thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.VaiTroList = new SelectList(_context.VaiTros, "MaVaiTro", "TenVaiTro", nguoiDung.MaVaiTro);
            ViewBag.ToList = new SelectList(_context.ToChuyenMons, "MaTo", "TenTo", nguoiDung.MaTo);
            return View(nguoiDung);
        }

        // GET: NguoiDung/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            ViewBag.VaiTroList = new SelectList(_context.VaiTros, "MaVaiTro", "TenVaiTro", nguoiDung.MaVaiTro);
            ViewBag.ToList = new SelectList(_context.ToChuyenMons, "MaTo", "TenTo", nguoiDung.MaTo);
            return View(nguoiDung);
        }

        // POST: NguoiDung/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NguoiDung nguoiDung, string? newPassword)
        {
            if (id != nguoiDung.MaNguoiDung)
            {
                return NotFound();
            }

            try
            {
                var existingUser = await _context.NguoiDungs
                    .FirstOrDefaultAsync(n => n.Email == nguoiDung.Email && n.MaNguoiDung != id);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại.");
                }
                else
                {
                    var user = await _context.NguoiDungs.FindAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    user.HoTen = nguoiDung.HoTen;
                    user.Email = nguoiDung.Email;
                    user.MaVaiTro = nguoiDung.MaVaiTro;
                    user.MaTo = nguoiDung.MaTo;
                    user.TrangThai = nguoiDung.TrangThai;

                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật tài khoản thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NguoiDungExists(nguoiDung.MaNguoiDung))
                {
                    return NotFound();
                }
                throw;
            }

            ViewBag.VaiTroList = new SelectList(_context.VaiTros, "MaVaiTro", "TenVaiTro", nguoiDung.MaVaiTro);
            ViewBag.ToList = new SelectList(_context.ToChuyenMons, "MaTo", "TenTo", nguoiDung.MaTo);
            return View(nguoiDung);
        }

        // POST: NguoiDung/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.CongViecs)
                .Include(n => n.DuAns)
                .Include(n => n.TaiLieus)
                .FirstOrDefaultAsync(n => n.MaNguoiDung == id);

            if (nguoiDung == null)
            {
                return NotFound();
            }

            if (nguoiDung.CongViecs.Any() || nguoiDung.DuAns.Any() || nguoiDung.TaiLieus.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa tài khoản vì đã có công việc, dự án hoặc tài liệu liên quan.";
                return RedirectToAction(nameof(Index));
            }

            _context.NguoiDungs.Remove(nguoiDung);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa tài khoản thành công!";
            return RedirectToAction(nameof(Index));
        }

        private bool NguoiDungExists(int id)
        {
            return _context.NguoiDungs.Any(e => e.MaNguoiDung == id);
        }
    }
}