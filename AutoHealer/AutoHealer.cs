using Il2CppGoogle.Protobuf;
using Il2CppIGG.Game.Data.Cache;
using Il2CppIGG.Game.Data.Cache.Hospital;
using Il2CppIGG.Game.Data.Cache.CityBuilding;
using Il2CppIGG.Game.Data.Cache.Bag;
using Il2CppIGG.Game.Data.Config;
using Il2CppIGG.Game.Module.CityBuilding;
using Il2CppIGG.Game.Module.Hospital;
using Il2CppIGG.Game.Module.Bag;
using Il2CppIGG.Game.Module.Guild;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using Il2CppProtomsg;
using MelonLoader;
using UnityEngine;

namespace AutoHealClick
{
    public class AutoHealerFeStiCal : MelonMod
    {
        private const string ModVersion = "1.0.0";
        private const string Author = "@AlirezaFeStiCal";

        // ---------- تلگرام سازنده ----------
        // برای تغییر آیدی فقط همین دو مقدار را عوض کنید.
        private const string TelegramHandle = "@AlirezaFeStiCal";
        private const string TelegramUrl = "https://t.me/AlirezaFeStiCal";

        // ========================================================
        //  چیدمان پنل (همه مختصات داخلی گروه هستند)
        // ========================================================
        private const int PanelW = 440;
        private const int PanelX = 10;
        private const int PanelY = 10;
        private const int HeaderH = 30;
        private const int GroupX = 16;
        private const int GroupY = 48;
        private const int InnerW = PanelW - 12;   // عرض گروه

        // ارتفاع محتوای پنل: وقتی درمان خودکار روشن است یک کارت اضافه می‌شود
        private const int ContentIdle = 342;
        private const int ContentActive = 446;

        private const int KeyRowY = 0;
        private const int KeyRowH = 28;
        private const int StatusLabelY = 40;
        private const int StatusCardY = 60;
        private const int StatusCardH = 96;
        private const int ControlsLabelY = 172;
        private const int ControlsCardY = 192;
        private const int ControlsCardH = 142;
        private const int AutoCardY = 350;
        private const int AutoCardH = 88;

        // لیست زبان فقط پرچم است؛ در هر ردیف حداکثر ۵ پرچم
        private const int LangItemW = 46;
        private const int LangItemH = 36;
        private const int LangGap = 6;
        private const int LangPad = 8;
        private const int LangPerRow = 5;

        // ========================================================
        //  رنگ‌ها
        // ========================================================
        private static readonly Color ColPanel = new Color(0.063f, 0.068f, 0.082f, 0.98f);
        private static readonly Color ColHeader = new Color(0.125f, 0.135f, 0.165f, 1f);
        private static readonly Color ColCard = new Color(0.105f, 0.115f, 0.140f, 1f);
        private static readonly Color ColPopup = new Color(0.095f, 0.103f, 0.126f, 1f);
        private static readonly Color ColAccent = new Color(0.165f, 0.72f, 0.35f, 1f);
        private static readonly Color ColAccentDim = new Color(0.165f, 0.72f, 0.35f, 0.30f);
        private static readonly Color ColBorder = new Color(1f, 1f, 1f, 0.075f);
        private static readonly Color ColText = new Color(0.90f, 0.92f, 0.95f, 1f);
        private static readonly Color ColTextDim = new Color(0.50f, 0.55f, 0.63f, 1f);
        private static readonly Color ColDanger = new Color(0.80f, 0.25f, 0.27f, 1f);
        private static readonly Color ColWarn = new Color(0.94f, 0.71f, 0.20f, 1f);
        private static readonly Color ColField = new Color(0.16f, 0.175f, 0.215f, 1f);
        private static readonly Color ColNeutralBtn = new Color(0.20f, 0.22f, 0.27f, 1f);
        private static readonly Color ColHover = new Color(1f, 1f, 1f, 0.07f);
        private static readonly Color ColTrack = new Color(0f, 0f, 0f, 0.38f);

        // ---------- وضعیت UI ----------
        private bool visible = true;
        private KeyCode toggleKey = KeyCode.F8;
        private string keyText = "F8";
        private bool waitingForKey = false;
        private bool languageOpen = false;

        // ---------- تنظیمات ----------
        private bool autoHealEnabled = false;
        private bool useSpeedUp = false;
        private bool speedUpWarned = false;
        private string uiBatchCount = "100";

        // ---------- حلقه وضعیت-محور ----------
        private const float PollInterval = 0.1f;
        private float pollTimer = 0f;
        private float retryCooldown = 0f;
        private int collectAttempts = 0;
        private int cureSendStreak = 0;
        private bool resourceWarned = false;
        private bool allianceHelpRequested = false;

        // ---------- داده بیمارستان ----------
        private HospitalCacheVo lastVo = null;
        private System.Collections.Generic.List<ArmyEntry> entries = new System.Collections.Generic.List<ArmyEntry>();
        private bool dataLoaded = false;
        private ulong totalInjured = 0;
        private ulong totalCuring = 0;
        private uint hospitalCapacity = 0;
        private int hospitalPercent = 0;
        private bool moduleConnected = false;

        // ---------- ردیابی تغییر وضعیت برای کنسول ----------
        private HospitalCacheVo.CureState prevCureState = HospitalCacheVo.CureState.None;
        private ulong prevCuringAmount = 0;

