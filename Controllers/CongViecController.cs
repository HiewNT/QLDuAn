using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLDuAn.Models;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[Authorize]
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
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
        ViewBag.UserId = userId;
        ViewBag.UserRole = userRole;

        IQueryable<CongViec> congViecQuery = _context.CongViecs
            .Include(cv => cv.MaDuAnNavigation)
            .Include(cv => cv.MaNguoiDungNavigation)
            .Include(cv => cv.MaToNavigation);

        if (userRole == "Admin")
        {
            // Admin: xem tất cả
            ViewBag.DuAnList = new SelectList(_context.DuAns, "MaDuAn", "TenDuAn", maDuAn);
        }
        else
        {
            // Lấy các dự án phụ trách
            var duAnIdsPhuTrach = _context.DuAns
                .Where(d => d.NguoiPhuTrach == userId)
                .Select(d => d.MaDuAn)
                .ToList();

            // Công việc thuộc dự án mình phụ trách
            var congViecThuocDuAnPhuTrach = _context.CongViecs
                .Where(cv => duAnIdsPhuTrach.Contains(cv.MaDuAn));

            var congViecDuocGiaoNgoaiPhuTrach = _context.CongViecs
                .Where(cv => cv.MaNguoiDung == userId && !duAnIdsPhuTrach.Contains(cv.MaDuAn));

            // Gộp lại trước
            var congViecIds = congViecThuocDuAnPhuTrach
                .Select(cv => cv.MaCongViec)
                .Union(congViecDuocGiaoNgoaiPhuTrach.Select(cv => cv.MaCongViec))
                .ToList();

            // Sau đó query lại đầy đủ với Include
            congViecQuery = _context.CongViecs
                .Where(cv => congViecIds.Contains(cv.MaCongViec))
                .Include(cv => cv.MaDuAnNavigation)
                .Include(cv => cv.MaNguoiDungNavigation)
                .Include(cv => cv.MaToNavigation);

            // Tạo dropdown chỉ từ các dự án có liên quan đến công việc mình xem được
            var duAnIdsLienQuan = congViecQuery
                .Select(cv => cv.MaDuAn)
                .Distinct()
                .ToList();

            ViewBag.DuAnList = new SelectList(
                _context.DuAns.Where(d => duAnIdsLienQuan.Contains(d.MaDuAn)),
                "MaDuAn", "TenDuAn", maDuAn
            );
        }

        // Nếu có lọc theo dự án
        if (maDuAn.HasValue)
        {
            congViecQuery = congViecQuery.Where(cv => cv.MaDuAn == maDuAn.Value);
        }

        ViewBag.ToList = new SelectList(await _context.ToChuyenMons.ToListAsync(), "MaTo", "TenTo");

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
    public IActionResult Create(int? maDuAn)
    {
        var model = new CongViec(); // hoặc model phù hợp của bạn
        if (maDuAn.HasValue)
            model.MaDuAn = maDuAn.Value;

        ViewBag.DuAnList = new SelectList(_context.DuAns, "MaDuAn", "TenDuAn", model.MaDuAn); // chọn trước
        ViewBag.NguoiDungList = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen");
        ViewBag.ToList = new SelectList(_context.ToChuyenMons, "MaTo", "TenTo");
        ViewBag.GiaiDoanList = new SelectList(new[] { "Tiền kỳ", "Quay", "Hậu kỳ" });

        return View(model); // truyền model chứa MaDuAn vào View
    }


    // POST: CongViec/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CongViec congViec)
    {

        congViec.TrangThai ??= "Chưa bắt đầu";
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
                // Sau khi SaveChangesAsync()
                var tenDuAn = _context.DuAns
                    .Where(d => d.MaDuAn == congViec.MaDuAn)
                    .Select(d => d.TenDuAn)
                    .FirstOrDefault();

                var thongBao = new ThongBao
                {
                    TieuDe = "Công việc mới",
                    NoiDung = $"Bạn được giao công việc: '{congViec.TenCongViec}' thuộc dự án: '{tenDuAn}'. Dealine: {congViec.Deadline}",
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
        ViewBag.ToList = new SelectList(_context.ToChuyenMons, "MaTo", "TenTo", congViec.MaTo);
        ViewBag.GiaiDoanList = new SelectList(new[] { "Tiền kỳ", "Quay", "Hậu kỳ" }, congViec.GiaiDoan);
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
        ViewBag.ToList = new SelectList(_context.ToChuyenMons, "MaTo", "TenTo", congViec.MaTo);
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
                return RedirectToAction(nameof(Details), new { id = congViec.MaCongViec });
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
        ViewBag.ToList = new SelectList(_context.ToChuyenMons, "MaTo", "TenTo", congViec.MaTo);
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


        _context.CongViecs.Remove(congViec);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Xóa công việc thành công!";
        return RedirectToAction(nameof(Index));
    }

    private bool CongViecExists(int id)
    {
        return _context.CongViecs.Any(e => e.MaCongViec == id);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemBinhLuan(int MaCongViec, string NoiDung)
    {
        if (string.IsNullOrWhiteSpace(NoiDung)) return RedirectToAction("Details", new { id = MaCongViec });

        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
        var binhLuan = new BinhLuan
        {
            MaCongViec = MaCongViec,
            NoiDung = NoiDung,
            NgayTao = DateTime.Now,
            MaNguoiDung = userId
        };

        _context.BinhLuans.Add(binhLuan);
        await _context.SaveChangesAsync();

        // Xử lý tag người dùng (@UserName hoặc @HoTen)
        var danhSachNguoiDung = _context.NguoiDungs.ToList();
        foreach (var nguoi in danhSachNguoiDung)
        {
            if (NoiDung.Contains($"@{nguoi.HoTen}", StringComparison.OrdinalIgnoreCase))
            {
                var thongBao = new ThongBao
                {
                    TieuDe = "Bạn được nhắc đến trong bình luận",
                    NoiDung = $"{nguoi.HoTen}, bạn được {binhLuan.MaNguoiDungNavigation.HoTen} tag trong bình luận tại công việc: {_context.CongViecs.Find(MaCongViec)?.TenCongViec}",
                    NgayTao = DateTime.Now,
                    MaNguoiDung = nguoi.MaNguoiDung,
                    MaCongViec = MaCongViec,
                    DaDoc = false
                };
                _context.ThongBaos.Add(thongBao);
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = MaCongViec });
    }

}


