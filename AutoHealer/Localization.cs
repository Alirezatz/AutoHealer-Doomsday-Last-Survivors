using System.Collections.Generic;

namespace AutoHealClick
{
    // ============================================================
    //  کلیدهای متنی برنامه
    //  برای اضافه کردن متن جدید فقط یک const اینجا بسازید و بعد
    //  همان کلید را در دیکشنری زبان‌هایی که می‌خواهید اضافه کنید.
    //  هر زبانی که کلیدی را نداشته باشد، خودکار به English برمی‌گردد.
    // ============================================================
    public static class Keys
    {
        // عنوان و اعتبار
        public const string Title = "Title";
        public const string PressAnyKey = "PressAnyKey";
        public const string Credit = "Credit";

        // برچسب‌های UI
        public const string SectionStatus = "SectionStatus";
        public const string SectionControls = "SectionControls";
        public const string LabelKey = "LabelKey";
        public const string LabelBatch = "LabelBatch";
        public const string BtnStart = "BtnStart";
        public const string BtnStop = "BtnStop";
        public const string LabelUseSpeedUp = "LabelUseSpeedUp";

        // خط وضعیت
        public const string StatusWaitingGame = "StatusWaitingGame";
        public const string StatusDisconnectedModule = "StatusDisconnectedModule";
        public const string StatusDisconnectedCache = "StatusDisconnectedCache";
        public const string StatusDisconnectedVo = "StatusDisconnectedVo";
        public const string StatusConnected = "StatusConnected";
        public const string StatusError = "StatusError";
        public const string StatsLine = "StatsLine";
        public const string InjuredShort = "InjuredShort";
        public const string HealedShort = "HealedShort";

        // پیام‌های پنل وضعیت
        public const string MsgAutoHealStarted = "MsgAutoHealStarted";
        public const string MsgAutoHealStopped = "MsgAutoHealStopped";
        public const string MsgHealingTroops = "MsgHealingTroops";
        public const string MsgCollectingHealed = "MsgCollectingHealed";
        public const string MsgHealedCollected = "MsgHealedCollected";
        public const string MsgHospitalClear = "MsgHospitalClear";
        public const string MsgCollectingCured = "MsgCollectingCured";
        public const string MsgWaitingInjured = "MsgWaitingInjured";
        public const string MsgNotEnoughResources = "MsgNotEnoughResources";

        // لاگ‌های کنسول
        public const string LogModStarted = "LogModStarted";
        public const string LogLanguageChanged = "LogLanguageChanged";
        public const string LogKeyCancelled = "LogKeyCancelled";
        public const string LogKeySet = "LogKeySet";
        public const string LogCuringInProgress = "LogCuringInProgress";
        public const string LogCureFinished = "LogCureFinished";
        public const string LogStillNotCollected = "LogStillNotCollected";
        public const string LogNotEnoughResources = "LogNotEnoughResources";
        public const string LogCureRejected = "LogCureRejected";
        public const string LogAutoHealError = "LogAutoHealError";
        public const string LogRefreshFailed = "LogRefreshFailed";
        public const string LogHospitalModuleNotReady = "LogHospitalModuleNotReady";
        public const string LogArmyListEmpty = "LogArmyListEmpty";
        public const string LogCureRequestFailed = "LogCureRequestFailed";
        public const string LogCastFailed = "LogCastFailed";
        public const string LogCityCureSent = "LogCityCureSent";
        public const string LogCityCureFailed = "LogCityCureFailed";
        public const string KindCure = "KindCure";
        public const string KindInstantCure = "KindInstantCure";
        public const string LogResourceCheckFailed = "LogResourceCheckFailed";
        public const string LogGuildAssistSent = "LogGuildAssistSent";
        public const string LogGuildAssistFailed = "LogGuildAssistFailed";
        public const string LogStepFailed = "LogStepFailed";
        public const string LogShortageType = "LogShortageType";
        public const string LogShortageUnknown = "LogShortageUnknown";

        // SpeedUP
        public const string LogSpeedUpBagNotReady = "LogSpeedUpBagNotReady";
        public const string LogSpeedUpCacheNull = "LogSpeedUpCacheNull";
        public const string LogSpeedUpNoneInBag = "LogSpeedUpNoneInBag";
        public const string LogSpeedUpUsed = "LogSpeedUpUsed";
        public const string LogSpeedUpFinished = "LogSpeedUpFinished";
        public const string LogSpeedUpFailed = "LogSpeedUpFailed";

        // جمع‌آوری
        public const string LogCollectCacheNull = "LogCollectCacheNull";
        public const string LogCollectButtonClicked = "LogCollectButtonClicked";
        public const string LogCollectCastleNull = "LogCollectCastleNull";
        public const string LogCollectInfoNotFound = "LogCollectInfoNotFound";
        public const string LogCollectShellAllocFailed = "LogCollectShellAllocFailed";
        public const string LogCollectShellClicked = "LogCollectShellClicked";
        public const string LogCollectClickFailed = "LogCollectClickFailed";
    }

