using System.Collections.Generic;
using System.Text;

namespace AutoHealClick
{
    // ============================================================
    //  پشتیبانی از زبان‌های راست‌به‌چپ (فارسی/عربی)
    //
    //  موتور IMGUI یونیتی نه شکل‌دهی (shaping) حروف را انجام می‌دهد و
    //  نه الگوریتم Bidi دارد؛ بنابراین متن فارسی/عربی خام برعکس و
    //  جدا از هم دیده می‌شود. این کلاس متن منطقی را به متن بصری آماده
    //  برای رسم در GUI تبدیل می‌کند:
    //      1) حروف را به شکل‌های متصل (Presentation Forms) تبدیل می‌کند
    //      2) ترتیب کاراکترها را برای رسم چپ‌به‌راست برمی‌گرداند
    //
    //  متنِ کنسول (MelonLogger) عمداً دست‌نخورده می‌ماند، چون ترمینال
    //  خودش Bidi را درست نمایش می‌دهد.
    //
    //  افزودن حرف جدید: فقط یک ردیف به ToRtl/Table اضافه کنید.
    // ============================================================
    public static class RtlText
    {
        private const char None = '\0';
        private const char Lam = '\u0644';       // ل
        private const char Tatweel = '\u0640';   // ـ (کشیده، فقط اتصال‌دهنده)

        private const int KindRtl = 0;
        private const int KindLtr = 1;
        private const int KindNeutral = 2;

        private sealed class Letter
        {
            public char Isolated;
            public char Final;
            public char Initial;
            public char Medial;

            /// <summary>حرفی که از هر دو طرف می‌چسبد (مثل ب، س، م).</summary>
            public bool Dual => Initial != None;

            /// <summary>حرفی که فقط از سمت راست می‌چسبد (مثل ا، د، ر، و).</summary>
            public bool RightJoining => !Dual && Final != None;
        }

        private static readonly Dictionary<char, Letter> Letters = new Dictionary<char, Letter>();

        // لیگاتورهای «لام + الف»: کلید = الف، مقدار = [شکل جدا، شکل چسبیده]
        private static readonly Dictionary<char, char[]> LamAlef = new Dictionary<char, char[]>
        {
            { '\u0622', new[] { '\uFEF5', '\uFEF6' } }, // لآ
            { '\u0623', new[] { '\uFEF7', '\uFEF8' } }, // لأ
            { '\u0625', new[] { '\uFEF9', '\uFEFA' } }, // لإ
            { '\u0627', new[] { '\uFEFB', '\uFEFC' } }, // لا
        };

