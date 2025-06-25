using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLDuAn.Models;
using System.Linq;
using System.Security.Claims;

[Authorize]
public class LichController : Controller
{
    private readonly QlduAnContext _context;
    public LichController(QlduAnContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Get all users for the dropdown
        var users = _context.NguoiDungs.Select(u => new
        {
            u.MaNguoiDung,
            u.HoTen
        }).ToList();

        // Get all projects for the dropdown
        var projects = _context.DuAns.Select(d => new
        {
            d.MaDuAn,
            d.TenDuAn
        }).ToList();

        ViewBag.Users = new SelectList(users, "MaNguoiDung", "HoTen");
        ViewBag.Projects = new SelectList(projects, "MaDuAn", "TenDuAn");
        return View();
    }

    // API cho FullCalendar
    [HttpGet]
    public IActionResult GetCongViecLich(int? userId = null, int? projectId = null)
    {
        var currentUserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
        var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        var query = _context.CongViecs
            .Include(cv => cv.MaDuAnNavigation)
            .Include(cv => cv.MaNguoiDungNavigation)
            .Where(cv => cv.NgayBatDau != null && cv.Deadline != null)
            .Where(cv => cv.GiaiDoan == "Sản xuất" || cv.GiaiDoan == "Hậu kỳ");

        if (projectId.HasValue)
        {
            query = query.Where(cv => cv.MaDuAn == projectId.Value);
        }

        if (userRole == "Admin")
        {
            if (userId.HasValue)
                query = query.Where(cv => cv.MaNguoiDung == userId.Value);
        }
        else
        {
            var managedProjectIds = _context.DuAns
                .Where(d => d.NguoiPhuTrach == currentUserId)
                .Select(d => d.MaDuAn)
                .ToList();

            if (managedProjectIds.Any())
            {
                if (userId.HasValue)
                    query = query.Where(cv => managedProjectIds.Contains(cv.MaDuAn) && cv.MaNguoiDung == userId.Value);
                else
                    query = query.Where(cv => managedProjectIds.Contains(cv.MaDuAn));
            }
            else
            {
                query = query.Where(cv => cv.MaNguoiDung == currentUserId);
            }
        }

        var events = query.Select(cv => new
        {
            id = cv.MaCongViec,
            title = cv.TenCongViec,
            start = cv.NgayBatDau.Value.ToString("yyyy-MM-dd"),
            end = cv.Deadline.Value.AddDays(1).ToString("yyyy-MM-dd"),
            giaiDoan = cv.GiaiDoan,
            trangThai = cv.TrangThai,
            duAn = cv.MaDuAnNavigation.TenDuAn,
            nguoiThucHien = cv.MaNguoiDungNavigation.HoTen
        }).ToList();

        return Json(events);
    }
}