    // ============================================================
    //  سیستم چندزبانه
    //
    //  برای اضافه کردن زبان جدید فقط یک مورد در RegisterLanguages()
    //  اضافه کنید؛ نه UI و نه هیچ متد دیگری نیازی به تغییر ندارد:
    //
    //  Add(new Language {
    //      Code = "de", Native = "Deutsch", Flag = "de", Rtl = false,
    //      Strings = new Dictionary<string, string> {
    //          { Keys.Title, "Auto Healer FeStiCal" },
    //          ...
    //      }
    //  });
    //
    //  کلیدهای ترجمه‌نشده خودکار به English برمی‌گردند، پس می‌توانید
    //  کم‌کم ترجمه‌ها را کامل کنید.
    // ============================================================
    public static class Loc
    {
        public sealed class Language
        {
            public string Code = "";
            public string Native = "";

            /// <summary>کد پرچم (مثلاً "gb") که در Flags تعریف شده است.</summary>
            public string Flag = "";

            /// <summary>زبان‌های راست‌به‌چپ علاوه بر ترجمه، چینش و شکل‌دهی هم می‌خواهند.</summary>
            public bool Rtl = false;

            public Dictionary<string, string> Strings = new Dictionary<string, string>();
        }

        private static readonly List<Language> Languages = new List<Language>();
        private static int current = 0;

        static Loc()
        {
            RegisterLanguages();
        }