        static RtlText()
        {
            // base, isolated, final, initial, medial   (0000 = ندارد)
            string[] rows =
            {
                "0621 FE80 0000 0000 0000", // ء
                "0622 FE81 FE82 0000 0000", // آ
                "0623 FE83 FE84 0000 0000", // أ
                "0624 FE85 FE86 0000 0000", // ؤ
                "0625 FE87 FE88 0000 0000", // إ
                "0626 FE89 FE8A FE8B FE8C", // ئ
                "0627 FE8D FE8E 0000 0000", // ا
                "0628 FE8F FE90 FE91 FE92", // ب
                "0629 FE93 FE94 0000 0000", // ة
                "062A FE95 FE96 FE97 FE98", // ت
                "062B FE99 FE9A FE9B FE9C", // ث
                "062C FE9D FE9E FE9F FEA0", // ج
                "062D FEA1 FEA2 FEA3 FEA4", // ح
                "062E FEA5 FEA6 FEA7 FEA8", // خ
                "062F FEA9 FEAA 0000 0000", // د
                "0630 FEAB FEAC 0000 0000", // ذ
                "0631 FEAD FEAE 0000 0000", // ر
                "0632 FEAF FEB0 0000 0000", // ز
                "0633 FEB1 FEB2 FEB3 FEB4", // س
                "0634 FEB5 FEB6 FEB7 FEB8", // ش
                "0635 FEB9 FEBA FEBB FEBC", // ص
                "0636 FEBD FEBE FEBF FEC0", // ض
                "0637 FEC1 FEC2 FEC3 FEC4", // ط
                "0638 FEC5 FEC6 FEC7 FEC8", // ظ
                "0639 FEC9 FECA FECB FECC", // ع
                "063A FECD FECE FECF FED0", // غ
                "0641 FED1 FED2 FED3 FED4", // ف
                "0642 FED5 FED6 FED7 FED8", // ق
                "0643 FED9 FEDA FEDB FEDC", // ك
                "0644 FEDD FEDE FEDF FEE0", // ل
                "0645 FEE1 FEE2 FEE3 FEE4", // م
                "0646 FEE5 FEE6 FEE7 FEE8", // ن
                "0647 FEE9 FEEA FEEB FEEC", // ه
                "0648 FEED FEEE 0000 0000", // و
                "0649 FEEF FEF0 0000 0000", // ى
                "064A FEF1 FEF2 FEF3 FEF4", // ي
                "0671 FB50 FB51 0000 0000", // ٱ
                "067E FB56 FB57 FB58 FB59", // پ
                "0686 FB7A FB7B FB7C FB7D", // چ
                "0698 FB8A FB8B 0000 0000", // ژ
                "06A9 FB8E FB8F FB90 FB91", // ک
                "06AF FB92 FB93 FB94 FB95", // گ
                "06CC FBFC FBFD FBFE FBFF", // ی
            };

            foreach (string row in rows)
            {
                string[] parts = row.Split(' ');
                if (parts.Length < 5) continue;
                char baseChar = (char)System.Convert.ToInt32(parts[0], 16);
                Letters[baseChar] = new Letter
                {
                    Isolated = (char)System.Convert.ToInt32(parts[1], 16),
                    Final = (char)System.Convert.ToInt32(parts[2], 16),
                    Initial = (char)System.Convert.ToInt32(parts[3], 16),
                    Medial = (char)System.Convert.ToInt32(parts[4], 16),
                };
            }
        }

        /// <summary>متن منطقی را به متن بصری قابل رسم در IMGUI تبدیل می‌کند.</summary>
        public static string Visual(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return Reorder(Shape(text));
        }

        // ---------- مرحله ۱: شکل‌دهی حروف ----------
        private static string Shape(string text)
        {
            char[] src = text.ToCharArray();
            var sb = new StringBuilder(src.Length);

            for (int i = 0; i < src.Length; i++)
            {
                char c = src[i];
                Letter? letter;
                if (!Letters.TryGetValue(c, out letter))
                {
                    sb.Append(c);
                    continue;
                }

                // لام + الف => یک گلیف لیگاتور
                if (c == Lam)
                {
                    int nextIndex = -1;
                    for (int j = i + 1; j < src.Length; j++)
                    {
                        if (!IsTransparent(src[j])) { nextIndex = j; break; }
                    }

                    char[]? lig;
                    if (nextIndex >= 0 && LamAlef.TryGetValue(src[nextIndex], out lig))
                    {
                        sb.Append(JoinsForward(src, i - 1) ? lig[1] : lig[0]);
                        i = nextIndex; // الف مصرف شد
                        continue;
                    }
                }

                bool joinPrev = JoinsForward(src, i - 1);
                bool joinNext = letter.Dual && JoinsBackward(src, i + 1);

                if (letter.Dual)
                {
                    if (joinPrev && joinNext) sb.Append(letter.Medial);
                    else if (joinPrev) sb.Append(letter.Final);
                    else if (joinNext) sb.Append(letter.Initial);
                    else sb.Append(letter.Isolated);
                }
                else
                {
                    sb.Append(joinPrev && letter.RightJoining ? letter.Final : letter.Isolated);
                }
            }

            return sb.ToString();
        }

