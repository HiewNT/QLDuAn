using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDuAn.Models;

public class LichController : Controller
{
    private readonly QlduAnContext _context;

    public LichController(QlduAnContext context)
    {
        _context = context;
    }

    // Hiển thị view lịch
    public IActionResult Index()
    {
        return View();
    }

    // API: trả về dữ liệu JSON cho FullCalendar
    [HttpGet]
    public async Task<IActionResult> GetCongViecLich()
    {
        var data = await _context.CongViecs
            .Include(c => c.MaNguoiDungNavigation)
            .Where(c => c.GiaiDoan == "Sản xuất" || c.GiaiDoan == "Hậu kỳ")
            .Select(c => new
            {
                id = c.MaCongViec,
                title = c.TenCongViec + " - " + (c.MaNguoiDungNavigation != null ? c.MaNguoiDungNavigation.HoTen : "Chưa phân công"),
                end = c.Deadline != null ? DateOnlyToDateTime(c.Deadline.Value) as DateTime? : null,
                backgroundColor = c.GiaiDoan == "Sản xuất" ? "#0d6efd" : "#6f42c1",
                extendedProps = new
                {
                    trangThai = c.TrangThai,
                    giaiDoan = c.GiaiDoan
                }
            })
            .ToListAsync();

        return Json(data);
    }

    private DateTime DateOnlyToDateTime(DateOnly date) => date.ToDateTime(new TimeOnly(8, 0));
}
