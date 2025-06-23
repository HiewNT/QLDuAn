using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLDuAn.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QLDuAn.Controllers
{
    public class ToChuyenMonController : Controller
    {
        private readonly QlduAnContext _context;

        public ToChuyenMonController(QlduAnContext context)
        {
            _context = context;
        }

        // GET: ToChuyenMon/Index (Danh sách tổ chuyên môn)
        public async Task<IActionResult> Index()
        {
            var toChuyenMons = await _context.ToChuyenMons
                .Include(t => t.NguoiDungs)
                .Include(t => t.CongViecs)
                .ToListAsync();

            // Tính toán khối lượng công việc cho mỗi tổ
            ViewBag.Workload = toChuyenMons.Select(t => new
            {
                t.MaTo,
                TotalTasks = t.CongViecs.Count,
                CompletedTasks = t.CongViecs.Count(c => c.TrangThai == "Hoàn thành"),
                OverdueTasks = t.CongViecs.Count(c => c.TrangThai != "Hoàn thành" && c.Deadline.HasValue && c.Deadline.Value < DateOnly.FromDateTime(DateTime.Now)),
                Members = t.NguoiDungs.Count
            }).ToDictionary(t => t.MaTo, t => t);

            return View(toChuyenMons);
        }

        // GET: ToChuyenMon/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ToChuyenMon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ToChuyenMon toChuyenMon)
        {
                _context.Add(toChuyenMon);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo tổ chuyên môn thành công!";
                return RedirectToAction(nameof(Index));
            
        }

        // GET: ToChuyenMon/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var toChuyenMon = await _context.ToChuyenMons.FindAsync(id);
            if (toChuyenMon == null)
            {
                return NotFound();
            }
            return View(toChuyenMon);
        }

        // POST: ToChuyenMon/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ToChuyenMon toChuyenMon)
        {
            if (id != toChuyenMon.MaTo)
            {
                return NotFound();
            }

                try
                {
                    _context.Update(toChuyenMon);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật tổ chuyên môn thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToChuyenMonExists(toChuyenMon.MaTo))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            
        }

        // POST: ToChuyenMon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toChuyenMon = await _context.ToChuyenMons
                .Include(t => t.NguoiDungs)
                .Include(t => t.CongViecs)
                .FirstOrDefaultAsync(t => t.MaTo == id);

            if (toChuyenMon == null)
            {
                return NotFound();
            }

            if (toChuyenMon.NguoiDungs.Any() || toChuyenMon.CongViecs.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa tổ vì đã có nhân sự hoặc công việc liên quan.";
                return RedirectToAction(nameof(Index));
            }

            _context.ToChuyenMons.Remove(toChuyenMon);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa tổ chuyên môn thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: ToChuyenMon/AssignMembers/5 (Gán nhân sự vào tổ)
        public async Task<IActionResult> AssignMembers(int id)
        {
            var toChuyenMon = await _context.ToChuyenMons.FindAsync(id);
            if (toChuyenMon == null)
            {
                return NotFound();
            }

            var nguoiDungs = await _context.NguoiDungs
                .Where(n => n.MaTo == null)
                .ToListAsync();

            ViewBag.ToChuyenMon = toChuyenMon;
            ViewBag.NguoiDungList = new SelectList(nguoiDungs, "MaNguoiDung", "HoTen");
            return View();
        }

        // POST: ToChuyenMon/AssignMembers/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMembers(int id, int[] maNguoiDungs)
        {
            var toChuyenMon = await _context.ToChuyenMons.FindAsync(id);
            if (toChuyenMon == null)
            {
                return NotFound();
            }

            // Xóa các nhân sự hiện tại khỏi tổ (nếu cần)
            var currentMembers = await _context.NguoiDungs
                .Where(n => n.MaTo == id)
                .ToListAsync();
            foreach (var member in currentMembers)
            {
                member.MaTo = null;
            }

            // Gán nhân sự mới
            if (maNguoiDungs != null)
            {
                foreach (var maNguoiDung in maNguoiDungs)
                {
                    var nguoiDung = await _context.NguoiDungs.FindAsync(maNguoiDung);
                    if (nguoiDung != null)
                    {
                        nguoiDung.MaTo = id;
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Gán nhân sự thành công!";
            return RedirectToAction(nameof(Index));
        }

        private bool ToChuyenMonExists(int id)
        {
            return _context.ToChuyenMons.Any(e => e.MaTo == id);
        }

        // GET: ToChuyenMon/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var to = await _context.ToChuyenMons
                .Include(t => t.NguoiDungs)
                .Include(t => t.CongViecs)
                    .ThenInclude(cv => cv.MaNguoiDungNavigation)
                .FirstOrDefaultAsync(t => t.MaTo == id);

            if (to == null)
                return NotFound();

            return View(to);
        }
    }
}