        // ---------- متن‌ها به صورت کلید نگه داشته می‌شوند تا با تغییر زبان فوراً ترجمه شوند ----------
        private string statusLineKey = Keys.StatusWaitingGame;
        private object[]? statusLineArgs = null;

        private string statusKey = "";
        private object[]? statusArgs = null;
        private Color statusColor = ColAccent;
        private ulong totalHealedTroops = 0; // مجموع نیروهای درمان‌شده

        // ---------- لاگ (برای MelonConsole) ----------
        private System.Collections.Generic.List<string> log = new System.Collections.Generic.List<string>();
        private const int MaxLogLines = 60;

        private class ArmyEntry
        {
            public uint ArmyId;
            public string Name = "";
            public uint Rank;
            public ulong Count;
            public bool IsSpecial;
            public bool Selected;
        }

        // ========================================================
        //  چرخه زندگی MelonLoader
        // ========================================================
        public override void OnApplicationStart()
        {
            Loc.LanguageChanged += OnLanguageChanged;
            MelonLogger.Msg("[AutoHealer] " + Loc.T(Keys.LogModStarted, keyText));
        }

        private void OnLanguageChanged()
        {
            MelonLogger.Msg("[AutoHealer] " + Loc.T(Keys.LogLanguageChanged, Loc.CurrentName));
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(toggleKey))
                visible = !visible;

            if (retryCooldown > 0f)
                retryCooldown -= Time.deltaTime;

            pollTimer += Time.deltaTime;
            if (pollTimer < PollInterval)
                return;
            pollTimer = 0f;

            RefreshData();
            LogCureStateTransitions();

            if (autoHealEnabled)
                AutoHealTick();
        }

        // ========================================================
        //  UI
        // ========================================================
        private const int FooterH = 32;   // ارتفاع ناحیه پانویس

        // ۳۸ پیکسل بالای گروه + پانویس + ۸ پیکسل فاصله پایین
        private int PanelH => (autoHealEnabled ? ContentActive : ContentIdle) + 38 + FooterH + 8;

        public override void OnGUI()
        {
            if (!visible) return;

            Event cur = Event.current;

            if (waitingForKey)
            {
                if (cur != null && cur.type == EventType.KeyDown && cur.keyCode != KeyCode.None)
                {
                    if (cur.keyCode == KeyCode.Escape)
                    {
                        Log(Loc.T(Keys.LogKeyCancelled));
                    }
                    else
                    {
                        toggleKey = cur.keyCode;
                        keyText = cur.keyCode.ToString();
                        Log(Loc.T(Keys.LogKeySet, keyText));
                    }
                    waitingForKey = false;
                    cur.Use();
                }
            }
            else if (languageOpen && cur != null && cur.type == EventType.KeyDown && cur.keyCode == KeyCode.Escape)
            {
                languageOpen = false;
                cur.Use();
            }

            // کلیک بیرون از لیست زبان، لیست را می‌بندد
            if (languageOpen && cur != null && cur.type == EventType.MouseDown &&
                !LanguagePopupRect().Contains(cur.mousePosition) &&
                !LanguageButtonRect().Contains(cur.mousePosition))
            {
                languageOpen = false;
            }

            // حاشیه بیرونی پنل
            Rect panel = new Rect(PanelX, PanelY, PanelW, PanelH);
            Fill(panel, ColPanel);
            Color edge = new Color(1f, 1f, 1f, 0.10f);
            Fill(new Rect(panel.x, panel.y, panel.width, 1), edge);
            Fill(new Rect(panel.x, panel.yMax - 1, panel.width, 1), edge);
            Fill(new Rect(panel.x, panel.y, 1, panel.height), edge);
            Fill(new Rect(panel.xMax - 1, panel.y, 1, panel.height), edge);

            DrawHeader();

            GUI.BeginGroup(new Rect(GroupX, GroupY, InnerW, PanelH - 38 - FooterH));

            DrawKeyRow();
            DrawStatusSection();
            DrawControlsSection();

            if (autoHealEnabled)
                DrawAutoSection();

            GUI.EndGroup();

            DrawFooter();

            // لیست زبان آخر از همه رسم می‌شود تا روی بقیه بیفتد
            DrawLanguagePopup();
        }

        // ========================================================
        //  ابزارهای رسم
        // ========================================================
        private static void Fill(Rect rect, Color color)
        {
            Color prev = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = prev;
        }

        /// <summary>کارت با حاشیه نازک و نوار رنگی کنار.</summary>
        private static void Card(Rect rect, Color accent)
        {
            Fill(rect, ColCard);
            Fill(new Rect(rect.x, rect.y, rect.width, 1), ColBorder);
            Fill(new Rect(rect.x, rect.yMax - 1, rect.width, 1), ColBorder);
            Fill(new Rect(rect.x, rect.y, 1, rect.height), ColBorder);
            Fill(new Rect(rect.xMax - 1, rect.y, 1, rect.height), ColBorder);

            float accentX = Loc.Rtl ? rect.xMax - 4 : rect.x + 1;
            Fill(new Rect(accentX, rect.y + 1, 3, rect.height - 2), accent);
        }

        /// <summary>یک مستطیل محلی را در حالت راست‌به‌چپ افقی آینه می‌کند.</summary>
        private static Rect MirrorX(Rect local, float containerWidth)
        {
            if (!Loc.Rtl) return local;
            return new Rect(containerWidth - local.xMax, local.y, local.width, local.height);
        }

