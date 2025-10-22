<h1 align="center">
  <br>
  <a href="https://github.com/zfi2/FL-Studio-Discord-RPC"><img src="https://raw.githubusercontent.com/zfi2/FL-Studio-Discord-RPC/refs/heads/main/FLStudioRPC.ico" alt="FL Studio Discord RPC" width="200"></a>
  <br>
  FL Studio Discord RPC
  <br>
</h1>

<h4 align="center">A simple, yet cool way to show off your FL Studio projects to your friends and others.</h4>

<p align="center">
  <a href="#key-features">Key Features</a> •
  <a href="#how-to-use">How To Use</a> •
  <a href="#download">Download</a> •
  <a href="#building-from-source">Building From Source</a> •
  <a href="#packages-used">Packages Used</a> •
  <a href="#license">License</a>
  <a href="#feedback">Feedback</a>
</p>

<p align="center"><img src="https://i.imgur.com/viJFFoI.png"></p>

## Key Features

* **System Tray Integration** - Runs invisibly in the background
* **Auto-Startup** - Optionally launches with Windows (configurable via tray menu)
* **Secret Mode** - Hide your project names from others
* **Conditional RPC** - Only shows Discord activity when FL Studio is actually running
* **Single Instance** - Prevents multiple copies from running simultaneously
* **Accurate Version Display** - Show exact FL Studio version (e.g., FL Studio 20.8.4.1873)
* **JSON Configuration** - Easy-to-manage settings
* **Lightweight & Efficient** - Minimal resource usage

## How To Use

### Installation

1. Download `FLStudioRPC_Setup.exe` from the [Releases](https://github.com/zfi2/FL-Studio-Discord-RPC/releases) page
2. Run the installer
3. Choose your installation location (optional)
4. Select your preferences:
   - Open FL Studio Discord RPC on startup (recommended)
   - Create a desktop icon (optional)
5. Click Install

The app will automatically start and run in your system tray.

### Usage

Once installed, FL Studio Discord RPC runs automatically in the background:

1. **System Tray Icon** - Look for the icon in your system tray (bottom-right, near the clock)
2. **Right-click the icon** to access:
   - Secret Mode (Hide Project Name) - Toggle project name visibility
   - Start with Windows - Toggle auto-startup
   - About - Opens the GitHub repository
   - Exit - Closes the application

The app will automatically detect when FL Studio is running and update your Discord status accordingly.

> **Note**
> When FL Studio is closed, your Discord activity will be cleared automatically.

## Download

You can [download](https://github.com/zfi2/FL-Studio-Discord-RPC/releases) the latest installer for Windows.

**System Requirements:**
- Windows 10/11
- .NET Framework 4.8.1 (usually pre-installed)
- Discord running on your PC

## Building From Source

This project uses .NET Framework 4.8.1 and can be built with Visual Studio 2022 or MSBuild.

**Prerequisites:**
- Visual Studio 2022 (Community Edition or higher) OR Visual Studio Build Tools
- .NET Framework 4.8.1 SDK

**Steps:**
1. Clone the repository
2. Open `FLStudioRPC.sln` in Visual Studio
3. Build the solution (Ctrl+Shift+B)
4. The executable will be in `bin\Release\FLStudioRPC.exe`

**Creating the Installer:**
1. Install [Inno Setup](https://jrsoftware.org/isdl.php)
2. Right-click `installer.iss` and select "Compile"
3. The installer will be created in `installer_output\FLStudioRPC_Setup.exe`

## Feedback

If you have any feedback, reach out to me on discord (which is available on [my website](https://lain.ovh/))

## Packages Used

[DiscordRichPresence](https://github.com/Lachee/discord-rpc-csharp) - Discord Rich Presence library
[Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) - JSON serialization
[Colorful.Console](https://github.com/tomakita/Colorful.Console) - Console output formatting

## License

This project is licensed under the [MIT](https://opensource.org/license/mit/) License - see the [LICENSE](LICENSE) file for details.

---

> GitHub [@zfi2](https://github.com/zfi2)
