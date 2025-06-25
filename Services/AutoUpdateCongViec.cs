using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using QLDuAn.Models;
using Microsoft.EntityFrameworkCore;

public class AutoUpdateCongViec : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public AutoUpdateCongViec(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<QlduAnContext>();
                var now = DateTime.Now.Date;

                // Lấy công việc sắp đến hạn hoặc trễ hạn
                var congViecs = _context.CongViecs
                    .Include(cv => cv.MaDuAnNavigation)
                    .Where(cv => cv.Deadline != null && cv.TrangThai != "Hoàn thành")
                    .ToList();

                foreach (var cv in congViecs)
                {
                    if (!cv.MaNguoiDung.HasValue) continue;
                    var deadline = cv.Deadline.Value.ToDateTime(TimeOnly.MinValue);

                    // Nếu công việc đã quá hạn và chưa hoàn thành, chuyển trạng thái sang "Trễ hạn"
                    if (deadline < now && cv.TrangThai != "Trễ hạn")
                    {
                        cv.TrangThai = "Trễ hạn";
                    }

                    if (deadline <= now.AddDays(2) && deadline >= now)
                    {
                        var daCo = _context.ThongBaos.Any(tb =>
                            tb.MaCongViec == cv.MaCongViec &&
                            tb.MaNguoiDung == cv.MaNguoiDung &&
                            tb.TieuDe == "Công việc sắp đến hạn");

                        if (!daCo)
                        {
                            _context.ThongBaos.Add(new ThongBao
                            {
                                TieuDe = "Công việc sắp đến hạn",
                                NoiDung = $"Công việc '{cv.TenCongViec}' của dự án {cv.MaDuAnNavigation.TenDuAn} sắp đến hạn! Deadline:{cv.Deadline}",
                                NgayTao = DateTime.Now,
                                MaNguoiDung = cv.MaNguoiDung.Value,
                                MaDuAn = cv.MaDuAn,
                                MaCongViec = cv.MaCongViec,
                                DaDoc = false
                            });
                        }
                    }
                    else if (deadline < now)
                    {
                        var daCo = _context.ThongBaos.Any(tb =>
                            tb.MaCongViec == cv.MaCongViec &&
                            tb.MaNguoiDung == cv.MaNguoiDung &&
                            tb.TieuDe == "Công việc trễ hạn");

                        if (!daCo)
                        {
                            _context.ThongBaos.Add(new ThongBao
                            {
                                TieuDe = "Công việc trễ hạn",
                                NoiDung = $"Công việc '{cv.TenCongViec}' thuộc dự án {cv.MaDuAnNavigation.TenDuAn} đã trễ hạn!",
                                NgayTao = DateTime.Now,
                                MaNguoiDung = cv.MaNguoiDung.Value,
                                MaDuAn = cv.MaDuAn,
                                MaCongViec = cv.MaCongViec,
                                DaDoc = false
                            });
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }

            // Chờ 30 phút rồi kiểm tra lại
            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}
