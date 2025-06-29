﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLDuAn.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize]
public class DuAnController : Controller
{
    private readonly QlduAnContext _context;

    public DuAnController(QlduAnContext context)
    {
        _context = context;
    }

    // GET: DuAn/Index
    public async Task<IActionResult> Index()
    {
        var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0"); 
        ViewBag.UserId = userId;
        ViewBag.UserRole = userRole;

        IQueryable<DuAn> duAnQuery = _context.DuAns;

        if (userRole != "Admin")
        {
            // Dự án phụ trách
            var duAnIdsPhuTrach = _context.DuAns
                .Where(d => d.NguoiPhuTrach == userId)
                .Select(d => d.MaDuAn);

            // Dự án có công việc của mình
            var duAnIdsCoViec = _context.CongViecs
                .Where(cv => cv.MaNguoiDung == userId)
                .Select(cv => cv.MaDuAn);

            // Gộp lại, loại trùng
            var duAnIds = duAnIdsPhuTrach.Union(duAnIdsCoViec).Distinct();

            duAnQuery = duAnQuery.Where(d => duAnIds.Contains(d.MaDuAn));
        }

        var duAns = await duAnQuery
            .Include(d => d.NguoiPhuTrachNavigation)
            .ToListAsync();

        ViewBag.NguoiDungList = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen");
        return View(duAns);
    }

    // GET: DuAn/Create
    public IActionResult Create()
    {
        ViewBag.NguoiDungList = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen");
        ViewBag.TrangThaiList = new SelectList(new[] { "Chuẩn bị", "Đang thực hiện", "Hoàn thành" }, "Đang thực hiện");
        return View();
    }

    // POST: DuAn/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DuAn duAn)
    {
        duAn.TrangThai ??= "Chuẩn bị"; // Đặt mặc định nếu chưa có
        _context.Add(duAn);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Tạo dự án thành công!";
        ViewBag.NguoiDungList = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen", duAn.NguoiPhuTrach);;
        return RedirectToAction(nameof(Index));
    }

    // GET: DuAn/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var duAn = await _context.DuAns.FindAsync(id);
        if (duAn == null)
        {
            return NotFound();
        }

        ViewBag.NguoiDungList = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen", duAn.NguoiPhuTrach);
        ViewBag.TrangThaiList = new SelectList(new[] { "Chuẩn bị", "Đang thực hiện", "Hoàn thành" }, duAn.TrangThai);
        return View(duAn);
    }

    // POST: DuAn/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DuAn duAn)
    {
            try
            {
                _context.Update(duAn);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật dự án thành công!";
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DuAnExists(duAn.MaDuAn))
                {
                    return NotFound();
                }
                throw;
            }
        

        ViewBag.NguoiDungList = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen", duAn.NguoiPhuTrach);
        ViewBag.TrangThaiList = new SelectList(new[] { "Chuẩn bị", "Đang thực hiện", "Hoàn thành" }, duAn.TrangThai);
        return RedirectToAction(nameof(Index));
    }

    // POST: DuAn/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var duAn = await _context.DuAns.FindAsync(id);
        if (duAn == null)
        {
            return NotFound();
        }


        _context.DuAns.Remove(duAn);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Xóa dự án thành công!";
        return RedirectToAction(nameof(Index));
    }

    private bool DuAnExists(int id)
    {
        return _context.DuAns.Any(e => e.MaDuAn == id);
    }

    // GET: DuAn/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0"); 
        ViewBag.UserId = userId;
        ViewBag.UserRole = userRole;

        var duAn = await _context.DuAns
            .Include(d => d.NguoiPhuTrachNavigation) // Load thông tin người phụ trách
            .Include(d => d.CongViecs) // Load danh sách công việc
                .ThenInclude(cv => cv.MaNguoiDungNavigation) // Load người thực hiện công việc
            .Include(d => d.CongViecs)
                .ThenInclude(cv => cv.MaToNavigation) // Load tổ chuyên môn
            .FirstOrDefaultAsync(m => m.MaDuAn == id);

        if (duAn == null)
        {
            return NotFound();
        }

        return View(duAn);
    }



    [HttpPost]
    public async Task<IActionResult> GenerateReport(int? id = null)
    {
        IQueryable<DuAn> query = _context.DuAns
            .Include(d => d.NguoiPhuTrachNavigation)
            .Include(d => d.CongViecs)
                .ThenInclude(cv => cv.MaNguoiDungNavigation)
            .Include(d => d.CongViecs)
                .ThenInclude(cv => cv.MaToNavigation);
        var time = DateTime.Now;
        if (id.HasValue)
        {
            query = query.Where(d => d.MaDuAn == id.Value);
        }

        var duAns = await query.ToListAsync();

        if (!duAns.Any())
        {
            return NotFound();
        }

        var pdfBytes = BaoCaoGenerator.GeneratePdf(duAns);
        return File(pdfBytes, "application/pdf", $"BaoCao_DuAn_{(id.HasValue ? id.ToString() : "TatCa")}_{time}.pdf");
    }
}