        /// <summary>مستطیل داخل یک کارت/ردیف را (با آینه‌کردن در RTL) به مختصات پنل تبدیل می‌کند.</summary>
        private static Rect InCard(Rect container, Rect local)
        {
            Rect r = MirrorX(local, container.width);
            return new Rect(container.x + r.x, container.y + r.y, r.width, r.height);
        }

        // -------- لیبل‌ها --------
        //  متن هر لیبل داخل محدوده خودش بریده می‌شود تا روی کنترل‌های دیگر نیفتد؛
        //  برای همین چینش عمودی همهجا وسط است تا از بالا و پایین بریده نشود.
        private static void DrawLabel(Rect rect, string text, TextAnchor anchor)
        {
            TextAnchor prevAnchor = GUI.skin.label.alignment;
            TextClipping prevClip = GUI.skin.label.clipping;
            GUI.skin.label.alignment = anchor;
            GUI.skin.label.clipping = TextClipping.Clip;
            GUI.Label(rect, text);
            GUI.skin.label.clipping = prevClip;
            GUI.skin.label.alignment = prevAnchor;
        }

        private static void LabelLeft(Rect rect, string text)
        {
            DrawLabel(rect, text, TextAnchor.MiddleLeft);
        }

        private static void LabelCentered(Rect rect, string text)
        {
            DrawLabel(rect, text, TextAnchor.MiddleCenter);
        }

        /// <summary>لیبل با چینش درست برای زبان‌های راست‌به‌چپ.</summary>
        private static void Label(Rect rect, string text)
        {
            DrawLabel(rect, text, Loc.Rtl ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft);
        }

        /// <summary>متن را برای رسم آماده می‌کند (در زبان‌های راست‌به‌چپ: شکل‌دهی و ترتیب بصری).</summary>
        private static string Disp(string text)
        {
            return Loc.Rtl ? RtlText.Visual(text) : text;
        }

        // ========================================================
        //  سربرگ + دکمه کوچک زبان در گوشه
        // ========================================================
        private static Rect LanguageButtonRect()
        {
            return new Rect(PanelX + PanelW - 10 - 42, PanelY + 5, 42, 22);
        }

        private static Rect TelegramButtonRect()
        {
            return new Rect(LanguageButtonRect().x - 6 - 42, PanelY + 5, 42, 22);
        }

        private static Rect LanguagePopupRect()
        {
            int cols = Mathf.Min(Loc.Count, LangPerRow);
            int rows = (Loc.Count + LangPerRow - 1) / LangPerRow;
            float w = LangPad * 2 + cols * LangItemW + Mathf.Max(0, cols - 1) * LangGap;
            float h = LangPad * 2 + rows * LangItemH + Mathf.Max(0, rows - 1) * LangGap;
            return new Rect(PanelX + PanelW - 10 - w, PanelY + HeaderH + 8, w, h);
        }

        private void DrawHeader()
        {
            Fill(new Rect(PanelX, PanelY, PanelW, HeaderH), ColHeader);
            Fill(new Rect(PanelX, PanelY + HeaderH - 2, PanelW, 2), ColAccent);

            GUI.color = ColText;
            string heading = waitingForKey
                ? Disp(Loc.T(Keys.PressAnyKey))
                : Disp(Loc.T(Keys.Title)) + "   v" + ModVersion;
            int titleW = Mathf.RoundToInt(TelegramButtonRect().x - (PanelX + 12) - 8);
            LabelLeft(new Rect(PanelX + 12, PanelY + 3, titleW, HeaderH - 6), heading);
            GUI.color = Color.white;

            DrawTelegramButton();

            Rect btn = LanguageButtonRect();
            bool hover = btn.Contains(Event.current.mousePosition);
            Fill(btn, languageOpen ? ColAccentDim : (hover ? ColHover : new Color(1f, 1f, 1f, 0.04f)));
            Fill(new Rect(btn.x, btn.y, btn.width, 1), ColBorder);
            Fill(new Rect(btn.x, btn.yMax - 1, btn.width, 1), ColBorder);
            Fill(new Rect(btn.x, btn.y, 1, btn.height), ColBorder);
            Fill(new Rect(btn.xMax - 1, btn.y, 1, btn.height), ColBorder);

            if (GUI.Button(btn, "", GUIStyle.none))
            {
                languageOpen = !languageOpen;
                waitingForKey = false;
            }

            Flags.Draw(new Rect(btn.x + 6, btn.y + 5, btn.width - 12, btn.height - 10), Loc.CurrentFlag);
        }

        private static void DrawTelegramButton()
        {
            Rect btn = TelegramButtonRect();
            bool hover = btn.Contains(Event.current.mousePosition);

            Color telegram = new Color(0.13f, 0.62f, 0.85f, 1f);
            Fill(btn, hover ? new Color(0.20f, 0.70f, 0.93f, 1f) : telegram);
            Fill(new Rect(btn.x, btn.y, btn.width, 1), ColBorder);
            Fill(new Rect(btn.x, btn.yMax - 1, btn.width, 1), ColBorder);
            Fill(new Rect(btn.x, btn.y, 1, btn.height), ColBorder);
            Fill(new Rect(btn.xMax - 1, btn.y, 1, btn.height), ColBorder);

            GUI.color = Color.white;
            LabelCentered(btn, "TG");
            GUI.color = Color.white;

            if (GUI.Button(btn, "", GUIStyle.none))
                OpenTelegram();
        }

        private static void OpenTelegram()
        {
            try { Application.OpenURL(TelegramUrl); }
            catch (System.Exception ex) { MelonLogger.Msg("[AutoHealer] " + TelegramHandle + " -> " + ex.Message); }
        }

