﻿using System;
using System.Diagnostics;
using System.IO;

// ClientID and settings
using static ConfigValues; 

public static class Utils
{
    public static string GetMainWindowsTitleByProcessNames(params string[] processNames)
    {
        // Iterate through the provided process names5
        foreach (var processName in processNames)
        {
            try
            {
                // Attempt to get processes by the given name
                Process[] processes = Process.GetProcessesByName(processName);

                // Check if any processes were found
                if (processes.Length > 0)
                {
                    // Return the main window title of the first process found
                    return processes[0].MainWindowTitle;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and print an error message
                Console.WriteLine($"Error finding main window title by process name: {ex.Message}");

                // Return null in case of an exception
                return null;
            }
        }

        // Return null if no processes with the specified names were found
        return null;
    }

    public static Version GetApplicationVersion(string processName)
    {
        // Get processes by the given name
        Process[] processes = Process.GetProcessesByName(processName);

        // Check if any processes were found
        if (processes.Length > 0)
        {
            // Get the file path of the main module (executable) of the first process found
            string filePath = processes[0].MainModule.FileName;

            // Check if the file path is not empty and the file exists
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                    // Retrieve version information from the file
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(filePath);

                    // Return a Version object created from the file version information
                    return new Version(versionInfo.FileVersion);
                }
                catch (Exception ex)
                {
                    // Handle exceptions and print an error message
                    Console.WriteLine($"Error retrieving version information: {ex.Message}");
                }
            }
        }

        // Return null if no processes with the specified name were found, or if version information couldn't be retrieved
        return null;
    }

    public static FLInfo GetFLInfo()
    {
        // Create a new FLInfo object to store FL Studio's information
        FLInfo Info = new FLInfo();

        // Get FL Studio's title from the main window
        string fullTitle = GetMainWindowsTitleByProcessNames("FL", "FL64");

        // Check if the title is empty or null
        if (string.IsNullOrEmpty(fullTitle))
        {
            // Set project name and app name to null if the title is empty or null
            Info.ProjectName = null;
            Info.AppName = null;
        }
        else
        {
            // Check if accurate version information is enabled in the config
            if (AccurateVersion)
            {
                // Retrieve the version information for FL Studio
                Version accurateVersion = GetApplicationVersion("FL64") ?? GetApplicationVersion("FL");

                // Set the app name to "FL Studio + for example 20.5.2.1576" if version information is available,
                // otherwise set it to null
                Info.AppName = accurateVersion != null ? $"FL Studio {accurateVersion}" : null;
            }
            else
            {
                int hyphenIndex = fullTitle.IndexOf('-');

                Info.ProjectName = hyphenIndex == -1 ? null : fullTitle.Substring(0, hyphenIndex).Trim();
                Info.AppName = hyphenIndex == -1 ? fullTitle.Trim() : fullTitle.Substring(hyphenIndex + 1).Trim();
            }
        }

        // Return FLInfo object containing FL Studio's data
        return Info;
    }

    public struct FLInfo
    {
        public string AppName { get; set; }
        public string ProjectName { get; set; }
        public string AccurateVersion { get; set; }
    }
}