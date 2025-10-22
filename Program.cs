using DiscordRPC;
using DiscordRPC.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

using System.Drawing;
using Console = Colorful.Console;

// Configuration settings
using static ConfigSettings;

// ClientID and settings
using static ConfigValues;

// Memory stuff
using static Utils;

public static class Program
{
    // Initialize a new Rich Presence client
    public static DiscordRpcClient _Client;

    // Configuration file location
    public static readonly string ConfigPath = "fls_rpc_config.json";

    // System tray icon
    private static NotifyIcon _trayIcon;

    // Mutex for single-instance checking
    private static Mutex _mutex;

    // Initialize a placeholder, because we don't want null reference exceptions
    private static RichPresence _RPC = new RichPresence()
    {
        Details = "",
        State = "",
        Assets = new Assets()
        {
            LargeImageKey = "fl_studio_logo",
        }
    };

    static void InitializeRPC()
    {
        // Create a new Rich Presence client, set the client ID, and most importantly, disable the autoEvents so we can update manually
        _Client = new DiscordRpcClient(ClientID, -1, null, false);

        // Don't update the presence if it didn't change
        _Client.SkipIdenticalPresence = true;

        // Use a file logger instead of console logger since we're running as WinExe
        _Client.Logger = new DiscordRPC.Logging.FileLogger("discord_rpc_log.txt")
        {
            Level = DiscordRPC.Logging.LogLevel.Warning
        };

        // Register events
        _Client.OnReady += Events.OnReady;
        _Client.OnClose += Events.OnClose;
        _Client.OnError += Events.OnError;
        _Client.OnConnectionEstablished += Events.OnConnectionEstablished;
        _Client.OnConnectionFailed += Events.OnConnectionFailed;
        _Client.OnPresenceUpdate += Events.OnPresenceUpdate;

        // Initialize the client
        _Client.Initialize();
    }

    static void InitializeTrayIcon()
    {
        // Load the custom icon
        Icon customIcon = null;
        try
        {
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FLStudioRPC.ico");
            if (File.Exists(iconPath))
            {
                customIcon = new Icon(iconPath);
            }
        }
        catch
        {
            // Fall back to system icon if loading fails
        }

        _trayIcon = new NotifyIcon()
        {
            Icon = customIcon ?? SystemIcons.Application,
            Visible = true,
            Text = "FL Studio Discord RPC"
        };

        // Create context menu
        var contextMenu = new ContextMenuStrip();

        // Secret mode toggle
        var secretModeItem = new ToolStripMenuItem("Secret Mode (Hide Project Name)");
        secretModeItem.Checked = SecretMode;
        secretModeItem.Click += (s, e) =>
        {
            SecretMode = !SecretMode;
            secretModeItem.Checked = SecretMode;
            // Save the current setting to config
            SaveCurrentConfig(ConfigPath);
        };
        contextMenu.Items.Add(secretModeItem);

        contextMenu.Items.Add(new ToolStripSeparator());

        // Auto-startup toggle
        var autoStartItem = new ToolStripMenuItem("Start with Windows");
        autoStartItem.Checked = IsAutoStartEnabled();
        autoStartItem.Click += (s, e) =>
        {
            bool currentState = IsAutoStartEnabled();
            SetAutoStart(!currentState);
            autoStartItem.Checked = !currentState;
        };
        contextMenu.Items.Add(autoStartItem);

        contextMenu.Items.Add(new ToolStripSeparator());

        // About
        var aboutItem = new ToolStripMenuItem("About");
        aboutItem.Click += (s, e) =>
        {
            System.Diagnostics.Process.Start("https://github.com/zfi2/FL-Studio-Discord-RPC");
        };
        contextMenu.Items.Add(aboutItem);

        // Exit
        var exitItem = new ToolStripMenuItem("Exit");
        exitItem.Click += (s, e) =>
        {
            _trayIcon.Visible = false;
            _Client?.Dispose();
            Application.Exit();
        };
        contextMenu.Items.Add(exitItem);

        _trayIcon.ContextMenuStrip = contextMenu;
    }

    static bool IsAutoStartEnabled()
    {
        try
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
            {
                return key?.GetValue("FLStudioRPC") != null;
            }
        }
        catch
        {
            return false;
        }
    }

    static void SetAutoStart(bool enable)
    {
        try
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (enable)
                {
                    string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    key?.SetValue("FLStudioRPC", $"\"{exePath}\"");
                }
                else
                {
                    key?.DeleteValue("FLStudioRPC", false);
                }
            }
        }
        catch
        {
            // Silently fail if registry access denied
        }
    }

    static void RunRPCLoop()
    {
        // Save default config with default values (also load it at startup, the function is already called in SaveConfig)
        SaveConfig(ConfigPath);

        bool wasRunning = false;

        // Continuous monitoring loop
        while (true)
        {
            try
            {
                // Retrieve the FL Studio data constantly
                FLInfo FLStudioData = GetFLInfo();

                // Check if FL Studio is running
                bool isRunning = !string.IsNullOrEmpty(FLStudioData.AppName) || !string.IsNullOrEmpty(FLStudioData.ProjectName);

                if (isRunning)
                {
                    // FL Studio is running
                    if (!wasRunning)
                    {
                        // FL Studio just started - initialize RPC
                        InitializeRPC();

                        // Initialize timestamp if enabled
                        if (ShowTimestamp)
                        {
                            _RPC.Timestamps = new Timestamps()
                            {
                                Start = DateTime.UtcNow
                            };
                        }

                        wasRunning = true;
                    }

                    // Update presence with current FL Studio info
                    _RPC.Details = FLStudioData.AppName;
                    _RPC.State = FLStudioData.ProjectName ?? "Empty project";

                    // Check if secret mode is enabled
                    if (SecretMode)
                        _RPC.State = "Working on a hidden project";

                    // Invoke event handlers and set presence
                    _Client?.Invoke();
                    _Client?.SetPresence(_RPC);
                }
                else
                {
                    // FL Studio is not running
                    if (wasRunning)
                    {
                        // FL Studio just closed - clear presence and dispose client
                        _Client?.ClearPresence();
                        _Client?.Dispose();
                        _Client = null;
                        wasRunning = false;
                    }
                }

                // Sleep for the interval defined in the config file
                Thread.Sleep(UpdateInterval);
            }
            catch (Exception ex)
            {
                // Log errors but keep running
                File.WriteAllText("rpc_error.txt", $"Error: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }

    [STAThread]
    static void Main(string[] args)
    {
        // Check if another instance is already running
        bool createdNew;
        _mutex = new Mutex(true, "FLStudioRPC_SingleInstance", out createdNew);

        if (!createdNew)
        {
            // Another instance is already running
            MessageBox.Show(
                "FL Studio Discord RPC is already running.\n\nCheck your system tray for the icon.",
                "Already Running",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            return; // Exit this instance
        }

        // Enable auto-start by default if not already configured
        if (!IsAutoStartEnabled())
        {
            SetAutoStart(true);
        }

        // Initialize system tray icon
        InitializeTrayIcon();

        // Start RPC monitoring in a background thread
        Thread rpcThread = new Thread(RunRPCLoop)
        {
            IsBackground = true
        };
        rpcThread.Start();

        // Keep the application running (for the tray icon)
        Application.Run();

        // Release the mutex when application exits
        _mutex?.ReleaseMutex();
        _mutex?.Dispose();
    }
}
