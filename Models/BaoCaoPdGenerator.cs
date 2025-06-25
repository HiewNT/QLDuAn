using OfficeOpenXml;
using QLDuAn.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using IContainer = QuestPDF.Infrastructure.IContainer;

public class BaoCaoGenerator
{
    static BaoCaoGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static byte[] GeneratePdf(List<DuAn> duAnList)
    {
        return Document.Create(container =>
        {
            // Trang tổng quan
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Calibri));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("BÁO CÁO TỔNG QUAN DỰ ÁN")
                            .FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                        column.Item().Text($"Ngày lập: {DateTime.Now:dd/MM/yyyy}")
                            .FontSize(10).Italic();
                    });
                    row.ConstantItem(100).AlignRight().Image(Placeholders.Image(200, 50));
                });

                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Item().Element(ThongKeTongQuan(duAnList));
                    column.Item().PaddingTop(20).Text("Danh sách dự án").FontSize(13).Bold().FontColor(Colors.Blue.Medium);

                    // Danh sách dự án tổng quát
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("STT").Bold();
                            header.Cell().Element(CellStyle).Text("Tên dự án").Bold();
                            header.Cell().Element(CellStyle).Text("Trạng thái").Bold();
                            header.Cell().Element(CellStyle).Text("Tiến độ").Bold();
                        });

                        int stt = 1;
                        foreach (var duAn in duAnList)
                        {
                            var congViecs = duAn.CongViecs.ToList();
                            int tongCV = congViecs.Count;
                            int hoanThanh = congViecs.Count(cv => cv.TrangThai == "Hoàn thành");
                            int tiendo = tongCV > 0 ? (int)Math.Round((double)hoanThanh * 100 / tongCV) : 0;

                            table.Cell().Element(CellStyle).Text((stt++).ToString());
                            table.Cell().Element(CellStyle).Text(duAn.TenDuAn);
                            table.Cell().Element(CellStyle).Text(duAn.TrangThai)
                                .FontColor(GetStatusColor(duAn.TrangThai));
                            table.Cell().Element(CellStyle).Text($"{tiendo}%");
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Trang ").FontSize(8);
                    x.CurrentPageNumber().FontSize(8);
                    x.Span(" / ").FontSize(8);
                    x.TotalPages().FontSize(8);
                });
            });

            // Mỗi dự án một trang riêng
            int sttDuAn = 1;
            foreach (var duAn in duAnList)
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Calibri));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text($"DỰ ÁN {sttDuAn++}: {duAn.TenDuAn}")
                                .FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
                            column.Item().Text($"Ngày lập: {DateTime.Now:dd/MM/yyyy}")
                                .FontSize(10).Italic();
                        });
                        row.ConstantItem(100).AlignRight().Image(Placeholders.Image(200, 50));
                    });

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        var congViecs = duAn.CongViecs.ToList();
                        int tongCV = congViecs.Count;
                        int hoanThanh = congViecs.Count(cv => cv.TrangThai == "Hoàn thành");
                        int tiendo = tongCV > 0 ? (int)Math.Round((double)hoanThanh * 100 / tongCV) : 0;
                        var thanhVien = duAn.NguoiPhuTrachNavigation?.HoTen ?? "N/A";

                        // Thông tin dự án
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Trạng thái: {duAn.TrangThai}")
                                .FontSize(11).FontColor(GetStatusColor(duAn.TrangThai));
                            row.RelativeItem().Text($"Phụ trách: {thanhVien}");
                            row.RelativeItem().Text($"Tiến độ: {tiendo}%");
                        });

                        // Bảng công việc
                        column.Item().PaddingTop(10);
                        if (congViecs.Any())
                        {
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("STT").Bold();
                                    header.Cell().Element(CellStyle).Text("Tên công việc").Bold();
                                    header.Cell().Element(CellStyle).Text("Tổ chuyên môn").Bold();
                                    header.Cell().Element(CellStyle).Text("Trạng thái").Bold();
                                    header.Cell().Element(CellStyle).Text("Người thực hiện").Bold();
                                    header.Cell().Element(CellStyle).Text("Ngày bắt đầu").Bold();
                                    header.Cell().Element(CellStyle).Text("Deadline").Bold();
                                });

                                int stt = 1;
                                foreach (var cv in congViecs)
                                {
                                    table.Cell().Element(CellStyle).Text((stt++).ToString());
                                    table.Cell().Element(CellStyle).Text(cv.TenCongViec);
                                    table.Cell().Element(CellStyle).Text(cv.MaToNavigation?.TenTo ?? "N/A");
                                    table.Cell().Element(CellStyle).Text(cv.TrangThai)
                                        .FontColor(GetStatusColor(cv.TrangThai));
                                    table.Cell().Element(CellStyle).Text(cv.MaNguoiDungNavigation?.HoTen ?? "N/A");
                                    table.Cell().Element(CellStyle).Text(cv.NgayBatDau?.ToString("dd/MM/yyyy") ?? "");
                                    table.Cell().Element(CellStyle).Text(cv.Deadline?.ToString("dd/MM/yyyy") ?? "");
                                }
                            });
                        }
                        else
                        {
                            column.Item().PaddingVertical(5).Text("Không có công việc nào.").Italic();
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Trang ").FontSize(8);
                        x.CurrentPageNumber().FontSize(8);
                        x.Span(" / ").FontSize(8);
                        x.TotalPages().FontSize(8);
                    });
                });
            }
        }).GeneratePdf();
    }

    static Action<IContainer> ThongKeTongQuan(List<DuAn> duAnList)
    {
        int tongDuAn = duAnList.Count;
        int tongCV = duAnList.Sum(d => d.CongViecs.Count);
        int tongHoanThanh = duAnList.Sum(d => d.CongViecs.Count(cv => cv.TrangThai == "Hoàn thành"));
        int duAnHoanThanh = duAnList.Count(d => d.TrangThai == "Hoàn thành");
        double tiLeHoanThanh = tongCV > 0 ? Math.Round((double)tongHoanThanh * 100 / tongCV, 1) : 0;

        return container => container
            .Background(Colors.Grey.Lighten4)
            .Padding(10)
            .Row(row =>
            {
                row.RelativeItem().PaddingRight(5).Border(1f).Padding(5)
                    .Text($"Tổng số dự án: {tongDuAn}").Bold();
                row.RelativeItem().PaddingHorizontal(5).Border(1f).Padding(5)
                    .Text($"Tổng số công việc: {tongCV}").Bold();
                row.RelativeItem().PaddingHorizontal(5).Border(1f).Padding(5)
                    .Text($"Tỉ lệ hoàn thành: {tiLeHoanThanh}%").Bold();
                row.RelativeItem().PaddingLeft(5).Border(1f).Padding(5)
                    .Text($"Dự án hoàn thành: {duAnHoanThanh}").Bold();
            });
    }

    static IContainer CellStyle(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(5)
            .PaddingHorizontal(3)
            .AlignCenter();
    }

    static string GetStatusColor(string trangThai)
    {
        return trangThai switch
        {
            "Hoàn thành" => Colors.Green.Medium,
            "Đang thực hiện" => Colors.Blue.Medium,
            "Chưa bắt đầu" => Colors.Red.Medium,
            _ => Colors.Black
        };
    }
}