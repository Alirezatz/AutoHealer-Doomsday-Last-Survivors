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

After the mod loads successfully, you can play the game. Press `F8` to open or close the mod panel (default key).

## Release

Download the ready-to-install ZIP from the [v1.0.0 release](https://github.com/Alirezatz/AutoHealer-Doomsday-Last-Survivors/releases/tag/v1.0.0). The ZIP is provided as a release asset and is not included in the source repository.

## Community

Join the [Doomsday FT Telegram group](https://t.me/DoomsdayFTGroup) to connect with the community.

## Source project

The Visual Studio project is `AutoHealer/AutoHealer.csproj`. Building it requires the MelonLoader and game assemblies referenced by the project file to be available in your development environment.