        // ---------- افزودن زبان‌ها (تنها جایی که برای زبان جدید دست می‌زنید) ----------
        private static void RegisterLanguages()
        {
            Add(new Language
            {
                Code = "en",
                Native = "English",
                Flag = "gb",
                Rtl = false,
                Strings = new Dictionary<string, string>
                {
                    { Keys.Title, "Auto Healer FeStiCal" },
                    { Keys.PressAnyKey, "Press any key... (Esc = cancel)" },
                    { Keys.Credit, "Programmer: {0}" },
                    { Keys.SectionStatus, "STATUS" },
                    { Keys.SectionControls, "CONTROLS" },
                    { Keys.LabelKey, "Key:" },
                    { Keys.LabelBatch, "Soldiers per cure:" },
                    { Keys.BtnStart, "Start Auto Heal" },
                    { Keys.BtnStop, "Stop Auto Heal" },
                    { Keys.LabelUseSpeedUp, "Use SpeedUP" },
                    { Keys.StatusWaitingGame, "WAITING FOR GAME..." },
                    { Keys.StatusDisconnectedModule, "DISCONNECTED - HospitalModule not ready" },
                    { Keys.StatusDisconnectedCache, "DISCONNECTED - HospitalCache null" },
                    { Keys.StatusDisconnectedVo, "DISCONNECTED - HospitalVo null" },
                    { Keys.StatusConnected, "CONNECTED - Hospital {0}% full" },
                    { Keys.StatusError, "ERROR: {0}" },
                    { Keys.StatsLine, "Injured: {0:N0}  |  Curing: {1:N0}  |  Cap: {2:N0}" },
                    { Keys.InjuredShort, "Injured: {0:N0}" },
                    { Keys.HealedShort, "Healed: {0:N0}" },
                    { Keys.MsgAutoHealStarted, "AUTO HEAL STARTED" },
                    { Keys.MsgAutoHealStopped, "AUTO HEAL STOPPED" },
                    { Keys.MsgHealingTroops, "Healing troops..." },
                    { Keys.MsgCollectingHealed, "Collecting healed troops..." },
                    { Keys.MsgHealedCollected, "Healed troops collected ({0:N0} still waiting)" },
                    { Keys.MsgHospitalClear, "Hospital clear" },
                    { Keys.MsgCollectingCured, "Collecting cured troops (attempt {0})..." },
                    { Keys.MsgWaitingInjured, "Waiting for injured..." },
                    { Keys.MsgNotEnoughResources, "Not enough resources for healing" },
                    { Keys.LogModStarted, "Mod started. Press {0} to toggle panel." },
                    { Keys.LogLanguageChanged, "Language changed to: {0}" },
                    { Keys.LogKeyCancelled, "Key bind cancelled" },
                    { Keys.LogKeySet, "Menu key set to: {0}" },
                    { Keys.LogCuringInProgress, "Curing in progress..." },
                    { Keys.LogCureFinished, "Cure finished - collecting healed troops..." },
                    { Keys.LogStillNotCollected, "Still not collected - open hospital panel once manually" },
                    { Keys.LogNotEnoughResources, "NOT ENOUGH RESOURCES ({0})" },
                    { Keys.LogCureRejected, "Cure rejected repeatedly - slowing down" },
                    { Keys.LogAutoHealError, "AutoHeal error: {0}" },
                    { Keys.LogRefreshFailed, "Refresh failed: {0}" },
                    { Keys.LogHospitalModuleNotReady, "HospitalModule not ready" },
                    { Keys.LogArmyListEmpty, "Army list is empty" },
                    { Keys.LogCureRequestFailed, "Cure request failed: {0}" },
                    { Keys.LogCastFailed, "Failed to cast proto message (IMessage)" },
                    { Keys.LogCityCureSent, "City {0} sent: {1}+{2} type(s), {3:N0} soldiers" },
                    { Keys.LogCityCureFailed, "City {0} send FAILED (network?)" },
                    { Keys.KindCure, "cure" },
                    { Keys.KindInstantCure, "instant cure" },
                    { Keys.LogResourceCheckFailed, "resource check failed: {0}" },
                    { Keys.LogGuildAssistSent, "Guild hospital assist request sent" },
                    { Keys.LogGuildAssistFailed, "Guild hospital assist request failed: {0}" },
                    { Keys.LogStepFailed, "step {0} failed: {1}" },
                    { Keys.LogShortageType, "type {0}/{1} need {2}" },
                    { Keys.LogShortageUnknown, "unknown shortage" },
                    { Keys.LogSpeedUpBagNotReady, "speedup: BagModule not ready" },
                    { Keys.LogSpeedUpCacheNull, "speedup: BagCache null" },
                    { Keys.LogSpeedUpNoneInBag, "speedup: no treatment speed-ups in bag" },
                    { Keys.LogSpeedUpUsed, "SpeedUP used: item {0} x{1}" },
                    { Keys.LogSpeedUpFinished, "speedup: items finished, waiting for timer..." },
                    { Keys.LogSpeedUpFailed, "speedup failed: {0}" },
                    { Keys.LogCollectCacheNull, "collect: CityBuildingCache null" },
                    { Keys.LogCollectButtonClicked, "collect: hospital collect button clicked" },
                    { Keys.LogCollectCastleNull, "collect: ECompCastle null" },
                    { Keys.LogCollectInfoNotFound, "collect: hospital BuildingInfo not found" },
                    { Keys.LogCollectShellAllocFailed, "collect: shell alloc failed" },
                    { Keys.LogCollectShellClicked, "collect: shell hospital collect clicked" },
                    { Keys.LogCollectClickFailed, "collect click failed: {0}" },
                }
            });

            Add(new Language
            {
                Code = "fa",
                Native = "فارسی",
                Flag = "ir",
                Rtl = true,
                Strings = new Dictionary<string, string>
                {
                    { Keys.Title, "Auto Healer FeStiCal" },
                    { Keys.PressAnyKey, "یک کلید را فشار دهید... (Esc = لغو)" },
                    { Keys.Credit, "برنامه‌نویس: {0}" },
                    { Keys.SectionStatus, "وضعیت" },
                    { Keys.SectionControls, "تنظیمات" },
                    { Keys.LabelKey, "کلید:" },
                    { Keys.LabelBatch, "سرباز در هر درمان:" },
                    { Keys.BtnStart, "شروع درمان خودکار" },
                    { Keys.BtnStop, "توقف درمان خودکار" },
                    { Keys.LabelUseSpeedUp, "استفاده از SpeedUP" },
                    { Keys.StatusWaitingGame, "در انتظار بازی..." },
                    { Keys.StatusDisconnectedModule, "قطع - ماژول بیمارستان آماده نیست" },
                    { Keys.StatusDisconnectedCache, "قطع - HospitalCache خالی است" },
                    { Keys.StatusDisconnectedVo, "قطع - HospitalVo خالی است" },
                    { Keys.StatusConnected, "متصل - بیمارستان {0}% پر است" },
                    { Keys.StatusError, "خطا: {0}" },
                    { Keys.StatsLine, "مجروح: {0:N0}  |  در حال درمان: {1:N0}  |  ظرفیت: {2:N0}" },
                    { Keys.InjuredShort, "مجروح: {0:N0}" },
                    { Keys.HealedShort, "درمان‌شده: {0:N0}" },
                    { Keys.MsgAutoHealStarted, "درمان خودکار آغاز شد" },
                    { Keys.MsgAutoHealStopped, "درمان خودکار متوقف شد" },
                    { Keys.MsgHealingTroops, "در حال درمان نیروها..." },
                    { Keys.MsgCollectingHealed, "در حال جمع‌آوری نیروهای درمان‌شده..." },
                    { Keys.MsgHealedCollected, "نیروهای درمان‌شده جمع شدند ({0:N0} هنوز در انتظار)" },
                    { Keys.MsgHospitalClear, "بیمارستان خالی است" },
                    { Keys.MsgCollectingCured, "جمع‌آوری نیروهای درمان‌شده (تلاش {0})..." },
                    { Keys.MsgWaitingInjured, "در انتظار مجروح..." },
                    { Keys.MsgNotEnoughResources, "منابع کافی برای درمان نیست" },
                    { Keys.LogModStarted, "مود اجرا شد. برای نمایش پنل {0} را فشار دهید." },
                    { Keys.LogLanguageChanged, "زبان تغییر کرد به: {0}" },
                    { Keys.LogKeyCancelled, "تغییر کلید لغو شد" },
                    { Keys.LogKeySet, "کلید منو تنظیم شد: {0}" },
                    { Keys.LogCuringInProgress, "درمان در جریان است..." },
                    { Keys.LogCureFinished, "درمان تمام شد - در حال جمع‌آوری نیروها..." },
                    { Keys.LogStillNotCollected, "هنوز جمع‌آوری نشد - یک بار پنل بیمارستان را دستی باز کنید" },
                    { Keys.LogNotEnoughResources, "منابع کافی نیست ({0})" },
                    { Keys.LogCureRejected, "درخواست درمان مکرراً رد شد - کاهش سرعت" },
                    { Keys.LogAutoHealError, "خطای درمان خودکار: {0}" },
                    { Keys.LogRefreshFailed, "به‌روزرسانی داده ناموفق: {0}" },
                    { Keys.LogHospitalModuleNotReady, "ماژول بیمارستان آماده نیست" },
                    { Keys.LogArmyListEmpty, "لیست نیروها خالی است" },
                    { Keys.LogCureRequestFailed, "درخواست درمان ناموفق: {0}" },
                    { Keys.LogCastFailed, "تبدیل پیام پروتوباف ناموفق بود (IMessage)" },
                    { Keys.LogCityCureSent, "درخواست {0} شهر ارسال شد: {1}+{2} نوع، {3:N0} سرباز" },
                    { Keys.LogCityCureFailed, "ارسال {0} شهر ناموفق بود (شبکه?)" },
                    { Keys.KindCure, "درمان" },
                    { Keys.KindInstantCure, "درمان فوری" },
                    { Keys.LogResourceCheckFailed, "بررسی منابع ناموفق: {0}" },
                    { Keys.LogGuildAssistSent, "درخواست کمک اتحاد بیمارستان ارسال شد" },
                    { Keys.LogGuildAssistFailed, "درخواست کمک اتحاد ناموفق: {0}" },
                    { Keys.LogStepFailed, "مرحله {0} ناموفق: {1}" },
                    { Keys.LogShortageType, "نوع {0}/{1} نیاز {2}" },
                    { Keys.LogShortageUnknown, "کمبود نامشخص" },
                    { Keys.LogSpeedUpBagNotReady, "تسریع: ماژول کیف آماده نیست" },
                    { Keys.LogSpeedUpCacheNull, "تسریع: BagCache خالی است" },
                    { Keys.LogSpeedUpNoneInBag, "تسریع: آیتم تسریع درمان در کیف نیست" },
                    { Keys.LogSpeedUpUsed, "تسریع استفاده شد: آیتم {0} در تعداد {1}" },
                    { Keys.LogSpeedUpFinished, "تسریع: آیتم‌ها تمام شد، در انتظار تایمر..." },
                    { Keys.LogSpeedUpFailed, "تسریع ناموفق: {0}" },
                    { Keys.LogCollectCacheNull, "جمع‌آوری: CityBuildingCache خالی است" },
                    { Keys.LogCollectButtonClicked, "جمع‌آوری: دکمه جمع‌آوری بیمارستان کلیک شد" },
                    { Keys.LogCollectCastleNull, "جمع‌آوری: ECompCastle خالی است" },
                    { Keys.LogCollectInfoNotFound, "جمع‌آوری: BuildingInfo بیمارستان پیدا نشد" },
                    { Keys.LogCollectShellAllocFailed, "جمع‌آوری: تخصیص shell ناموفق بود" },
                    { Keys.LogCollectShellClicked, "جمع‌آوری: shell بیمارستان کلیک شد" },
                    { Keys.LogCollectClickFailed, "کلیک جمع‌آوری ناموفق: {0}" },
                }
            });

            Add(new Language
            {
                Code = "ar",
                Native = "العربية",
                Flag = "sa",
                Rtl = true,
                Strings = new Dictionary<string, string>
                {
                    { Keys.Title, "Auto Healer FeStiCal" },
                    { Keys.PressAnyKey, "اضغط أي مفتاح... (Esc = إلغاء)" },
                    { Keys.Credit, "المبرمج: {0}" },
                    { Keys.SectionStatus, "الحالة" },
                    { Keys.SectionControls, "الإعدادات" },
                    { Keys.LabelKey, "المفتاح:" },
                    { Keys.LabelBatch, "جنود لكل علاج:" },
                    { Keys.BtnStart, "بدء العلاج التلقائي" },
                    { Keys.BtnStop, "إيقاف العلاج التلقائي" },
                    { Keys.LabelUseSpeedUp, "استخدام SpeedUP" },
                    { Keys.StatusWaitingGame, "في انتظار اللعبة..." },
                    { Keys.StatusDisconnectedModule, "غير متصل - وحدة المستشفى غير جاهزة" },
                    { Keys.StatusDisconnectedCache, "غير متصل - HospitalCache فارغ" },
                    { Keys.StatusDisconnectedVo, "غير متصل - HospitalVo فارغ" },
                    { Keys.StatusConnected, "متصل - المستشفى ممتلئ {0}%" },
                    { Keys.StatusError, "خطأ: {0}" },
                    { Keys.StatsLine, "الجرحى: {0:N0}  |  قيد العلاج: {1:N0}  |  السعة: {2:N0}" },
                    { Keys.InjuredShort, "الجرحى: {0:N0}" },
                    { Keys.HealedShort, "تم علاجهم: {0:N0}" },
                    { Keys.MsgAutoHealStarted, "بدأ العلاج التلقائي" },
                    { Keys.MsgAutoHealStopped, "تم إيقاف العلاج التلقائي" },
                    { Keys.MsgHealingTroops, "جارٍ علاج القوات..." },
                    { Keys.MsgCollectingHealed, "جارٍ جمع القوات المعالجة..." },
                    { Keys.MsgHealedCollected, "تم جمع القوات المعالجة ({0:N0} في الانتظار)" },
                    { Keys.MsgHospitalClear, "المستشفى فارغ" },
                    { Keys.MsgCollectingCured, "جمع القوات المعالجة (المحاولة {0})..." },
                    { Keys.MsgWaitingInjured, "في انتظار الجرحى..." },
                    { Keys.MsgNotEnoughResources, "لا توجد موارد كافية للعلاج" },
                    { Keys.LogModStarted, "تم تشغيل المود. اضغط {0} لإظهار اللوحة." },
                    { Keys.LogLanguageChanged, "تم تغيير اللغة إلى: {0}" },
                    { Keys.LogKeyCancelled, "تم إلغاء تغيير المفتاح" },
                    { Keys.LogKeySet, "تم تعيين مفتاح القائمة: {0}" },
                    { Keys.LogCuringInProgress, "العلاج قيد التنفيذ..." },
                    { Keys.LogCureFinished, "انتهى العلاج - جارٍ جمع القوات..." },
                    { Keys.LogStillNotCollected, "لم يتم الجمع بعد - افتح لوحة المستشفى يدويًا مرة واحدة" },
                    { Keys.LogNotEnoughResources, "الموارد غير كافية ({0})" },
                    { Keys.LogCureRejected, "تم رفض العلاج بشكل متكرر - إبطاء" },
                    { Keys.LogAutoHealError, "خطأ العلاج التلقائي: {0}" },
                    { Keys.LogRefreshFailed, "فشل تحديث البيانات: {0}" },
                    { Keys.LogHospitalModuleNotReady, "وحدة المستشفى غير جاهزة" },
                    { Keys.LogArmyListEmpty, "قائمة القوات فارغة" },
                    { Keys.LogCureRequestFailed, "فشل طلب العلاج: {0}" },
                    { Keys.LogCastFailed, "فشل تحويل رسالة البروتوباف (IMessage)" },
                    { Keys.LogCityCureSent, "تم إرسال {0} المدينة: {1}+{2} نوع، {3:N0} جندي" },
                    { Keys.LogCityCureFailed, "فشل إرسال {0} المدينة (الشبكة؟)" },
                    { Keys.KindCure, "علاج" },
                    { Keys.KindInstantCure, "علاج فوري" },
                    { Keys.LogResourceCheckFailed, "فشل فحص الموارد: {0}" },
                    { Keys.LogGuildAssistSent, "تم إرسال طلب مساعدة التحالف للمستشفى" },
                    { Keys.LogGuildAssistFailed, "فشل طلب مساعدة التحالف: {0}" },
                    { Keys.LogStepFailed, "فشل الخطوة {0}: {1}" },
                    { Keys.LogShortageType, "النوع {0}/{1} يحتاج {2}" },
                    { Keys.LogShortageUnknown, "نقص غير معروف" },
                    { Keys.LogSpeedUpBagNotReady, "التسريع: وحدة الحقيبة غير جاهزة" },
                    { Keys.LogSpeedUpCacheNull, "التسريع: BagCache فارغ" },
                    { Keys.LogSpeedUpNoneInBag, "التسريع: لا توجد عناصر تسريع العلاج في الحقيبة" },
                    { Keys.LogSpeedUpUsed, "تم استخدام التسريع: العنصر {0} بعدد {1}" },
                    { Keys.LogSpeedUpFinished, "التسريع: انتهت العناصر، في انتظار المؤقت..." },
                    { Keys.LogSpeedUpFailed, "فشل التسريع: {0}" },
                    { Keys.LogCollectCacheNull, "الجمع: CityBuildingCache فارغ" },
                    { Keys.LogCollectButtonClicked, "الجمع: تم النقر على زر جمع المستشفى" },
                    { Keys.LogCollectCastleNull, "الجمع: ECompCastle فارغ" },
                    { Keys.LogCollectInfoNotFound, "الجمع: لم يتم العثور على BuildingInfo للمستشفى" },
                    { Keys.LogCollectShellAllocFailed, "الجمع: فشل تخصيص shell" },
                    { Keys.LogCollectShellClicked, "الجمع: تم النقر على shell المستشفى" },
                    { Keys.LogCollectClickFailed, "فشل النقر على الجمع: {0}" },
                }
            });

            Add(new Language
            {
                Code = "tr",
                Native = "Türkçe",
                Flag = "tr",
                Rtl = false,
                Strings = new Dictionary<string, string>
                {
                    { Keys.Title, "Auto Healer FeStiCal" },
                    { Keys.PressAnyKey, "Bir tuşa basın... (Esc = iptal)" },
                    { Keys.Credit, "Programcı: {0}" },
                    { Keys.SectionStatus, "DURUM" },
                    { Keys.SectionControls, "AYARLAR" },
                    { Keys.LabelKey, "Tuş:" },
                    { Keys.LabelBatch, "Tedavi başına asker:" },
                    { Keys.BtnStart, "Otomatik Tedaviyi Başlat" },
                    { Keys.BtnStop, "Otomatik Tedaviyi Durdur" },
                    { Keys.LabelUseSpeedUp, "SpeedUP kullan" },
                    { Keys.StatusWaitingGame, "OYUN BEKLENİYOR..." },
                    { Keys.StatusDisconnectedModule, "BAĞLANTI YOK - Hastane modülü hazır değil" },
                    { Keys.StatusDisconnectedCache, "BAĞLANTI YOK - HospitalCache boş" },
                    { Keys.StatusDisconnectedVo, "BAĞLANTI YOK - HospitalVo boş" },
                    { Keys.StatusConnected, "BAĞLI - Hastane %{0} dolu" },
                    { Keys.StatusError, "HATA: {0}" },
                    { Keys.StatsLine, "Yaralı: {0:N0}  |  Tedavide: {1:N0}  |  Kapasite: {2:N0}" },
                    { Keys.InjuredShort, "Yaralı: {0:N0}" },
                    { Keys.HealedShort, "Tedavi edilen: {0:N0}" },
                    { Keys.MsgAutoHealStarted, "OTOMATİK TEDAVİ BAŞLADI" },
                    { Keys.MsgAutoHealStopped, "OTOMATİK TEDAVİ DURDURULDU" },
                    { Keys.MsgHealingTroops, "Askerler tedavi ediliyor..." },
                    { Keys.MsgCollectingHealed, "Tedavi edilen askerler toplanıyor..." },
                    { Keys.MsgHealedCollected, "Tedavi edilen askerler toplandı ({0:N0} hâlâ bekliyor)" },
                    { Keys.MsgHospitalClear, "Hastane temiz" },
                    { Keys.MsgCollectingCured, "Tedavi edilen askerler toplanıyor (deneme {0})..." },
                    { Keys.MsgWaitingInjured, "Yaralı bekleniyor..." },
                    { Keys.MsgNotEnoughResources, "Tedavi için yeterli kaynak yok" },
                    { Keys.LogModStarted, "Mod başlatıldı. Paneli açmak için {0} tuşuna basın." },
                    { Keys.LogLanguageChanged, "Dil şu olarak değiştirildi: {0}" },
                    { Keys.LogKeyCancelled, "Tuş atama iptal edildi" },
                    { Keys.LogKeySet, "Menü tuşu ayarlandı: {0}" },
                    { Keys.LogCuringInProgress, "Tedavi sürüyor..." },
                    { Keys.LogCureFinished, "Tedavi bitti - askerler toplanıyor..." },
                    { Keys.LogStillNotCollected, "Hâlâ toplanmadı - hastane panelini bir kez elle açın" },
                    { Keys.LogNotEnoughResources, "YETERLİ KAYNAK YOK ({0})" },
                    { Keys.LogCureRejected, "Tedavi sürekli reddedildi - yavaşlatılıyor" },
                    { Keys.LogAutoHealError, "Otomatik tedavi hatası: {0}" },
                    { Keys.LogRefreshFailed, "Veri yenileme başarısız: {0}" },
                    { Keys.LogHospitalModuleNotReady, "Hastane modülü hazır değil" },
                    { Keys.LogArmyListEmpty, "Asker listesi boş" },
                    { Keys.LogCureRequestFailed, "Tedavi isteği başarısız: {0}" },
                    { Keys.LogCastFailed, "Proto mesajı dönüştürülemedi (IMessage)" },
                    { Keys.LogCityCureSent, "Şehir {0} gönderildi: {1}+{2} tür, {3:N0} asker" },
                    { Keys.LogCityCureFailed, "Şehir {0} gönderimi BAŞARISIZ (ağ?)" },
                    { Keys.KindCure, "tedavi" },
                    { Keys.KindInstantCure, "anında tedavi" },
                    { Keys.LogResourceCheckFailed, "kaynak kontrolü başarısız: {0}" },
                    { Keys.LogGuildAssistSent, "Lonca hastane yardım isteği gönderildi" },
                    { Keys.LogGuildAssistFailed, "Lonca hastane yardım isteği başarısız: {0}" },
                    { Keys.LogStepFailed, "{0} adımı başarısız: {1}" },
                    { Keys.LogShortageType, "tür {0}/{1} gereken {2}" },
                    { Keys.LogShortageUnknown, "bilinmeyen eksiklik" },
                    { Keys.LogSpeedUpBagNotReady, "hızlandırma: Çanta modülü hazır değil" },
                    { Keys.LogSpeedUpCacheNull, "hızlandırma: BagCache boş" },
                    { Keys.LogSpeedUpNoneInBag, "hızlandırma: çantada tedavi hızlandırıcı yok" },
                    { Keys.LogSpeedUpUsed, "Hızlandırma kullanıldı: eşya {0} x{1}" },
                    { Keys.LogSpeedUpFinished, "hızlandırma: eşyalar bitti, zamanlayıcı bekleniyor..." },
                    { Keys.LogSpeedUpFailed, "hızlandırma başarısız: {0}" },
                    { Keys.LogCollectCacheNull, "toplama: CityBuildingCache boş" },
                    { Keys.LogCollectButtonClicked, "toplama: hastane toplama düğmesine tıklandı" },
                    { Keys.LogCollectCastleNull, "toplama: ECompCastle boş" },
                    { Keys.LogCollectInfoNotFound, "toplama: hastane BuildingInfo bulunamadı" },
                    { Keys.LogCollectShellAllocFailed, "toplama: shell ayırma başarısız" },
                    { Keys.LogCollectShellClicked, "toplama: hastane shell tıklandı" },
                    { Keys.LogCollectClickFailed, "toplama tıklaması başarısız: {0}" },
                }
            });

            Add(new Language
            {
                Code = "ru",
                Native = "Русский",
                Flag = "ru",
                Rtl = false,
                Strings = new Dictionary<string, string>
                {
                    { Keys.Title, "Auto Healer FeStiCal" },
                    { Keys.PressAnyKey, "Нажмите любую клавишу... (Esc = отмена)" },
                    { Keys.Credit, "Программист: {0}" },
                    { Keys.SectionStatus, "СОСТОЯНИЕ" },
                    { Keys.SectionControls, "НАСТРОЙКИ" },
                    { Keys.LabelKey, "Клавиша:" },
                    { Keys.LabelBatch, "Солдат за лечение:" },
                    { Keys.BtnStart, "Запустить авто-лечение" },
                    { Keys.BtnStop, "Остановить авто-лечение" },
                    { Keys.LabelUseSpeedUp, "Использовать SpeedUP" },
                    { Keys.StatusWaitingGame, "ОЖИДАНИЕ ИГРЫ..." },
                    { Keys.StatusDisconnectedModule, "НЕТ СВЯЗИ - модуль госпиталя не готов" },
                    { Keys.StatusDisconnectedCache, "НЕТ СВЯЗИ - HospitalCache пуст" },
                    { Keys.StatusDisconnectedVo, "НЕТ СВЯЗИ - HospitalVo пуст" },
                    { Keys.StatusConnected, "ПОДКЛЮЧЕНО - госпиталь заполнен на {0}%" },
                    { Keys.StatusError, "ОШИБКА: {0}" },
                    { Keys.StatsLine, "Раненые: {0:N0}  |  Лечатся: {1:N0}  |  Вместимость: {2:N0}" },
                    { Keys.InjuredShort, "Раненые: {0:N0}" },
                    { Keys.HealedShort, "Вылечено: {0:N0}" },
                    { Keys.MsgAutoHealStarted, "АВТО-ЛЕЧЕНИЕ ЗАПУЩЕНО" },
                    { Keys.MsgAutoHealStopped, "АВТО-ЛЕЧЕНИЕ ОСТАНОВЛЕНО" },
                    { Keys.MsgHealingTroops, "Идёт лечение войск..." },
                    { Keys.MsgCollectingHealed, "Сбор вылеченных войск..." },
                    { Keys.MsgHealedCollected, "Вылеченные войска собраны (ожидает {0:N0})" },
                    { Keys.MsgHospitalClear, "Госпиталь пуст" },
                    { Keys.MsgCollectingCured, "Сбор вылеченных войск (попытка {0})..." },
                    { Keys.MsgWaitingInjured, "Ожидание раненых..." },
                    { Keys.MsgNotEnoughResources, "Недостаточно ресурсов для лечения" },
                    { Keys.LogModStarted, "Мод запущен. Нажмите {0} для панели." },
                    { Keys.LogLanguageChanged, "Язык изменён на: {0}" },
                    { Keys.LogKeyCancelled, "Смена клавиши отменена" },
                    { Keys.LogKeySet, "Клавиша меню установлена: {0}" },
                    { Keys.LogCuringInProgress, "Идёт лечение..." },
                    { Keys.LogCureFinished, "Лечение завершено - сбор войск..." },
                    { Keys.LogStillNotCollected, "Всё ещё не собрано - откройте панель госпиталя вручную один раз" },
                    { Keys.LogNotEnoughResources, "НЕДОСТАТОЧНО РЕСУРСОВ ({0})" },
                    { Keys.LogCureRejected, "Лечение постоянно отклоняется - замедление" },
                    { Keys.LogAutoHealError, "Ошибка авто-лечения: {0}" },
                    { Keys.LogRefreshFailed, "Ошибка обновления данных: {0}" },
                    { Keys.LogHospitalModuleNotReady, "Модуль госпиталя не готов" },
                    { Keys.LogArmyListEmpty, "Список войск пуст" },
                    { Keys.LogCureRequestFailed, "Запрос лечения не удался: {0}" },
                    { Keys.LogCastFailed, "Не удалось преобразовать proto-сообщение (IMessage)" },
                    { Keys.LogCityCureSent, "Город {0} отправлен: {1}+{2} тип(ов), {3:N0} солдат" },
                    { Keys.LogCityCureFailed, "Город {0}: отправка НЕ УДАЛАСЬ (сеть?)" },
                    { Keys.KindCure, "лечение" },
                    { Keys.KindInstantCure, "мгновенное лечение" },
                    { Keys.LogResourceCheckFailed, "проверка ресурсов не удалась: {0}" },
                    { Keys.LogGuildAssistSent, "Запрос помощи гильдии для госпиталя отправлен" },
                    { Keys.LogGuildAssistFailed, "Запрос помощи гильдии не удался: {0}" },
                    { Keys.LogStepFailed, "шаг {0} не удался: {1}" },
                    { Keys.LogShortageType, "тип {0}/{1} нужно {2}" },
                    { Keys.LogShortageUnknown, "неизвестная нехватка" },
                    { Keys.LogSpeedUpBagNotReady, "ускорение: модуль сумки не готов" },
                    { Keys.LogSpeedUpCacheNull, "ускорение: BagCache пуст" },
                    { Keys.LogSpeedUpNoneInBag, "ускорение: в сумке нет ускорителей лечения" },
                    { Keys.LogSpeedUpUsed, "Ускорение использовано: предмет {0} x{1}" },
                    { Keys.LogSpeedUpFinished, "ускорение: предметы закончились, ждём таймер..." },
                    { Keys.LogSpeedUpFailed, "ускорение не удалось: {0}" },
                    { Keys.LogCollectCacheNull, "сбор: CityBuildingCache пуст" },
                    { Keys.LogCollectButtonClicked, "сбор: нажата кнопка сбора госпиталя" },
                    { Keys.LogCollectCastleNull, "сбор: ECompCastle пуст" },
                    { Keys.LogCollectInfoNotFound, "сбор: BuildingInfo госпиталя не найден" },
                    { Keys.LogCollectShellAllocFailed, "сбор: не удалось выделить shell" },
                    { Keys.LogCollectShellClicked, "сбор: shell госпиталя нажат" },
                    { Keys.LogCollectClickFailed, "клик сбора не удался: {0}" },
                }
            });
        }

