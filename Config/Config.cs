using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using System.Drawing;
using Console = Colorful.Console;
using System.Runtime.InteropServices;

public static class ConfigValues
{
    [DefaultValue("1192880494086455357")]
    public static string ClientID { get; set; }

    [DefaultValue(false)]
    public static bool SecretMode { get; set; }

    [DefaultValue(true)]
    public static bool ShowTimestamp { get; set; }

    [DefaultValue(true)]
    public static bool DisplayConfigInfo { get; set; }

    [DefaultValue(false)]
    public static bool AccurateVersion { get; set; }

    [DefaultValue(4000)]
    public static int UpdateInterval { get; set; }
}

public static class ConfigSettings
{
    private static object ConvertValue(object value, Type targetType)
    {
        try
        {
            // Convert the value to the appropriate type
            // Side note: most of those are not needed, but implemented anyways just in case
            if (targetType == typeof(bool))
            {
                return Convert.ToBoolean(value);
            }
            else if (targetType == typeof(int))
            {
                return Convert.ToInt32(value);
            }
            else if (targetType == typeof(long))
            {
                return Convert.ToInt64(value);
            }
            else if (targetType == typeof(float))
            {
                return Convert.ToSingle(value);
            }
            else if (targetType == typeof(double))
            {
                return Convert.ToDouble(value);
            }
            else if (targetType == typeof(decimal))
            {
                return Convert.ToDecimal(value);
            }
            else
            {
                // Assume it's convertible for other types
                return value;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Couldn't convert configuration values to the appropriate types: {ex.Message}", Color.Red);
            Utils.LogException(ex, "ConvertValue");
            return null;
        }
    }

    private static void SetValues(Dictionary<string, object> properties)
    {
        try
        {
            // Get properties with default values and their attributes
            var configProperties = typeof(ConfigValues)
                .GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(DefaultValueAttribute)));

            foreach (var prop in configProperties)
            {
                if (properties.TryGetValue(prop.Name, out var value))
                {
                    // Convert the value to the appropriate type
                    object convertedValue = ConvertValue(value, prop.PropertyType);

                    // Set the value to the ConfigValues property
                    prop.SetValue(null, convertedValue);
                }
                else
                {
                    // Handle missing properties in the loaded configuration
                    Console.WriteLine($"Property {prop.Name} not found in the loaded configuration.", Color.Red);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting configuration values: {ex.Message}", Color.Red);
            Utils.LogException(ex, "SetValues");
        }
    }


    public static void SaveCurrentConfig(string filePath)
    {
        try
        {
            // Ensure directory exists
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Get current values from ConfigValues properties
            var properties = typeof(ConfigValues)
                .GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(DefaultValueAttribute)))
                .ToDictionary(
                    prop => prop.Name,
                    prop => prop.GetValue(null) // Get current value, not default
                );

            // Serialize the current properties to JSON
            string json = JsonConvert.SerializeObject(properties, Formatting.Indented);

            // Write to file
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving current configuration: {ex.Message}", Color.Red);
            Utils.LogException(ex, "SaveCurrentConfig");
        }
    }

    public static void SaveConfig(string filePath)
    {
        try
        {
            // Ensure directory exists
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Get properties with default values and their attributes
            var properties = typeof(ConfigValues)
                .GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(DefaultValueAttribute)))
                .ToDictionary(
                    prop => prop.Name,
                    prop =>
                    {
                        var defaultValueAttribute = (DefaultValueAttribute)Attribute.GetCustomAttribute(prop, typeof(DefaultValueAttribute));

                    // Handle different types
                    return ConvertValue(defaultValueAttribute.Value, prop.PropertyType);
                    });

            // Serialize the properties to JSON
            string json = JsonConvert.SerializeObject(properties, Formatting.Indented);

            // If the config file doesn't exist, create one with default values
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Configuration file not found. Creating one with default values.\n", Color.LightSkyBlue);

                // Save JSON to file
                File.WriteAllText(filePath, json);
            }
            else
            {
                // Config file does exist
                Console.WriteLine("Configuration file present, loading values...\n", Color.LimeGreen);
            }

            // Load the configuration even if the config file doesn't exist,
            // because it already has been created and exists now (I love coding!)
            LoadConfig(filePath);
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., file I/O, JSON serialization, conversion)
            Console.WriteLine($"Error saving configuration: {ex.Message}", Color.Red);
            Utils.LogException(ex, "SaveConfig");
        }
    }

    public static void LoadConfig(string filePath)
    {
        try
        {
            // Read JSON from file
            string json = File.ReadAllText(filePath);

            // Deserialize JSON to dictionary
            var properties = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            // Set values to ConfigValues
            SetValues(properties);
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., file not found, invalid JSON format)
            Console.WriteLine($"Error loading configuration: {ex.Message}", Color.Red);
            Utils.LogException(ex, "LoadConfig");
        }
    }
}
