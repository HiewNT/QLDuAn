using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLDuAn.Models;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QLDuAn.Controllers
{
    [Authorize]
    public class TaiLieuController : Controller
    {
        private readonly QlduAnContext _context;
        private readonly int adminVaiTroId = 1; // Giả định MaVaiTro của Admin là 1

        public TaiLieuController(QlduAnContext context)
        {
            _context = context;
        }

        // GET: TaiLieu/Index (Xem tất cả tài liệu, hoặc lọc theo maDuAn)
        public async Task<IActionResult> Index(int? maDuAn)
        {
            // Lấy thông tin người dùng từ Claims
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);

            // Xây dựng truy vấn tài liệu
            var taiLieuQuery = _context.TaiLieus
                .Include(t => t.MaDuAnNavigation)
                .Include(t => t.MaCongViecNavigation)
                .Include(t => t.NguoiUploadNavigation)
                .Include(t => t.PhanQuyenTaiLieus)
                .ThenInclude(p => p.MaVaiTroNavigation)
                .AsQueryable();

            if (userRole == "Admin")
            {
                // Admin: Xem tất cả tài liệu có quyền "Xem"
                taiLieuQuery = taiLieuQuery.Where(t => t.PhanQuyenTaiLieus.Any(p => p.QuyenTruyCap == "Xem"));
            }
            else
            {
                // Lấy MaVaiTro của người dùng từ TenVaiTro
                var userRoleId = await _context.VaiTros
                    .Where(v => v.TenVaiTro == userRole)
                    .Select(v => v.MaVaiTro)
                    .FirstOrDefaultAsync();

                // Lấy danh sách dự án mà người dùng phụ trách (Quản lý dự án)
                var managedProjectIds = await _context.DuAns
                    .Where(pc => pc.NguoiPhuTrach == userId)
                    .Select(pc => pc.MaDuAn)
                    .ToListAsync();

                // Lấy danh sách công việc mà người dùng thực hiện
                var assignedTaskIds = await _context.CongViecs
                    .Where(pc => pc.MaNguoiDung == userId)
                    .Select(pc => pc.MaCongViec)
                    .ToListAsync();

                // Lọc tài liệu: Quản lý dự án hoặc Nhân viên công việc
                taiLieuQuery = taiLieuQuery.Where(t =>
                    t.PhanQuyenTaiLieus.Any(p => p.MaVaiTro == userRoleId && p.QuyenTruyCap == "Xem") &&
                    (managedProjectIds.Contains(t.MaDuAn) || (t.MaCongViec.HasValue && assignedTaskIds.Contains(t.MaCongViec.Value)))
                );
            }

            // Lọc thêm theo maDuAn nếu có
            if (maDuAn.HasValue)
            {
                taiLieuQuery = taiLieuQuery.Where(t => t.MaDuAn == maDuAn);
            }

            var taiLieus = await taiLieuQuery.ToListAsync();
            ViewBag.DuAnList = new SelectList(_context.DuAns, "MaDuAn", "TenDuAn", maDuAn);
            return View(taiLieus);
        }

        // GET: TaiLieu/Create
        public IActionResult Create()
        {
            ViewBag.DuAnList = new SelectList(_context.DuAns, "MaDuAn", "TenDuAn");
            ViewBag.CongViecList = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.VaiTroList = new SelectList(_context.VaiTros.Where(v => v.MaVaiTro != adminVaiTroId), "MaVaiTro", "TenVaiTro");
            return View();
        }

        // POST: TaiLieu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int maDuAn, int? maCongViec, IFormFile file, int[] vaiTroIds, string[] quyenTruyCaps)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn file để upload.");
            }
            else if (!_context.DuAns.Any(d => d.MaDuAn == maDuAn))
            {
                ModelState.AddModelError("maDuAn", "Dự án không tồn tại.");
            }
            else if (maCongViec.HasValue && !_context.CongViecs.Any(c => c.MaCongViec == maCongViec && c.MaDuAn == maDuAn))
            {
                ModelState.AddModelError("maCongViec", "Công việc không thuộc dự án đã chọn.");
            }
            else
            {
                // Lưu file vào wwwroot/uploads
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Lưu thông tin tài liệu
                var taiLieu = new TaiLieu
                {
                    TenTaiLieu = file.FileName,
                    FilePath = $"/uploads/{fileName}",
                    MaDuAn = maDuAn,
                    MaCongViec = maCongViec,
                    NguoiUpload = 1, // Giả định người dùng đăng nhập
                    NgayUpload = DateTime.Now
                };

                _context.TaiLieus.Add(taiLieu);
                await _context.SaveChangesAsync();

                // Tự động gán tất cả quyền cho Admin
                string[] allPermissions = { "Xem", "Tải", "Sửa", "Xóa" };
                foreach (var permission in allPermissions)
                {
                    _context.PhanQuyenTaiLieus.Add(new PhanQuyenTaiLieu
                    {
                        MaTaiLieu = taiLieu.MaTaiLieu,
                        MaVaiTro = adminVaiTroId,
                        QuyenTruyCap = permission
                    });
                }

                // Lưu phân quyền từ form (không bao gồm Admin)
                if (vaiTroIds != null && quyenTruyCaps != null)
                {
                    for (int i = 0; i < vaiTroIds.Length; i++)
                    {
                        if (vaiTroIds[i] != 0) // Chỉ thêm nếu vaiTroId hợp lệ
                        {
                            _context.PhanQuyenTaiLieus.Add(new PhanQuyenTaiLieu
                            {
                                MaTaiLieu = taiLieu.MaTaiLieu,
                                MaVaiTro = vaiTroIds[i],
                                QuyenTruyCap = quyenTruyCaps[i]
                            });
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo tài liệu thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DuAnList = new SelectList(_context.DuAns, "MaDuAn", "TenDuAn", maDuAn);
            ViewBag.CongViecList = new SelectList(_context.CongViecs.Where(c => c.MaDuAn == maDuAn), "MaCongViec", "TenCongViec", maCongViec);
            ViewBag.VaiTroList = new SelectList(_context.VaiTros.Where(v => v.MaVaiTro != adminVaiTroId), "MaVaiTro", "TenVaiTro");
            return View();
        }

        // GET: TaiLieu/Edit/5
        public async Task<IActionResult> Edit(int id)
        {

            // Kiểm tra phân quyền (bao gồm cả Admin)
            var userVaiTro = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value); ; // Giả định người dùng đăng nhập
            var hasPermission = await _context.PhanQuyenTaiLieus
                .AnyAsync(p => p.MaTaiLieu == id && p.MaVaiTro == userVaiTro && p.QuyenTruyCap == "Sửa");

            if (!hasPermission)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền sửa tài liệu này.";
                return RedirectToAction("Index", "TaiLieu");
            }

            var taiLieu = await _context.TaiLieus
                .Include(t => t.PhanQuyenTaiLieus)
                .ThenInclude(p => p.MaVaiTroNavigation)
                .FirstOrDefaultAsync(t => t.MaTaiLieu == id);
            if (taiLieu == null)
            {
                return NotFound();
            }

            ViewBag.DuAnList = new SelectList(_context.DuAns, "MaDuAn", "TenDuAn", taiLieu.MaDuAn);
            ViewBag.CongViecList = new SelectList(_context.CongViecs.Where(c => c.MaDuAn == taiLieu.MaDuAn), "MaCongViec", "TenCongViec", taiLieu.MaCongViec);
            ViewBag.VaiTroList = new SelectList(_context.VaiTros.Where(v => v.MaVaiTro != adminVaiTroId), "MaVaiTro", "TenVaiTro");
            return View(taiLieu);
        }

        // POST: TaiLieu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaiLieu taiLieu, int[] vaiTroIds, string[] quyenTruyCaps)
        {
            if (id != taiLieu.MaTaiLieu)
            {
                return NotFound();
            }

            try
            {
                if (!_context.DuAns.Any(d => d.MaDuAn == taiLieu.MaDuAn))
                {
                    ModelState.AddModelError("MaDuAn", "Dự án không tồn tại.");
                }
                else if (taiLieu.MaCongViec.HasValue && !_context.CongViecs.Any(c => c.MaCongViec == taiLieu.MaCongViec && c.MaDuAn == taiLieu.MaDuAn))
                {
                    ModelState.AddModelError("MaCongViec", "Công việc không thuộc dự án đã chọn.");
                }
                else
                {
                    // Cập nhật thông tin tài liệu
                    _context.Update(taiLieu);

                    // Xóa phân quyền cũ (không bao gồm Admin)
                    var oldPermissions = await _context.PhanQuyenTaiLieus
                        .Where(p => p.MaTaiLieu == id && p.MaVaiTro != adminVaiTroId)
                        .ToListAsync();
                    _context.PhanQuyenTaiLieus.RemoveRange(oldPermissions);

                    // Thêm lại tất cả quyền cho Admin
                    string[] allPermissions = { "Xem", "Tải", "Sửa", "Xóa" };
                    foreach (var permission in allPermissions)
                    {
                        if (!_context.PhanQuyenTaiLieus.Any(p => p.MaTaiLieu == id && p.MaVaiTro == adminVaiTroId && p.QuyenTruyCap == permission))
                        {
                            _context.PhanQuyenTaiLieus.Add(new PhanQuyenTaiLieu
                            {
                                MaTaiLieu = taiLieu.MaTaiLieu,
                                MaVaiTro = adminVaiTroId,
                                QuyenTruyCap = permission
                            });
                        }
                    }

                    // Thêm phân quyền mới từ form (không bao gồm Admin)
                    if (vaiTroIds != null && quyenTruyCaps != null)
                    {
                        for (int i = 0; i < vaiTroIds.Length; i++)
                        {
                            if (vaiTroIds[i] != 0) // Chỉ thêm nếu vaiTroId hợp lệ
                            {
                                _context.PhanQuyenTaiLieus.Add(new PhanQuyenTaiLieu
                                {
                                    MaTaiLieu = taiLieu.MaTaiLieu,
                                    MaVaiTro = vaiTroIds[i],
                                    QuyenTruyCap = quyenTruyCaps[i]
                                });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật tài liệu thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaiLieuExists(taiLieu.MaTaiLieu))
                {
                    return NotFound();
                }
                throw;
            }

            ViewBag.DuAnList = new SelectList(_context.DuAns, "MaDuAn", "TenDuAn", taiLieu.MaDuAn);
            ViewBag.CongViecList = new SelectList(_context.CongViecs.Where(c => c.MaDuAn == taiLieu.MaDuAn), "MaCongViec", "TenCongViec", taiLieu.MaCongViec);
            ViewBag.VaiTroList = new SelectList(_context.VaiTros.Where(v => v.MaVaiTro != adminVaiTroId), "MaVaiTro", "TenVaiTro");
            return View(taiLieu);
        }

        //

        // GET: TaiLieu/Download/5
        public async Task<IActionResult> Download(int id)
        {
            var taiLieu = await _context.TaiLieus.FindAsync(id);
            if (taiLieu == null)
            {
                return NotFound();
            }

            // Kiểm tra phân quyền (bao gồm cả Admin)
            var userVaiTro = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value); ; // Giả định người dùng đăng nhập
            var hasPermission = await _context.PhanQuyenTaiLieus
                .AnyAsync(p => p.MaTaiLieu == id && p.MaVaiTro == userVaiTro && p.QuyenTruyCap == "Tải");

            if (!hasPermission)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền tải tài liệu này.";
                return RedirectToAction("Index", "TaiLieu");
            }


            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", taiLieu.FilePath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File không tồn tại.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/octet-stream", taiLieu.TenTaiLieu);
        }

        // GET: TaiLieu/GetCongViecByDuAn
        [HttpGet]
        public async Task<IActionResult> GetCongViecByDuAn(int maDuAn)
        {
            var congViecList = await _context.CongViecs
                .Where(c => c.MaDuAn == maDuAn)
                .Select(c => new { c.MaCongViec, c.TenCongViec })
                .ToListAsync();
            return Json(congViecList);
        }
        // GET: TaiLieu/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (int.IsEvenInteger(id))
            {
                return NotFound();
            }

            var taiLieu = await _context.TaiLieus
                .Include(t => t.MaDuAnNavigation)
                .Include(t => t.MaCongViecNavigation)
                .Include(t => t.NguoiUploadNavigation)
                .FirstOrDefaultAsync(m => m.MaTaiLieu == id);

            if (taiLieu == null)
            {
                return NotFound();
            }

            return View(taiLieu);
        }

        // POST: TaiLieu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            // Kiểm tra phân quyền (bao gồm cả Admin)
            var userVaiTro = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value); ; // Giả định người dùng đăng nhập
            var hasPermission = await _context.PhanQuyenTaiLieus
                .AnyAsync(p => p.MaTaiLieu == id && p.MaVaiTro == userVaiTro && p.QuyenTruyCap == "Xóa");

            if (!hasPermission)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền sửa tài liệu này.";
                return RedirectToAction("Index", "TaiLieu");
            }

            var taiLieu = await _context.TaiLieus.FindAsync(id);
            if (taiLieu != null)
            {
                _context.TaiLieus.Remove(taiLieu);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool TaiLieuExists(int id)
        {
            return _context.TaiLieus.Any(e => e.MaTaiLieu == id);
        }
    }
}