using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLDuAn.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace QLDuAn.Controllers
{
    public class TaiLieuController : Controller
    {
        private readonly QlduAnContext _context;

        public TaiLieuController(QlduAnContext context)
        {
            _context = context;
        }

        // GET: TaiLieu/Index (Xem tất cả tài liệu, hoặc lọc theo maDuAn)
        public async Task<IActionResult> Index(int? maDuAn)
        {
            var taiLieuQuery = _context.TaiLieus
                .Include(t => t.MaDuAnNavigation)
                .Include(t => t.MaCongViecNavigation)
                .Include(t => t.NguoiUploadNavigation)
                .AsQueryable();

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
            ViewBag.CongViecList = new SelectList(_context.CongViecs, "MaCongViec", "TenCongViec");
            ViewBag.VaiTroList = new SelectList(_context.VaiTros, "MaVaiTro", "TenVaiTro");
            return View();
        }

        // POST: TaiLieu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? maDuAn, int? maCongViec, IFormFile file, int[] vaiTroIds, string[] quyenTruyCaps)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn file để upload.");
            }
            else if (maDuAn.HasValue && !_context.DuAns.Any(d => d.MaDuAn == maDuAn))
            {
                ModelState.AddModelError("maDuAn", "Dự án không tồn tại.");
            }
            else if (maCongViec.HasValue && !_context.CongViecs.Any(c => c.MaCongViec == maCongViec))
            {
                ModelState.AddModelError("maCongViec", "Công việc không tồn tại.");
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
                    MaDuAn = maDuAn.Value,
                    MaCongViec = maCongViec,
                    NguoiUpload = 1, // Giả định người dùng đăng nhập
                    NgayUpload = DateTime.Now
                };

                _context.TaiLieus.Add(taiLieu);
                await _context.SaveChangesAsync();

                // Lưu phân quyền
                if (vaiTroIds != null && quyenTruyCaps != null)
                {
                    for (int i = 0; i < vaiTroIds.Length; i++)
                    {
                        _context.PhanQuyenTaiLieus.Add(new PhanQuyenTaiLieu
                        {
                            MaTaiLieu = taiLieu.MaTaiLieu,
                            MaVaiTro = vaiTroIds[i],
                            QuyenTruyCap = quyenTruyCaps[i]
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Tạo tài liệu thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DuAnList = new SelectList(_context.DuAns, "MaDuAn", "TenDuAn", maDuAn);
            ViewBag.CongViecList = new SelectList(_context.CongViecs, "MaCongViec", "TenCongViec", maCongViec);
            ViewBag.VaiTroList = new SelectList(_context.VaiTros, "MaVaiTro", "TenVaiTro");
            return View();
        }

        // GET: TaiLieu/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var taiLieu = await _context.TaiLieus
                .Include(t => t.PhanQuyenTaiLieus)
                .FirstOrDefaultAsync(t => t.MaTaiLieu == id);
            if (taiLieu == null)
            {
                return NotFound();
            }

            ViewBag.DuAnList = new SelectList(_context.DuAns, "MaDuAn", "TenDuAn", taiLieu.MaDuAn);
            ViewBag.CongViecList = new SelectList(_context.CongViecs, "MaCongViec", "TenCongViec", taiLieu.MaCongViec);
            ViewBag.VaiTroList = new SelectList(_context.VaiTros, "MaVaiTro", "TenVaiTro");
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
                    // Fix for CS0019: Operator '&&' cannot be applied to operands of type 'int' and 'bool'
                    if (taiLieu.MaDuAn > 0 && !_context.DuAns.Any(d => d.MaDuAn == taiLieu.MaDuAn))
                    {
                        ModelState.AddModelError("MaDuAn", "Dự án không tồn tại.");
                    }
                    else if (taiLieu.MaCongViec.HasValue && !_context.CongViecs.Any(c => c.MaCongViec == taiLieu.MaCongViec))
                    {
                        ModelState.AddModelError("MaCongViec", "Công việc không tồn tại.");
                    }
                    else
                    {
                        // Cập nhật thông tin tài liệu
                        _context.Update(taiLieu);

                        // Xóa phân quyền cũ
                        var oldPermissions = await _context.PhanQuyenTaiLieus
                            .Where(p => p.MaTaiLieu == id)
                            .ToListAsync();
                        _context.PhanQuyenTaiLieus.RemoveRange(oldPermissions);

                        // Thêm phân quyền mới
                        if (vaiTroIds != null && quyenTruyCaps != null)
                        {
                            for (int i = 0; i < vaiTroIds.Length; i++)
                            {
                                _context.PhanQuyenTaiLieus.Add(new PhanQuyenTaiLieu
                                {
                                    MaTaiLieu = taiLieu.MaTaiLieu,
                                    MaVaiTro = vaiTroIds[i],
                                    QuyenTruyCap = quyenTruyCaps[i]
                                });
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
            ViewBag.CongViecList = new SelectList(_context.CongViecs, "MaCongViec", "TenCongViec", taiLieu.MaCongViec);
            ViewBag.VaiTroList = new SelectList(_context.VaiTros, "MaVaiTro", "TenVaiTro");
            return View(taiLieu);
        }

        // POST: TaiLieu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taiLieu = await _context.TaiLieus.FindAsync(id);
            if (taiLieu == null)
            {
                return NotFound();
            }

            // Xóa file trên server
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", taiLieu.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Xóa phân quyền
            var permissions = await _context.PhanQuyenTaiLieus
                .Where(p => p.MaTaiLieu == id)
                .ToListAsync();
            _context.PhanQuyenTaiLieus.RemoveRange(permissions);

            // Xóa tài liệu
            _context.TaiLieus.Remove(taiLieu);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa tài liệu thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: TaiLieu/Download/5
        public async Task<IActionResult> Download(int id)
        {
            var taiLieu = await _context.TaiLieus.FindAsync(id);
            if (taiLieu == null)
            {
                return NotFound();
            }

            // Kiểm tra phân quyền (giả định user có MaVaiTro = 1)
            var userVaiTro = 1; // Lấy từ User.Identity
            var hasPermission = await _context.PhanQuyenTaiLieus
                .AnyAsync(p => p.MaTaiLieu == id && p.MaVaiTro == userVaiTro && p.QuyenTruyCap == "Tải");

            if (!hasPermission)
            {
                return Forbid("Bạn không có quyền tải tài liệu này.");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", taiLieu.FilePath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File không tồn tại.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/octet-stream", taiLieu.TenTaiLieu);
        }

        private bool TaiLieuExists(int id)
        {
            return _context.TaiLieus.Any(e => e.MaTaiLieu == id);
        }
    }
}