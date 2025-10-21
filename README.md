# FL Studio Discord RPC

A simple, yet cool way to show off your FL Studio projects to your friends and others.


## Features

- Runs invisibly in the background with a system tray icon
- Optionally launches with Windows (configurable via tray menu)
- Secret mode (meaning others can't see what project you're working on)
- Display optional accurate FL Studio version (ex. FL Studio 20.8.4.1873)
- A JSON-based easy-to-manage configuration system

## Pros
- Very lightweight and resource efficient
- Uses only 3 external packages
- Almost everything is commented, so the code is easily manageable and readable

## Cons
- No integration into actual FL Studio (meaning it must run in the background while FL Studio is running)

## Screenshots

![RPC Screenshot 2](https://i.imgur.com/viJFFoI.png)

## Installation

### Using the Installer (Recommended)
1. Download `FLStudioRPC_Setup.exe` from the [Releases](https://github.com/zfi2/FL-Studio-Discord-RPC/releases) page
2. Run the installer
3. Choose installation location (optional)
4. Select options:
   - Open FL Studio Discord RPC on startup (recommended)
   - Create a desktop icon (optional)
5. Click Install

The app will run in your system tray. Right-click the tray icon to access settings or exit.

### Manual Installation
You can also simply run the pre-compiled release, or build it yourself! This project uses .NET Framework 4.8.1
## Feedback

If you have any feedback, reach out to me on discord - my username is on my website, which is on my github profile


## Usage

Once installed, FL Studio Discord RPC runs automatically in the background:

1. **System Tray Icon** - Look for the FL Studio Discord RPC icon in your system tray (bottom-right, near the clock)
2. **Right-click the icon** to access:
   - Open FL Studio Discord RPC on startup - Toggle auto-start
   - About - Opens the GitHub repository
   - Exit - Closes the application

The app will automatically detect when FL Studio is running and update your Discord status.

## Features that may (or may not) be added in the future

- Actual FL Studio integration
- More features



## Packages used

[DiscordRichPresence](https://github.com/Lachee/discord-rpc-csharp)\
[Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)\
[Colorful.Console](https://github.com/tomakita/Colorful.Console)



## License

This project is licensed under the [MIT](https://opensource.org/license/mit/) License - see the [LICENSE](LICENSE) file for details.
