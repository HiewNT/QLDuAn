using System.Text.RegularExpressions;

namespace QLDuAn.Helpers
{
    public static class BinhLuanHelper
    {
        public static string HighlightTags(string noiDung, List<string> hoTenNguoiDung)
        {
            foreach (var hoTen in hoTenNguoiDung)
            {
                if (!string.IsNullOrWhiteSpace(hoTen))
                {
                    noiDung = Regex.Replace(noiDung,
                        @$"@{Regex.Escape(hoTen)}",
                        $"<span class='text-primary fw-bold'>@{hoTen}</span>",
                        RegexOptions.IgnoreCase);
                }
            }
            return noiDung;
        }
    }
}
