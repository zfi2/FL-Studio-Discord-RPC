using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

public static class ConfigValues
{
    [DefaultValue("1192880494086455357")]
    public static string ClientID { get; set; }

    [DefaultValue(true)]
    public static bool SecretMode { get; set; }

    [DefaultValue(false)]
    public static bool DisplayConfigInfo { get; set; }

    [DefaultValue(false)]
    public static bool AccurateVersion { get; set; }

    [DefaultValue(4000)]
    public static int UpdateInterval { get; set; }
}

public static class ConfigSettings
{
    static Dictionary<string, object> GetPropertiesWithDefaults<T>(T obj)
    {
        var properties = typeof(T).GetProperties();
        var result = new Dictionary<string, object>();

        foreach (var property in properties)
        {
            var defaultValueAttribute = (DefaultValueAttribute)Attribute.GetCustomAttribute(property, typeof(DefaultValueAttribute));
            var defaultValue = defaultValueAttribute?.Value;

            result.Add(property.Name, defaultValue);
        }

        return result;
    }

    private static object ConvertValue(object value, Type targetType)
    {
        // Side note: most of those are not needed
        // Convert the value to the appropriate type
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
            // For other types, assume it's convertible
            return value;
        }
    }

    private static void SetValues(Dictionary<string, object> properties)
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
                // Handle missing property in loaded configuration
                Console.WriteLine($"Property {prop.Name} not found in the loaded configuration.");
            }
        }
    }


    public static void SaveConfig(string filePath)
    {
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
                    if (prop.PropertyType == typeof(bool))
                    {
                        return defaultValueAttribute.Value;
                    }
                    else if (prop.PropertyType == typeof(int))
                    {
                        return Convert.ToInt32(defaultValueAttribute.Value);
                    }
                    // Add other types if needed

                    return prop.GetValue(null) ?? defaultValueAttribute.Value.ToString();
                });

        // Serialize the properties to JSON
        string json = JsonConvert.SerializeObject(properties, Formatting.Indented);

        // If the config file doesn't exist, create one with default values
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Configuration file not found. Creating one with default values.\n");

            // Save JSON to file
            File.WriteAllText(filePath, json);

            // Load the config
            LoadConfig(filePath);
        }
        else
        {
            // Config file exists, so load it
            Console.WriteLine("Configuration file present, loading values...\n");

            LoadConfig(filePath);
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
            Console.WriteLine($"Error loading configuration: {ex.Message}");
        }
    }
}
