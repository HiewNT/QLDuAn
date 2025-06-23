using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLDuAn.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

    public class CongViecController : Controller
    {
        private readonly QlduAnContext _context;

        public CongViecController(QlduAnContext context)
        {
            _context = context;
        }
    // GET: CongViec/Index (Xem tất cả công việc, hoặc lọc theo dự án nếu có maDuAn)
    public async Task<IActionResult> Index(int? maDuAn)
    {
        var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
        ViewBag.UserId = userId;
        ViewBag.UserRole = userRole;

        IQueryable<CongViec> congViecQuery = _context.CongViecs
            .Include(cv => cv.MaDuAnNavigation)
            .Include(cv => cv.MaNguoiDungNavigation)
            .Include(cv => cv.MaToNavigation);

        // Lấy danh sách dự án mà user này phụ trách
        var duAnIdsPhuTrach = _context.DuAns
            .Where(d => d.NguoiPhuTrach == userId)
            .Select(d => d.MaDuAn)
            .ToList();

        if (userRole == "Admin")
        {
            // Không lọc gì, admin xem tất cả
            ViewBag.DuAnList = new SelectList(_context.DuAns, "MaDuAn", "TenDuAn", maDuAn);
        }
        else if (duAnIdsPhuTrach.Any())
        {
            // Người phụ trách dự án: xem tất cả công việc thuộc dự án mình phụ trách
            congViecQuery = congViecQuery.Where(cv => duAnIdsPhuTrach.Contains(cv.MaDuAn));
            ViewBag.DuAnList = new SelectList(
                _context.DuAns.Where(d => duAnIdsPhuTrach.Contains(d.MaDuAn)),
                "MaDuAn", "TenDuAn", maDuAn
            );
        }
        else
        {
            // Nhân viên chỉ xem công việc được giao
            congViecQuery = congViecQuery.Where(cv => cv.MaNguoiDung == userId);
            // Chỉ lấy dự án có công việc của mình cho dropdown
            var duAnIdsCoViec = _context.CongViecs
                .Where(cv => cv.MaNguoiDung == userId)
                .Select(cv => cv.MaDuAn)
                .Distinct()
                .ToList();
            ViewBag.DuAnList = new SelectList(
                _context.DuAns.Where(d => duAnIdsCoViec.Contains(d.MaDuAn)),
                "MaDuAn", "TenDuAn", maDuAn
            );
        }

        if (maDuAn.HasValue)
        {
            congViecQuery = congViecQuery.Where(cv => cv.MaDuAn == maDuAn);
        }

        var congViecs = await congViecQuery.ToListAsync();
        return View(congViecs);
    }

    // GET: CongViec/Details/5
    public async Task<IActionResult> Details(int id)
    {

        var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
        ViewBag.UserId = userId;
        ViewBag.UserRole = userRole;

        var congViec = await _context.CongViecs
            .Include(cv => cv.MaDuAnNavigation)
            .Include(cv => cv.MaNguoiDungNavigation)
            .Include(cv => cv.MaToNavigation)
            .Include(cv => cv.TaiLieus)
            .Include(cv => cv.BinhLuans)
                .ThenInclude(bl => bl.MaNguoiDungNavigation)
            .FirstOrDefaultAsync(cv => cv.MaCongViec == id);

        if (congViec == null)
        {
            return NotFound();
        }

        return View(congViec);
    }
    // GET: CongViec/Create
    public IActionResult Create()
        {
            ViewBag.DuAnList = new SelectList(_context.DuAns, "MaDuAn", "TenDuAn");
            ViewBag.NguoiDungList = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen");
            ViewBag.ToChuyenMonList = new SelectList(_context.ToChuyenMons, "MaTo", "TenTo");
            ViewBag.GiaiDoanList = new SelectList(new[] { "Tiền kỳ", "Quay", "Hậu kỳ" });
            ViewBag.TrangThaiList = new SelectList(new[] { "Chưa bắt đầu", "Đang thực hiện", "Hoàn thành", "Trễ hạn" });
            return View();
        }

        // POST: CongViec/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CongViec congViec)
        {
            
            
            // Kiểm tra MaDuAn tồn tại
            if (!_context.DuAns.Any(d => d.MaDuAn == congViec.MaDuAn))
            {
                ModelState.AddModelError("MaDuAn", "Dự án không tồn tại.");
            }
            else
            {
                _context.Add(congViec);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo công việc thành công!";
            // Sau khi lưu công việc thành công
            if (congViec.MaNguoiDung.HasValue)
            {
                var thongBao = new ThongBao
                {
                    TieuDe = "Công việc mới",
                    NoiDung = $"Bạn được giao công việc: {congViec.TenCongViec} thuộc dự án: {congViec.MaDuAnNavigation.TenDuAn}",
                    NgayTao = DateTime.Now,
                    MaNguoiDung = congViec.MaNguoiDung.Value,
                    MaDuAn = congViec.MaDuAn,
                    MaCongViec = congViec.MaCongViec,
                    DaDoc = false
                };
                _context.ThongBaos.Add(thongBao);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
            }
            

            ViewBag.DuAnList = new SelectList(_context.DuAns, "MaDuAn", "TenDuAn", congViec.MaDuAn);
            ViewBag.NguoiDungList = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen", congViec.MaNguoiDung);
            ViewBag.ToChuyenMonList = new SelectList(_context.ToChuyenMons, "MaTo", "TenTo", congViec.MaTo);
            ViewBag.GiaiDoanList = new SelectList(new[] { "Tiền kỳ", "Quay", "Hậu kỳ" }, congViec.GiaiDoan);
            ViewBag.TrangThaiList = new SelectList(new[] { "Chưa bắt đầu", "Đang thực hiện", "Hoàn thành", "Trễ hạn" }, congViec.TrangThai);
            return View(congViec);
        }

        // GET: CongViec/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var congViec = await _context.CongViecs.FindAsync(id);
            if (congViec == null)
            {
                return NotFound();
            }

            ViewBag.DuAnList = new SelectList(_context.DuAns, "MaDuAn", "TenDuAn", congViec.MaDuAn);
            ViewBag.NguoiDungList = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen", congViec.MaNguoiDung);
            ViewBag.ToChuyenMonList = new SelectList(_context.ToChuyenMons, "MaTo", "TenTo", congViec.MaTo);
            ViewBag.GiaiDoanList = new SelectList(new[] { "Tiền kỳ", "Quay", "Hậu kỳ" }, congViec.GiaiDoan);
            ViewBag.TrangThaiList = new SelectList(new[] { "Chưa bắt đầu", "Đang thực hiện", "Hoàn thành", "Trễ hạn" }, congViec.TrangThai);
            return View(congViec);
        }

        // POST: CongViec/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CongViec congViec)
        {
            
                try
                {
                    // Kiểm tra MaDuAn tồn tại
                    if (!_context.DuAns.Any(d => d.MaDuAn == congViec.MaDuAn))
                    {
                        ModelState.AddModelError("MaDuAn", "Dự án không tồn tại.");
                    }
                    else
                    {
                        _context.Update(congViec);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Cập nhật công việc thành công!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CongViecExists(congViec.MaCongViec))
                    {
                        return NotFound();
                    }
                    throw;
                }
            

            ViewBag.DuAnList = new SelectList(_context.DuAns, "MaDuAn", "TenDuAn", congViec.MaDuAn);
            ViewBag.NguoiDungList = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen", congViec.MaNguoiDung);
            ViewBag.ToChuyenMonList = new SelectList(_context.ToChuyenMons, "MaTo", "TenTo", congViec.MaTo);
            ViewBag.GiaiDoanList = new SelectList(new[] { "Tiền kỳ", "Quay", "Hậu kỳ" }, congViec.GiaiDoan);
            ViewBag.TrangThaiList = new SelectList(new[] { "Chưa bắt đầu", "Đang thực hiện", "Hoàn thành", "Trễ hạn" }, congViec.TrangThai);
            return View(congViec);
        }

        // POST: CongViec/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var congViec = await _context.CongViecs.FindAsync(id);
            if (congViec == null)
            {
                return NotFound();
            }

            // Kiểm tra xem công việc có tài liệu hoặc bình luận liên quan không
            var hasTaiLieu = await _context.TaiLieus.AnyAsync(tl => tl.MaCongViec == id);
            var hasBinhLuan = await _context.BinhLuans.AnyAsync(bl => bl.MaCongViec == id);
            if (hasTaiLieu || hasBinhLuan)
            {
                TempData["ErrorMessage"] = "Không thể xóa công việc vì đã có tài liệu hoặc bình luận liên quan.";
                return RedirectToAction(nameof(Index));
            }

            _context.CongViecs.Remove(congViec);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa công việc thành công!";
            return RedirectToAction(nameof(Index));
        }

        private bool CongViecExists(int id)
        {
            return _context.CongViecs.Any(e => e.MaCongViec == id);
        }
}


