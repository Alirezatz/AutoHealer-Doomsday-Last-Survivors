# Auto Healer FeStiCal

An Auto Healer mod for **Doomsday: Last Survivors**, running through MelonLoader.

## Installation and usage

1. Install MelonLoader if you have not already. Get it from the [official MelonLoader repository](https://github.com/lavagang/melonloader).
2. Launch the game once after installing MelonLoader. The first launch may take longer while MelonLoader initializes and injects the loader and generates the files it needs. Later launches should be faster.
3. Close the game completely after that first launch.
4. Download [`AutoHealer.zip` from the v1.0.0 release](https://github.com/Alirezatz/AutoHealer-Doomsday-Last-Survivors/releases/tag/v1.0.0).
5. Extract the ZIP contents directly into the game's `Mods` folder, which MelonLoader creates. The DLL should be directly inside `Mods`, not inside an extra nested folder.
6. Launch the game again and check the MelonLoader log. A successful load should show:

   - `Auto Healer FeStiCal v1.0.0`
   - `by AlirezaFeStiCal`
   - `Assembly: AutoHealer.dll`
   - `1 Mod loaded`

   **Note:** `ClassLibrary2.dll` was mentioned in the original request, but the supplied release ZIP contains `AutoHealer.dll`. Its managed assembly name is `AutoHealer`, so `AutoHealer.dll` is the expected assembly name in the log for this release.

After the mod loads successfully, you can play the game. Press `F8` to open or close the mod panel (default key).

## Release

Download the ready-to-install ZIP from the [v1.0.0 release](https://github.com/Alirezatz/AutoHealer-Doomsday-Last-Survivors/releases/tag/v1.0.0). The ZIP is provided as a release asset and is not included in the source repository.

## Source project

The Visual Studio project is `AutoHealer/AutoHealer.csproj`. Building it requires the MelonLoader and game assemblies referenced by the project file to be available in your development environment.

---

## راهنمای فارسی

این مود برای بازی **Doomsday: Last Survivors** و لودر MelonLoader است.

### نصب و اجرا

1. اگر MelonLoader را نصب نکرده‌اید، آن را از [مخزن رسمی MelonLoader](https://github.com/lavagang/melonloader) دریافت و نصب کنید.
2. پس از نصب MelonLoader، بازی را یک بار اجرا کنید. اجرای اول ممکن است طولانی‌تر باشد؛ MelonLoader در این اجرا لودر را راه‌اندازی و تزریق می‌کند و فایل‌های موردنیاز را می‌سازد. اجراهای بعدی باید سریع‌تر باشند.
3. پس از اجرای اول، بازی را کاملاً ببندید.
4. فایل [`AutoHealer.zip` را از انتشار v1.0.0](https://github.com/Alirezatz/AutoHealer-Doomsday-Last-Survivors/releases/tag/v1.0.0) دانلود کنید.
5. محتویات ZIP را مستقیماً در پوشهٔ `Mods` بازی استخراج کنید؛ این پوشه را MelonLoader ایجاد می‌کند. فایل DLL باید مستقیماً داخل `Mods` باشد، نه در یک زیرپوشهٔ اضافه.
6. بازی را دوباره اجرا کنید و گزارش MelonLoader را بررسی کنید. بارگذاری موفق باید شامل این موارد باشد:

   - `Auto Healer FeStiCal v1.0.0`
   - `by AlirezaFeStiCal`
   - `Assembly: AutoHealer.dll`
   - `1 Mod loaded`

   **توجه:** در درخواست اولیه نام `ClassLibrary2.dll` آمده بود، اما ZIP ارائه‌شده شامل `AutoHealer.dll` است. نام اسمبلی داخل آن `AutoHealer` است؛ بنابراین نام مورد انتظار برای این انتشار در گزارش `AutoHealer.dll` است.

پس از بارگذاری موفق مود می‌توانید بازی کنید. برای باز و بسته کردن پنل مود، کلید پیش‌فرض `F8` است.

### کد منبع

پروژهٔ Visual Studio در `AutoHealer/AutoHealer.csproj` قرار دارد. برای ساخت آن باید اسمبلی‌های MelonLoader و بازی که در فایل پروژه به آن‌ها ارجاع داده شده است، در محیط توسعه موجود باشند.
