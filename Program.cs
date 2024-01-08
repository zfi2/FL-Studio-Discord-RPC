using DiscordRPC;
using DiscordRPC.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;

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


    // Initialize a placeholder, because we don't want null reference exceptions
    // There shouldn't be any, but just in case
    private static RichPresence _RPC = new RichPresence()
    {
        Details = "",
        State = "",
        Assets = new Assets()
        {
            LargeImageKey = "fl_studio_logo",
        }
    };

    // Store the previous state
    private static RichPresence _previousRPCState;

    static void InitializeRPC()
    {
        // Create a new Rich Presence client, set the client ID, and most importantly, disable the autoEvents so we can update manually
        _Client = new DiscordRpcClient(ClientID, -1, null, false);

        // Create a console logger
        _Client.Logger = new ConsoleLogger() { Level = LogLevel.Warning, Coloured = true };

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

    // Helper method to check if the two Rich Presences are equal
    static bool IsEqual(RichPresence rpc1, RichPresence rpc2)
    {
        return rpc1?.Details == rpc2?.Details &&
               rpc1?.State == rpc2?.State &&
               rpc1?.Assets?.LargeImageKey == rpc2?.Assets?.LargeImageKey;
    }

    static void Main(string[] args)
    {
        // Save default config with default values (also load it, the function is already defined in SaveConfig)
        SaveConfig(ConfigPath);

        // If DisplayConfigInfo is enabled, read all settings from file and print them
        if (DisplayConfigInfo)
        {
            try
            {
                // Read JSON from file
                string json = File.ReadAllText(ConfigPath);

                // Deserialize JSON to dictionary
                var properties = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                // Print each setting to console
                foreach (var property in properties)
                {
                    Console.WriteLine($"{property.Key} => {property.Value}");
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., file not found, invalid JSON format)
                Console.WriteLine($"Error loading configuration from file: {ex.Message}");
            }
            // Ensure an extra newline after the configuration output
            Console.WriteLine();
        }

        // Initialize the Rich Presence
        InitializeRPC();

        // Initialize a timestamp
        _RPC.Timestamps = new Timestamps()
        {
            Start = DateTime.UtcNow
        };

        // If client is valid, continue the loop
        while (_Client != null)
        {
            // Retrieve the FL Studio data in a loop, so that we're up to date
            FLInfo FLStudioData = GetFLInfo();

            if (_Client != null)
                _Client.Invoke();

            // Check if AppName and ProjectName are both empty or null
            bool NoProject = string.IsNullOrEmpty(FLStudioData.AppName) && string.IsNullOrEmpty(FLStudioData.ProjectName);

            // Set Details and State based on conditions
            _RPC.Details = NoProject ? "FL Studio (inactive)" : FLStudioData.AppName;
            _RPC.State = NoProject ? "No project" : FLStudioData.ProjectName ?? "Empty project";

            // Check if secret mode is enabled and set the state accordingly
            if (SecretMode == true)
                _RPC.State = "Working on a hidden project";

            // Check if the state has changed before updating
            if (!IsEqual(_RPC, _previousRPCState))
            {
                // Update the presence and store the current state as the previous state
                _Client?.SetPresence(_RPC);

                // Clone to avoid reference issues
                _previousRPCState = _RPC.Clone();
            }

            Thread.Sleep(UpdateInterval);
        }
    }
}