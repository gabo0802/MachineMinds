using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Provides utility methods for handling save/load operations on the WebGL platform.
/// Uses PlayerPrefs as an alternative to file I/O operations.
/// </summary>
public static class WebGLSaveSystem
{
    /// <summary>
    /// Determines if the application is running on the WebGL platform.
    /// </summary>
    /// <returns>True if running on WebGL, otherwise false.</returns>
    public static bool IsWebGLPlatform()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer;
    }

/// <summary>
/// Checks if a "file" exists in PlayerPrefs storage.
/// </summary>
/// <param name="filePath">The path/key to check.</param>
/// <returns>True if the key exists in PlayerPrefs, otherwise false.</returns>
public static bool FileExists(string filePath)
{
    return PlayerPrefs.HasKey(filePath);
}

/// <summary>
/// Reads text content from PlayerPrefs.
/// </summary>
/// <param name="filePath">The path/key to read from.</param>
/// <returns>The string content stored at the specified key.</returns>
public static string ReadAllText(string filePath)
{
    if (!FileExists(filePath))
    {
        Debug.LogWarning($"Attempted to read non-existent file: {filePath}");
        return string.Empty;
    }
    
    return PlayerPrefs.GetString(filePath);
}

/// <summary>
/// Writes text content to PlayerPrefs.
/// </summary>
/// <param name="filePath">The path/key to write to.</param>
/// <param name="content">The string content to store.</param>
public static void WriteAllText(string filePath, string content)
{
    PlayerPrefs.SetString(filePath, content);
    PlayerPrefs.Save();
}

/// <summary>
/// Deletes a "file" from PlayerPrefs storage.
/// </summary>
/// <param name="filePath">The path/key to delete.</param>
public static void DeleteFile(string filePath)
{
    if (FileExists(filePath))
    {
        PlayerPrefs.DeleteKey(filePath);
        PlayerPrefs.Save();
    }
}

/// <summary>
/// Gets all "files" (keys) that match a specific pattern.
/// </summary>
/// <param name="directoryPath">The directory path pattern to match.</param>
/// <param name="searchPattern">The search pattern for files.</param>
/// <returns>An array of matching keys.</returns>
public static string[] GetFiles(string directoryPath, string searchPattern = "*")
{
    // Since PlayerPrefs doesn't support listing keys, we need to track them manually
    // This is a helper method to retrieve a list of keys that would represent "files" 
    // in a specific "directory". Implementation would require additional tracking.
    
    // Get the index of keys from a special tracking key
    string keysIndex = PlayerPrefs.GetString("WebGLSaveSystem_KeysIndex", "");
    List<string> allKeys = new List<string>(keysIndex.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries));
    
    // Filter keys based on the directory path pattern
    List<string> matchingKeys = new List<string>();
    foreach (string key in allKeys)
    {
        if (key.StartsWith(directoryPath))
        {
            // Further filter based on searchPattern if needed
            // Simple implementation just returns all keys in the "directory"
            matchingKeys.Add(key);
        }
    }
    
    return matchingKeys.ToArray();
}

/// <summary>
/// Adds a key to the tracking index.
/// </summary>
/// <param name="key">The key to add to the tracking index.</param>
private static void AddKeyToIndex(string key)
{
    string keysIndex = PlayerPrefs.GetString("WebGLSaveSystem_KeysIndex", "");
    List<string> keys = new List<string>(keysIndex.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries));
    
    if (!keys.Contains(key))
    {
        keys.Add(key);
        PlayerPrefs.SetString("WebGLSaveSystem_KeysIndex", string.Join("|", keys));
        PlayerPrefs.Save();
    }
}

/// <summary>
/// Removes a key from the tracking index.
/// </summary>
/// <param name="key">The key to remove from the tracking index.</param>
private static void RemoveKeyFromIndex(string key)
{
    string keysIndex = PlayerPrefs.GetString("WebGLSaveSystem_KeysIndex", "");
    List<string> keys = new List<string>(keysIndex.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries));
    
    if (keys.Contains(key))
    {
        keys.Remove(key);
        PlayerPrefs.SetString("WebGLSaveSystem_KeysIndex", string.Join("|", keys));
        PlayerPrefs.Save();
    }
}
}
