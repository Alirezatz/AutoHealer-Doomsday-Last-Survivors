using System.Collections.Generic;
using UnityEngine;

namespace AutoHealClick
{
    // ============================================================
    //  پرچم‌ها بدون هیچ فایل تصویری رسم می‌شوند.
    //
    //  هر پرچم یک «نقشه پیکسلی» است: هر کاراکتر یک خانه و هر خانه یک
    //  رنگ. حرف‌ها در Palette تعریف شده‌اند و '.' یعنی شفاف.
    //  خانه‌های هم‌رنگ افقی ادغام می‌شوند تا تعداد رسم کم بماند.
    //
    //  افزودن پرچم جدید: یک ردیف در Designs با کد زبان (مثلاً "de").
    // ============================================================
    public static class Flags
    {
        private static readonly Dictionary<char, Color> Palette = new Dictionary<char, Color>
        {
            { 'w', new Color(0.97f, 0.97f, 0.97f, 1f) }, // سفید
            { 'r', new Color(0.85f, 0.06f, 0.12f, 1f) }, // سرخ
            { 'b', new Color(0.02f, 0.14f, 0.44f, 1f) }, // آبی نفتی (بریتانیا)
            { 'n', new Color(0.00f, 0.22f, 0.65f, 1f) }, // آبی روشن (روسیه)
            { 'g', new Color(0.09f, 0.45f, 0.20f, 1f) }, // سبز
            { 'k', new Color(0.05f, 0.05f, 0.06f, 1f) }, // مشکی
        };

        private static readonly Color Fallback = new Color(0.32f, 0.35f, 0.42f, 1f);

        private static readonly Dictionary<string, string[]> Designs = new Dictionary<string, string[]>
        {
            // English - Union Jack ساده‌شده
            {
                "gb", new[]
                {
                    "wbbbbbwrrwbbbbbw",
                    "bwwbbbwrrwbbbwwb",
                    "bbbwbbwrrwbbwbbb",
                    "bbbbwwwrrwwwbbbb",
                    "wwwwwwwrrwwwwwww",
                    "rrrrrrrrrrrrrrrr",
                    "wwwwwwwrrwwwwwww",
                    "bbbbwwwrrwwwbbbb",
                    "bbbwbbwrrwbbwbbb",
                    "bwwbbbwrrwbbbwwb",
                    "wbbbbbwrrwbbbbbw",
                }
            },
            // فارسی - ایران
            {
                "ir", new[]
                {
                    "gggggggggggggggg",
                    "gggggggggggggggg",
                    "gggggggggggggggg",
                    "gggggggggggggggg",
                    "wwwwwwwwwwwwwwww",
                    "wwwwwwwrrwwwwwww",
                    "wwwwwwwwwwwwwwww",
                    "rrrrrrrrrrrrrrrr",
                    "rrrrrrrrrrrrrrrr",
                    "rrrrrrrrrrrrrrrr",
                    "rrrrrrrrrrrrrrrr",
                }
            },
            // العربية - عربستان
            {
                "sa", new[]
                {
                    "gggggggggggggggg",
                    "gggggggggggggggg",
                    "gggwwwwwwwwwwggg",
                    "ggwwwwwwwwwwwwgg",
                    "ggggwwwwwwwwgggg",
                    "gggggggggggggggg",
                    "ggwwwwwwwwwwwwgg",
                    "gggggggggggggggg",
                    "gggggggggggggggg",
                    "gggggggggggggggg",
                    "gggggggggggggggg",
                }
            },
            // Türkçe - ترکیه
            {
                "tr", new[]
                {
                    "rrrrrrrrrrrrrrrr",
                    "rrrrrrrrrrrrrrrr",
                    "rrrrrrrrrrrrrrrr",
                    "rrrwwwwrrrrrrrrr",
                    "rrrwrrrrrrrrrrrr",
                    "rrrwrrrrwrrrrrrr",
                    "rrrwrrrrrrrrrrrr",
                    "rrrwwwwrrrrrrrrr",
                    "rrrrrrrrrrrrrrrr",
                    "rrrrrrrrrrrrrrrr",
                    "rrrrrrrrrrrrrrrr",
                }
            },
            // Русский - روسیه
            {
                "ru", new[]
                {
                    "wwwwwwwwwwwwwwww",
                    "wwwwwwwwwwwwwwww",
                    "wwwwwwwwwwwwwwww",
                    "wwwwwwwwwwwwwwww",
                    "nnnnnnnnnnnnnnnn",
                    "nnnnnnnnnnnnnnnn",
                    "nnnnnnnnnnnnnnnn",
                    "rrrrrrrrrrrrrrrr",
                    "rrrrrrrrrrrrrrrr",
                    "rrrrrrrrrrrrrrrr",
                    "rrrrrrrrrrrrrrrr",
                }
            },
        };

        /// <summary>پرچم مربوط به کد زبان را داخل مستطیل داده‌شده رسم می‌کند.</summary>
        public static void Draw(Rect rect, string flagId)
        {
            string[]? rows;
            if (string.IsNullOrEmpty(flagId) || !Designs.TryGetValue(flagId, out rows) || rows.Length == 0)
            {
                Fill(rect, Fallback);
                return;
            }

            int cols = rows[0].Length;
            Color prev = GUI.color;

            for (int row = 0; row < rows.Length; row++)
            {
                // مرزها گِرد می‌شوند تا بین خانه‌ها درز نیفتد
                float y1 = rect.y + Mathf.Round(row * rect.height / rows.Length);
                float y2 = rect.y + Mathf.Round((row + 1) * rect.height / rows.Length);
                string line = rows[row];

                int col = 0;
                while (col < cols)
                {
                    char ch = col < line.Length ? line[col] : '.';
                    int start = col;
                    while (col < cols && (col < line.Length ? line[col] : '.') == ch) col++;

                    if (ch == '.') continue;
                    Color color;
                    if (!Palette.TryGetValue(ch, out color)) continue;

                    float x1 = rect.x + Mathf.Round(start * rect.width / cols);
                    float x2 = rect.x + Mathf.Round(col * rect.width / cols);
                    GUI.color = color;
                    GUI.DrawTexture(new Rect(x1, y1, x2 - x1, y2 - y1), Texture2D.whiteTexture);
                }
            }

            GUI.color = prev;
        }

        private static void Fill(Rect rect, Color color)
        {
            Color prev = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = prev;
        }
    }
}
