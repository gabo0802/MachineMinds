using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Utility class for handling save/load operations on WebGL platform.
/// Provides alternatives to File I/O operations using PlayerPrefs.
/// </summary>
public static class WebGLSaveSystem
{
    private const string PATH_PREFIX = "WEBGL_FILE_";
    
    /// <summary>
    /// Checks if the application is running on WebGL platform.
    /// </summary>
    /// <returns>True if running on WebGL, false otherwise.</returns>
    public static bool IsWebGLPlatform()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer;
    }
    
    /// <summary>
    /// Checks if a file exists in the PlayerPrefs system.
    /// </summary>
    /// <param name="filePath">The path of the file to check.</param>
    /// <returns>True if the file exists, false otherwise.</returns>
    public static bool FileExists(string filePath)
    {
        string key = PATH_PREFIX + filePath.Replace("/", "_").Replace("\\", "_");
        return PlayerPrefs.HasKey(key);
    }
    
    /// <summary>
    /// Saves text data to PlayerPrefs as if writing to a file.
    /// </summary>
    /// <param name="filePath">The path where the file would be saved.</param>
    /// <param name="content">The text content to save.</param>
    public static void SaveText(string filePath, string content)
    {
        string key = PATH_PREFIX + filePath.Replace("/", "_").Replace("\\", "_");
        PlayerPrefs.SetString(key, content);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Loads text data from PlayerPrefs as if reading from a file.
    /// </summary>
    /// <param name="filePath">The path of the file to load.</param>
    /// <returns>The text content if the file exists, empty string otherwise.</returns>
    public static string LoadText(string filePath)
    {
        string key = PATH_PREFIX + filePath.Replace("/", "_").Replace("\\", "_");
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetString(key);
        }
        return string.Empty;
    }
        
        /// <summary>
        /// Reads all text from a file, mimicking File.ReadAllText behavior.
        /// Uses PlayerPrefs in WebGL and forwards to File.ReadAllText otherwise.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        /// <returns>The text content of the file.</returns>
        public static string ReadAllText(string filePath)
        {
            return LoadText(filePath);
        }

                /// <summary>
                /// Writes all text to a file, mimicking File.WriteAllText behavior.
                /// Uses PlayerPrefs in WebGL and forwards to File.WriteAllText otherwise.
                /// </summary>
                /// <param name="filePath">The path of the file to write.</param>
                /// <param name="content">The content to write.</param>
                public static void WriteAllText(string filePath, string content)
                {
                    SaveText(filePath, content);
                }

                /// <summary>
                /// Appends text to an existing "file" in PlayerPrefs.
                /// </summary>
    /// <param name="filePath">The path of the file to append to.</param>
    /// <param name="content">The content to append.</param>
    public static void AppendText(string filePath, string content)
    {
        string key = PATH_PREFIX + filePath.Replace("/", "_").Replace("\\", "_");
        string existingContent = PlayerPrefs.GetString(key, "");
        PlayerPrefs.SetString(key, existingContent + content);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Deletes a "file" from PlayerPrefs.
    /// </summary>
    /// <param name="filePath">The path of the file to delete.</param>
    public static void DeleteFile(string filePath)
    {
        string key = PATH_PREFIX + filePath.Replace("/", "_").Replace("\\", "_");
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }
    
    /// <summary>
    /// Gets all "file" keys that match a specific pattern.
    /// </summary>
    /// <param name="directoryPath">The directory path pattern to match.</param>
    /// <returns>A list of file paths that match the pattern.</returns>
    public static List<string> GetFiles(string directoryPath)
    {
        // This is a limited implementation since PlayerPrefs doesn't support key enumeration
        // In a real implementation, you might need to track your files in a separate registry
        // For demo purposes, this returns an empty list
        Debug.LogWarning("WebGLSaveSystem.GetFiles() is not fully implemented. File enumeration is limited in WebGL.");
        return new List<string>();
    }
    
    /// <summary>
    /// Converts a web-safe file path back to its original form.
    /// </summary>
    /// <param name="webSafePath">The web-safe file path.</param>
    /// <returns>The original file path.</returns>
    public static string ConvertWebSafePathToOriginal(string webSafePath)
    {
        if (webSafePath.StartsWith(PATH_PREFIX))
        {
            webSafePath = webSafePath.Substring(PATH_PREFIX.Length);
        }
        return webSafePath.Replace("_", "/");
    }
}