        private static void Add(Language lang)
        {
            if (lang == null || string.IsNullOrEmpty(lang.Code)) return;
            Languages.Add(lang);
        }

        // ---------- وضعیت فعلی ----------
        public static int Count => Languages.Count;

        public static int Index => current;

        public static Language Current => current >= 0 && current < Languages.Count ? Languages[current] : Languages[0];

        public static string CurrentName => Current.Native;

        public static bool Rtl => Current.Rtl;

        public static string NameAt(int index)
        {
            return index >= 0 && index < Languages.Count ? Languages[index].Native : "";
        }

        public static string FlagAt(int index)
        {
            return index >= 0 && index < Languages.Count ? Languages[index].Flag : "";
        }

        public static string CurrentFlag => Current.Flag;

        public static void SetIndex(int index)
        {
            if (index < 0 || index >= Languages.Count || index == current) return;
            current = index;
            LanguageChanged?.Invoke();
        }

        // برای اینکه بقیه کدها از تغییر زبان باخبر شوند.
        public static event System.Action? LanguageChanged;

        // ---------- ترجمه ----------
        /// <summary>ترجمه یک کلید؛ اگر نبود به English و در نهایت به خود کلید برمی‌گردد.</summary>
        public static string T(string key)
        {
            if (key == null) return "";
            string? value;
            if (Current.Strings.TryGetValue(key, out value) && !string.IsNullOrEmpty(value)) return value;
            if (Languages.Count > 0 &&
                Languages[0].Strings.TryGetValue(key, out value) && !string.IsNullOrEmpty(value)) return value;
            return key;
        }

        /// <summary>ترجمه یک کلید با پارامترهای قالب‌بندی (string.Format).</summary>
        public static string T(string key, params object[]? args)
        {
            string format = T(key);
            if (args == null || args.Length == 0) return format;
            try { return string.Format(format, args); }
            catch { return format; }
        }
    }
}
