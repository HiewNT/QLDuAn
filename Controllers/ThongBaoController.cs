using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDuAn.Models;
using System.Linq;

public class ThongBaoController : Controller
{
    private readonly QlduAnContext _context;
    public ThongBaoController(QlduAnContext context)
    {
        _context = context;
    }

    // API trả về danh sách thông báo chưa đọc cho user hiện tại (dùng AJAX)
    public IActionResult GetAll()
    {
        int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
        var thongBaos = _context.ThongBaos
            .Where(tb => tb.MaNguoiDung == userId)
            .OrderByDescending(tb => tb.NgayTao)
            .Take(10)
            .Select(tb => new {
                tb.TieuDe,
                tb.NoiDung,
                ThoiGian = tb.NgayTao.HasValue ? tb.NgayTao.Value.ToString("HH:mm dd/MM/yyyy") : "",
                tb.MaThongBao,
                DaDoc = tb.DaDoc ?? false,
                MaCongViec = tb.MaCongViec
            })
            .ToList();
        return Json(thongBaos);
    }
    // API đánh dấu tất cả đã đọc
    [HttpPost]
    public IActionResult MarkAllAsRead()
    {
        int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

        var thongBaos = _context.ThongBaos
            .Where(tb => tb.MaNguoiDung == userId && (tb.DaDoc == false || tb.DaDoc == null));

        foreach (var tb in thongBaos)
            tb.DaDoc = true;

        _context.SaveChanges();
        return Ok();
    }

    [HttpPost]
    public IActionResult MarkAsRead(int id)
    {
        var tb = _context.ThongBaos.FirstOrDefault(x => x.MaThongBao == id);
        if (tb != null)
        {
            tb.DaDoc = true;
            _context.SaveChanges();
            return Ok();
        }
        return NotFound();
    }

}