        private void DrawFooter()
        {
            float separatorY = PanelY + PanelH - FooterH;
            Fill(new Rect(GroupX, separatorY, InnerW, 1), ColBorder);

            GUI.color = ColTextDim;
            Label(new Rect(GroupX, separatorY + 4, InnerW, FooterH - 8), Disp(Loc.T(Keys.Credit, Author)));
            GUI.color = Color.white;
        }

        private void DrawLanguagePopup()
        {
            if (!languageOpen) return;

            Rect box = LanguagePopupRect();
            Fill(box, ColPopup);
            Fill(new Rect(box.x, box.y, box.width, 1), new Color(1f, 1f, 1f, 0.14f));
            Fill(new Rect(box.x, box.yMax - 1, box.width, 1), new Color(1f, 1f, 1f, 0.14f));
            Fill(new Rect(box.x, box.y, 1, box.height), new Color(1f, 1f, 1f, 0.14f));
            Fill(new Rect(box.xMax - 1, box.y, 1, box.height), new Color(1f, 1f, 1f, 0.14f));

            float contentW = box.width - LangPad * 2;
            for (int i = 0; i < Loc.Count; i++)
            {
                int col = i % LangPerRow;
                int row = i / LangPerRow;

                float itemX = box.x + LangPad + col * (LangItemW + LangGap);
                if (Loc.Rtl)
                    itemX = box.x + LangPad + contentW - col * (LangItemW + LangGap) - LangItemW;

                Rect item = new Rect(itemX, box.y + LangPad + row * (LangItemH + LangGap), LangItemW, LangItemH);
                bool selected = i == Loc.Index;
                bool hover = item.Contains(Event.current.mousePosition);

                if (selected || hover)
                {
                    Fill(item, selected ? ColAccentDim : ColHover);
                    Fill(new Rect(item.x, item.y, item.width, 1), ColBorder);
                    Fill(new Rect(item.x, item.yMax - 1, item.width, 1), ColBorder);
                    Fill(new Rect(item.x, item.y, 1, item.height), ColBorder);
                    Fill(new Rect(item.xMax - 1, item.y, 1, item.height), ColBorder);
                }

                Flags.Draw(new Rect(item.x + 5, item.y + 6, LangItemW - 10, LangItemH - 12), Loc.FlagAt(i));

                if (GUI.Button(item, "", GUIStyle.none))
                {
                    Loc.SetIndex(i);
                    languageOpen = false;
                }
            }
        }

        // ========================================================
        //  ردیف بایند کلید (بالای پنل)
        // ========================================================
        private void DrawKeyRow()
        {
            GUI.color = ColTextDim;
            Label(MirrorX(new Rect(2, KeyRowY + 4, 72, 24), InnerW), Disp(Loc.T(Keys.LabelKey)));
            GUI.color = Color.white;

            Color prev = GUI.backgroundColor;
            GUI.backgroundColor = waitingForKey ? ColWarn : ColNeutralBtn;
            string keyLabel = waitingForKey ? "..." : keyText;
            if (GUI.Button(MirrorX(new Rect(80, KeyRowY, InnerW - 80, KeyRowH), InnerW), keyLabel))
            {
                waitingForKey = !waitingForKey;
                languageOpen = false;
            }
            GUI.backgroundColor = prev;
        }

        // ========================================================
        //  کارت وضعیت
        // ========================================================
        private void DrawStatusSection()
        {
            GUI.color = ColTextDim;
            Label(new Rect(6, StatusLabelY, InnerW - 12, 20), Disp(Loc.T(Keys.SectionStatus)));
            GUI.color = Color.white;

            Rect card = new Rect(0, StatusCardY, InnerW, StatusCardH);
            Card(card, moduleConnected ? ColAccent : ColWarn);

            Fill(InCard(card, new Rect(18, 23, 10, 10)), moduleConnected ? ColAccent : ColWarn);

            GUI.color = moduleConnected ? new Color(0.62f, 0.95f, 0.72f) : ColWarn;
            Label(InCard(card, new Rect(42, 16, card.width - 62, 28)), Disp(Loc.T(statusLineKey, statusLineArgs)));
            GUI.color = Color.white;

            if (dataLoaded)
            {
                GUI.color = ColTextDim;
                Label(InCard(card, new Rect(18, 50, card.width - 36, 24)),
                    Disp(Loc.T(Keys.StatsLine, totalInjured, totalCuring, hospitalCapacity)));
                GUI.color = Color.white;

                DrawCapacityBar(InCard(card, new Rect(18, 76, card.width - 36, 14)));
            }
        }

        private void DrawCapacityBar(Rect rect)
        {
            Fill(rect, ColTrack);

            float ratio = Mathf.Clamp01(hospitalPercent / 100f);
            float width = (rect.width - 2) * ratio;
            Color fillColor = ratio >= 0.9f ? ColDanger : (ratio >= 0.7f ? ColWarn : ColAccent);

            if (width > 0f)
            {
                float x = Loc.Rtl ? rect.xMax - 1 - width : rect.x + 1;
                Fill(new Rect(x, rect.y + 1, width, rect.height - 2), fillColor);
            }

            GUI.color = ColText;
            LabelCentered(new Rect(rect.x, rect.y - 5, rect.width, rect.height + 10), hospitalPercent + "%");
            GUI.color = Color.white;
        }