        // ---------- مرحله ۲: تبدیل ترتیب منطقی به ترتیب بصری ----------
        private static string Reorder(string shaped)
        {
            int n = shaped.Length;
            if (n == 0) return "";

            int[] kinds = new int[n];
            for (int i = 0; i < n; i++) kinds[i] = ClassOf(shaped[i]);

            // قواعد N1/N2 یونیکد: فاصله‌ها و نشانه‌ها جهت متن قوی اطراف
            // خود را می‌گیرند؛ اگر دو طرف متفاوت بود، جهت پایه (راست‌به‌چپ).
            for (int i = 0; i < n; i++)
            {
                if (kinds[i] != KindNeutral) continue;

                int end = i;
                while (end < n && kinds[end] == KindNeutral) end++;

                int before = i > 0 ? kinds[i - 1] : KindRtl;
                int after = end < n ? kinds[end] : KindRtl;
                int resolved = (before == KindLtr && after == KindLtr) ? KindLtr : KindRtl;

                for (int k = i; k < end; k++) kinds[k] = resolved;
                i = end - 1;
            }

            // بازه‌های هم‌جنس: ترتیبشان برعکس می‌شود و بازه‌های
            // راست‌به‌چپ کاراکتر-به-کاراکتر برعکس می‌شوند.
            var sb = new StringBuilder(n);
            int runEnd = n;
            while (runEnd > 0)
            {
                int runStart = runEnd - 1;
                while (runStart > 0 && kinds[runStart - 1] == kinds[runEnd - 1]) runStart--;

                if (kinds[runEnd - 1] == KindRtl)
                {
                    for (int j = runEnd - 1; j >= runStart; j--) sb.Append(shaped[j]);
                }
                else
                {
                    for (int j = runStart; j < runEnd; j++) sb.Append(shaped[j]);
                }
                runEnd = runStart;
            }
            return sb.ToString();
        }

        private static int ClassOf(char c)
        {
            // ارقام عربی/فارسی همیشه چپ‌به‌راست نمایش داده می‌شوند
            if (c >= '\u0660' && c <= '\u0669') return KindLtr;
            if (c >= '\u06F0' && c <= '\u06F9') return KindLtr;

            if (IsArabicScript(c)) return KindRtl;
            return char.IsLetterOrDigit(c) ? KindLtr : KindNeutral;
        }

        private static bool IsArabicScript(char c)
        {
            return (c >= '\u0600' && c <= '\u06FF') ||
                   (c >= '\u0750' && c <= '\u077F') ||
                   (c >= '\u08A0' && c <= '\u08FF') ||
                   (c >= '\uFB50' && c <= '\uFDFF') ||
                   (c >= '\uFE70' && c <= '\uFEFF');
        }

        /// <summary>حرکت‌ها و اعراب: در اتصال حروف نادیده گرفته می‌شوند.</summary>
        private static bool IsTransparent(char c)
        {
            return (c >= '\u0610' && c <= '\u061A') ||
                   (c >= '\u064B' && c <= '\u065F') ||
                   c == '\u0670' ||
                   (c >= '\u06D6' && c <= '\u06DC') ||
                   (c >= '\u06DF' && c <= '\u06E4') ||
                   (c >= '\u06E7' && c <= '\u06E8') ||
                   (c >= '\u06EA' && c <= '\u06ED');
        }

        /// <summary>آیا حرف سمت راست (منطقی) می‌تواند به حرف بعدی بچسبد؟</summary>
        private static bool JoinsForward(char[] src, int index)
        {
            char prev = PrevSignificant(src, index);
            if (prev == None) return false;
            if (prev == Tatweel) return true;
            Letter? l;
            return Letters.TryGetValue(prev, out l) && l.Dual;
        }

        /// <summary>آیا حرف سمت چپ (منطقی) به این حرف می‌چسبد؟</summary>
        private static bool JoinsBackward(char[] src, int index)
        {
            char next = NextSignificant(src, index);
            if (next == None) return false;
            if (next == Tatweel) return true;
            Letter? l;
            return Letters.TryGetValue(next, out l) && (l.Dual || l.RightJoining);
        }

        private static char PrevSignificant(char[] src, int index)
        {
            for (int i = index; i >= 0 && i < src.Length; i--)
                if (!IsTransparent(src[i])) return src[i];
            return None;
        }

        private static char NextSignificant(char[] src, int index)
        {
            for (int i = index; i < src.Length; i++)
            {
                if (i < 0) continue;
                if (!IsTransparent(src[i])) return src[i];
            }
            return None;
        }
    }
}
