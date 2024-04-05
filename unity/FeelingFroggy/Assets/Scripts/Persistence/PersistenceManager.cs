using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Manages persistent storage of data.
/// </summary>
public static class PersistenceManager
{
    /// <summary>
    /// Get file path for a given key.
    /// </summary>
    /// <param name="key">The given key for the file.</param>
    /// <returns>The file path.</returns>
    public static string GetFilePath(string key)
    {
        return Path.Combine(Application.persistentDataPath, key + ".json");
    }

    /// <summary>
    /// Loads data from persistent storage (local file).
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="key">The key that identifies the data.</param>
    /// <returns>The data stored, or <b>null</b> if none.</returns>
    public static T Load<T>(string key) where T : class
    {
        var filePath = GetFilePath(key);

        if (!File.Exists(filePath))
        {
            return null;
        }

        var fileContents = File.ReadAllText(filePath);

        return JsonConvert.DeserializeObject<T>(fileContents);
    }

    /// <summary>
    /// Saves data to persistent storage (local file).
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="key">The key that identifies the data.</param>
    /// <param name="data">The data stored, or <b>null</b> to delete any existing data.</param>
    public static void Save<T>(string key, T data) where T : class
    {
        var filePath = GetFilePath(key);

        if (data != null)
        {
            // Save data to file
            var fileContents = JsonConvert.SerializeObject(data);

            File.WriteAllText(filePath, fileContents);
        }
        else if (File.Exists(filePath))
        {
            // Delete file if exists and data is null
            File.Delete(filePath);
        }
    }
}