        // ========================================================
        //  کارت کنترل‌ها
        // ========================================================
        private void DrawControlsSection()
        {
            GUI.color = ColTextDim;
            Label(new Rect(6, ControlsLabelY, InnerW - 12, 20), Disp(Loc.T(Keys.SectionControls)));
            GUI.color = Color.white;

            Rect card = new Rect(0, ControlsCardY, InnerW, ControlsCardH);
            Card(card, ColAccent);

            Label(InCard(card, new Rect(20, 18, 216, 24)), Disp(Loc.T(Keys.LabelBatch)));

            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = ColField;
            uiBatchCount = GUI.TextField(InCard(card, new Rect(card.width - 148, 16, 128, 26)), uiBatchCount);
            GUI.backgroundColor = prevBg;

            GUI.backgroundColor = autoHealEnabled ? ColDanger : ColAccent;
            string label = autoHealEnabled ? Disp(Loc.T(Keys.BtnStop)) : Disp(Loc.T(Keys.BtnStart));
            if (GUI.Button(new Rect(card.x + 20, card.y + 56, card.width - 40, 36), label))
            {
                autoHealEnabled = !autoHealEnabled;
                collectAttempts = 0;
                speedUpWarned = false;
                if (autoHealEnabled)
                {
                    ClearStatus();
                    Log(Loc.T(Keys.MsgAutoHealStarted));
                }
                else
                {
                    ClearStatus();
                    Log(Loc.T(Keys.MsgAutoHealStopped));
                }
            }
            GUI.backgroundColor = prevBg;

            useSpeedUp = CheckRow(new Rect(card.x + 16, card.y + 104, card.width - 32, 30),
                useSpeedUp, Disp(Loc.T(Keys.LabelUseSpeedUp)));
        }

        /// <summary>چک‌باکس دستی تا ظاهر تمیزتری داشته باشد.</summary>
        private bool CheckRow(Rect row, bool value, string label)
        {
            Fill(row, row.Contains(Event.current.mousePosition) ? ColHover : new Color(1f, 1f, 1f, 0.025f));

            Rect box = InCard(row, new Rect(4, (row.height - 20) / 2f, 20, 20));
            Fill(box, value ? ColAccent : new Color(1f, 1f, 1f, 0.08f));
            Fill(new Rect(box.x, box.y, box.width, 1), ColBorder);
            Fill(new Rect(box.x, box.yMax - 1, box.width, 1), ColBorder);
            Fill(new Rect(box.x, box.y, 1, box.height), ColBorder);
            Fill(new Rect(box.xMax - 1, box.y, 1, box.height), ColBorder);
            if (value) Fill(new Rect(box.x + 5, box.y + 5, 10, 10), new Color(1f, 1f, 1f, 0.92f));

            GUI.color = value ? ColText : ColTextDim;
            Label(InCard(row, new Rect(36, (row.height - 24) / 2f, row.width - 56, 24)), label);
            GUI.color = Color.white;

            return GUI.Button(row, "", GUIStyle.none) ? !value : value;
        }

        // ========================================================
        //  کارت وضعیت درمان خودکار
        // ========================================================
        private void DrawAutoSection()
        {
            Rect card = new Rect(0, AutoCardY, InnerW, AutoCardH);
            Card(card, ColAccent);

            int spinnerIndex = (int)(Time.time * 5) % 4;
            char spinnerChar = "|/-\\"[spinnerIndex];

            GUI.color = statusColor;
            Label(InCard(card, new Rect(20, 14, card.width - 40, 28)),
                Disp(Loc.T(statusKey, statusArgs) + " " + spinnerChar));
            GUI.color = ColText;

            Label(InCard(card, new Rect(20, 52, 180, 24)), Disp(Loc.T(Keys.InjuredShort, totalInjured)));
            Label(InCard(card, new Rect(card.width - 200, 52, 180, 24)), Disp(Loc.T(Keys.HealedShort, totalHealedTroops)));
            GUI.color = Color.white;
        }

        // -------- پیام وضعیت --------
        private void SetStatus(string key, Color color, params object[]? args)
        {
            statusKey = key;
            statusArgs = args;
            statusColor = color;
        }

        private void ClearStatus()
        {
            statusKey = "";
            statusArgs = null;
            statusColor = ColAccent;
        }

        // ========================================================
        //  داده‌ها (بیمارستان شهر - همان منبعی که پنل خود بازی استفاده می‌کند)
        // ========================================================
        private void RefreshData()
        {
            try
            {
                if (!HospitalModule.IsExist)
                {
                    statusLineKey = Keys.StatusDisconnectedModule;
                    statusLineArgs = null;
                    moduleConnected = false;
                    return;
                }

                HospitalCache cache = AppCache.Hospital;
                if (cache == null)
                {
                    statusLineKey = Keys.StatusDisconnectedCache;
                    statusLineArgs = null;
                    moduleConnected = false;
                    return;
                }

                HospitalCacheVo vo = cache.WorldHospitalVo;
                if (vo == null)
                    vo = cache.GetCurHospitalVo();
                if (vo == null)
                {
                    statusLineKey = Keys.StatusDisconnectedVo;
                    statusLineArgs = null;
                    moduleConnected = false;
                    return;
                }

                moduleConnected = true;
                dataLoaded = true;
                lastVo = vo;

                entries.Clear();
                totalInjured = 0;
                totalCuring = 0;

                AddArmyList(vo.InjuredList, false);
                AddArmyList(vo.SpecialInjuredList, true);
                totalInjured = vo.GetTotalInjuredAmount();
                totalCuring = vo.GetTotalCuringArmyAmount();

                try { hospitalCapacity = HospitalModule.Inst.GetHospitalCapacity(); } catch { }

                try { hospitalPercent = HospitalModule.Inst.GetCurHospitalPercent(); } catch { }
                statusLineKey = Keys.StatusConnected;
                statusLineArgs = new object[] { hospitalPercent };
            }
            catch (System.Exception ex)
            {
                moduleConnected = false;
                statusLineKey = Keys.StatusError;
                statusLineArgs = new object[] { ex.Message };
                Log(Loc.T(Keys.LogRefreshFailed, ex.Message));
            }
        }

        private void AddArmyList(Il2CppSystem.Collections.Generic.List<ArmyInfo> list, bool isSpecial)
        {
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                ArmyInfo a = list[i];
                if (a == null) continue;
                ArmyEntry e = entries.Find(x => x.ArmyId == a.ArmyId && x.IsSpecial == isSpecial);
                if (e == null)
                {
                    e = new ArmyEntry();
                    e.ArmyId = a.ArmyId;
                    e.Name = GetTroopName(a.ArmyId);
                    e.Rank = GetTroopRank(a.ArmyId);
                    e.Count = a.Count;
                    e.IsSpecial = isSpecial;
                    e.Selected = true;
                    entries.Add(e);
                }
                else
                {
                    e.Count += a.Count;
                }
            }
        }

        private string GetTroopName(uint armyId)
        {
            try
            {
                var dao = TroopDao.Inst;
                if (dao != null)
                {
                    string n = dao.GetTroopName(armyId);
                    if (!string.IsNullOrEmpty(n)) return n;
                }
            }
            catch { }
            return "Troop " + armyId;
        }

        private uint GetTroopRank(uint armyId)
        {
            try
            {
                var dao = TroopDao.Inst;
                if (dao != null) return dao.GetTroopRank(armyId);
            }
            catch { }
            return 0;
        }

        // ========================================================
        //  حلقه خودکارِ وضعیت-محور (بدون تایمر ثانیه‌ای)
        // ========================================================
        private void LogCureStateTransitions()
        {
            if (lastVo == null || !dataLoaded) return;
            var st = lastVo.CurrentCureState;
            ulong currentCuring = totalCuring;

            if (st == prevCureState)
            {
                prevCuringAmount = currentCuring;
                return;
            }

            switch (st)
            {
                case HospitalCacheVo.CureState.Curing:
                    SetStatus(Keys.MsgHealingTroops, Color.green);
                    Log(Loc.T(Keys.LogCuringInProgress));
                    cureSendStreak = 0;
                    resourceWarned = false;
                    allianceHelpRequested = false;
                    break;
                case HospitalCacheVo.CureState.CureDone:
                    SetStatus(Keys.MsgCollectingHealed, Color.yellow);
                    Log(Loc.T(Keys.LogCureFinished));
                    break;
                case HospitalCacheVo.CureState.Idle:
                    allianceHelpRequested = false;
                    if (prevCureState == HospitalCacheVo.CureState.CureDone ||
                        prevCureState == HospitalCacheVo.CureState.Curing)
                    {
                        totalHealedTroops += prevCuringAmount;
                        if (totalInjured > 0)
                        {
                            SetStatus(Keys.MsgHealedCollected, Color.green, totalInjured);
                        }
                        else
                        {
                            SetStatus(Keys.MsgHospitalClear, Color.green);
                        }
                        Log(Loc.T(Keys.MsgHealedCollected, totalInjured));
                    }
                    break;
            }
            prevCureState = st;
            prevCuringAmount = currentCuring;
        }

        private void AutoHealTick()
        {
            try
            {
                if (!moduleConnected || lastVo == null) return;

                var state = lastVo.CurrentCureState;

                if (state == HospitalCacheVo.CureState.Curing)
                {
                    collectAttempts = 0;
                    try { HospitalModule.Inst.UpdateCureState(); } catch { }
                    RequestAllianceHelpOnce();

                    if (useSpeedUp && retryCooldown <= 0f)
                    {
                        UseSpeedUpItems();
                        retryCooldown = 2.5f;
                    }
                    return;
                }

                if (state == HospitalCacheVo.CureState.CureDone)
                {
                    if (retryCooldown > 0f) return;

                    collectAttempts++;
                    SetStatus(Keys.MsgCollectingCured, Color.yellow, collectAttempts);
                    Log(Loc.T(Keys.MsgCollectingCured, collectAttempts));

                    CollectHealedTroops();

                    if (collectAttempts >= 4)
                    {
                        Log(Loc.T(Keys.LogStillNotCollected));
                        collectAttempts = 0;
                    }
                    retryCooldown = 1f;
                    return;
                }

                // Idle / None
                collectAttempts = 0;
                if (retryCooldown > 0f) return;

                if (totalInjured == 0 || entries.Count == 0)
                {
                    SetStatus(Keys.MsgWaitingInjured, Color.gray);
                    return;
                }

                long cap = ParseBatchCount();
                var batch = BuildBatch(cap);
                if (batch.Count == 0) return;

                string lack = CheckResourcesForBatch(batch);
                if (lack != null)
                {
                    if (!resourceWarned)
                    {
                        resourceWarned = true;
                        SetStatus(Keys.MsgNotEnoughResources, Color.red);
                        Log(Loc.T(Keys.LogNotEnoughResources, lack));
                    }
                    retryCooldown = 15f;
                    return;
                }
                resourceWarned = false;

                SendCure(batch, false);
                cureSendStreak++;
                SetStatus(Keys.MsgHealingTroops, Color.green);

                if (cureSendStreak >= 3)
                {
                    if (!resourceWarned)
                        Log(Loc.T(Keys.LogCureRejected));
                    cureSendStreak = 0;
                    retryCooldown = 20f;
                    return;
                }

                retryCooldown = 3f;
            }
            catch (System.Exception ex)
            {
                Log(Loc.T(Keys.LogAutoHealError, ex.Message));
                retryCooldown = 1f;
            }
        }

        private void CollectHealedTroops()
        {
            try { HospitalModule.Inst.CheckCureIsDone(); }
            catch (System.Exception ex) { Log(Loc.T(Keys.LogStepFailed, "CheckCureIsDone", ex.Message)); }

            try { HospitalModule.Inst.HospitalInstantSpeedUpReq(); }
            catch (System.Exception ex) { Log(Loc.T(Keys.LogStepFailed, "HospitalInstantSpeedUpReq", ex.Message)); }

            try { CityBuildingModule.API.SendCollectAllTroop(); }
            catch (System.Exception ex) { Log(Loc.T(Keys.LogStepFailed, "SendCollectAllTroop", ex.Message)); }

            try { HospitalModule.Inst.UpdateCureState(); }
            catch (System.Exception ex) { Log(Loc.T(Keys.LogStepFailed, "UpdateCureState", ex.Message)); }

            TryClickHospitalCollect();
        }

        private void UseSpeedUpItems()
        {
            try
            {
                if (!BagModule.IsExist) { Log(Loc.T(Keys.LogSpeedUpBagNotReady)); return; }

                BagCache bag = AppCache.Bag;
                if (bag == null) { Log(Loc.T(Keys.LogSpeedUpCacheNull)); return; }

                var items = bag.GetItemVosByResType(BaseResType.AccelerateTreatment);
                if (items == null || items.Count == 0)
                {
                    if (!speedUpWarned)
                    {
                        speedUpWarned = true;
                        Log(Loc.T(Keys.LogSpeedUpNoneInBag));
                    }
                    return;
                }

                foreach (var it in items)
                {
                    if (it == null || it.Count <= 0) continue;

                    uint cnt = it.Count;
                    uint id = it.Id;
                    BagModule.Inst.UseItem(id, cnt);
                    Log(Loc.T(Keys.LogSpeedUpUsed, id, cnt));
                    speedUpWarned = false;
                    return;
                }

                if (!speedUpWarned)
                {
                    speedUpWarned = true;
                    Log(Loc.T(Keys.LogSpeedUpFinished));
                }
            }
            catch (System.Exception ex)
            {
                Log(Loc.T(Keys.LogSpeedUpFailed, ex.Message));
            }
        }

        private void TryClickHospitalCollect()
        {
            try
            {
                CityBuildingCache cache = AppCache.CityBuilding;
                if (cache == null) { Log(Loc.T(Keys.LogCollectCacheNull)); return; }

                var myCity = cache.MyCompCity;

                if (myCity != null)
                {
                    Building real = myCity.GetBuilding(EBuildingType.Hospital);
                    if (real != null)
                    {
                        var rb = real.TryCast<Building1032>();
                        if (rb != null)
                        {
                            rb.OnCollectBtnClicked(null);
                            Log(Loc.T(Keys.LogCollectButtonClicked));
                            return;
                        }
                    }
                }

                var myCastle = cache.MyCity;
                if (myCastle == null) { Log(Loc.T(Keys.LogCollectCastleNull)); return; }

                BuildingInfo info = myCastle.GetBuildingInfo(EBuildingType.Hospital);
                if (info == null) { Log(Loc.T(Keys.LogCollectInfoNotFound)); return; }

                IntPtr shellPtr = IL2CPP.il2cpp_object_new(
                    Il2CppClassPointerStore<Building1032>.NativeClassPtr);
                if (shellPtr == IntPtr.Zero) { Log(Loc.T(Keys.LogCollectShellAllocFailed)); return; }

                var shell = new Building1032(shellPtr);

                try { if (myCity != null) { shell.m_ownerCom = myCity; } } catch { }
                try { shell.m_state = HospitalCacheVo.CureState.CureDone; } catch { }
                try { shell.m_isMyCity = true; } catch { }
                try
                {
                    var cured = lastVo != null ? lastVo.GetTotalCuringList() : null;
                    if (cured != null && cured.Count > 0)
                        shell.m_armiesToCollect = cured;
                }
                catch { }

                shell.OnCollectBtnClicked(null);
                Log(Loc.T(Keys.LogCollectShellClicked));
            }
            catch (System.Exception ex)
            {
                Log(Loc.T(Keys.LogCollectClickFailed, ex.Message));
            }
        }

        private string CheckResourcesForBatch(System.Collections.Generic.List<ArmyEntry> batch)
        {
            try
            {
                var armyInfos = new Il2CppSystem.Collections.Generic.List<ArmyInfo>();
                foreach (var e in batch)
                {
                    var ai = new ArmyInfo();
                    ai.ArmyId = e.ArmyId;
                    ai.Count = e.Count;
                    armyInfos.Add(ai);
                }

                var require = new Il2CppSystem.Collections.Generic.List<Il2CppIGG.Game.Data.Cache.Common.ResourceData>();
                HospitalModule.Inst.GetArmyListCureResource(armyInfos, require);

                var buffer = new Il2CppSystem.Collections.Generic.List<Il2CppIGG.Game.Data.Cache.Common.ResourceData>();
                var requireList = require.TryCast<Il2CppSystem.Collections.Generic.IList<Il2CppIGG.Game.Data.Cache.Common.ResourceData>>();
                bool enough = HospitalModule.Inst.CheckResourceEnough(requireList, buffer);
                if (enough) return null;

                if (buffer != null && buffer.Count > 0)
                {
                    var sb = new System.Text.StringBuilder();
                    for (int i = 0; i < buffer.Count && i < 4; i++)
                    {
                        var rd = buffer[i];
                        if (rd == null) continue;
                        if (sb.Length > 0) sb.Append(", ");
                        sb.Append(Loc.T(Keys.LogShortageType, rd.MainType, rd.SubType, rd.ValueNeed));
                    }
                    if (sb.Length > 0) return sb.ToString();
                }
                return Loc.T(Keys.LogShortageUnknown);
            }
            catch (System.Exception ex)
            {
                Log(Loc.T(Keys.LogResourceCheckFailed, ex.Message));
                return null;
            }
        }

        private long ParseBatchCount()
        {
            long v = 100;
            long.TryParse(uiBatchCount.Trim(), out v);
            if (v < 1) v = 1;
            return v;
        }

        private System.Collections.Generic.List<ArmyEntry> BuildBatch(long cap)
        {
            var batch = new System.Collections.Generic.List<ArmyEntry>();
            long taken = 0;
            foreach (var e in entries)
            {
                if (taken >= cap) break;
                if (e.Count == 0) continue;

                ulong take = e.Count;
                long room = cap - taken;
                if ((long)e.Count > room) take = (ulong)room;

                batch.Add(new ArmyEntry
                {
                    ArmyId = e.ArmyId,
                    Name = e.Name,
                    Rank = e.Rank,
                    IsSpecial = e.IsSpecial,
                    Count = take,
                    Selected = true
                });
                taken += (long)take;
            }
            return batch;
        }

        private void SendCure(System.Collections.Generic.List<ArmyEntry> sel, bool instant)
        {
            try
            {
                if (!HospitalModule.IsExist) { Log(Loc.T(Keys.LogHospitalModuleNotReady)); return; }
                if (sel.Count == 0) { Log(Loc.T(Keys.LogArmyListEmpty)); return; }

                ulong total = 0;
                foreach (var e in sel) total += e.Count;

                SendCityCure(sel, instant, total);
            }
            catch (System.Exception ex)
            {
                Log(Loc.T(Keys.LogCureRequestFailed, ex.Message));
            }
        }

        private void SendCityCure(System.Collections.Generic.List<ArmyEntry> sel, bool instant, ulong total)
        {
            var normal = new System.Collections.Generic.List<ArmyEntry>();
            var special = new System.Collections.Generic.List<ArmyEntry>();
            foreach (var e in sel)
            {
                if (e.IsSpecial) special.Add(e); else normal.Add(e);
            }

            IMessage imsg;
            string kind;

            if (instant)
            {
                var msg = new MsgCL2GSHospitalInstantCureRequest();
                FillRepeated(msg.ArmyInfo, normal);
                FillRepeated(msg.SpecialArmyInfo, special);
                imsg = msg.TryCast<IMessage>();
                kind = Loc.T(Keys.KindInstantCure);
            }
            else
            {
                var msg = new MsgCL2GSHospitalCureRequest();
                FillRepeated(msg.ArmyInfo, normal);
                FillRepeated(msg.SpecialArmyInfo, special);
                imsg = msg.TryCast<IMessage>();
                kind = Loc.T(Keys.KindCure);
            }

            if (imsg == null)
            {
                Log(Loc.T(Keys.LogCastFailed));
                return;
            }

            bool ok = HospitalModule.Inst.SendMsg(imsg);
            Log(ok
                ? Loc.T(Keys.LogCityCureSent, kind, normal.Count, special.Count, total)
                : Loc.T(Keys.LogCityCureFailed, kind));
        }

        private void RequestAllianceHelpOnce()
        {
            if (allianceHelpRequested) return;

            try
            {
                var item = new GuildAssistItem
                {
                    Type = AssistType.KAssistTypeHospital,
                    Id = 0,
                    Level = 0,
                    Param = 0
                };
                GuildModule.API.ReqGuildAssistAsk(item);
                allianceHelpRequested = true;
                Log(Loc.T(Keys.LogGuildAssistSent));
            }
            catch (System.Exception ex)
            {
                Log(Loc.T(Keys.LogGuildAssistFailed, ex.Message));
            }
        }

        private static void FillRepeated(Il2CppGoogle.Protobuf.Collections.RepeatedField<ArmyInfo> field,
            System.Collections.Generic.List<ArmyEntry> source)
        {
            if (field == null) return;
            foreach (var e in source)
            {
                if (e.Count == 0) continue;
                var ai = new ArmyInfo();
                ai.ArmyId = e.ArmyId;
                ai.Count = e.Count;
                field.Add(ai);
            }
        }

        private void Log(string msg)
        {
            string line = $"[{System.DateTime.Now:HH:mm:ss}] {msg}";
            log.Add(line);
            if (log.Count > MaxLogLines) log.RemoveAt(0);
            MelonLogger.Msg("[AutoHealer] " + msg);
        }
    }
}
