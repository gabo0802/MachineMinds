using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Provides utility methods for handling save/load operations on the WebGL platform.
/// Uses PlayerPrefs as an alternative to file I/O operations.
/// </summary>
public static class SaveSystem
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
        string keysIndex = PlayerPrefs.GetString("SaveSystem_KeysIndex", "");
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
        string keysIndex = PlayerPrefs.GetString("SaveSystem_KeysIndex", "");
        List<string> keys = new List<string>(keysIndex.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries));

        if (!keys.Contains(key))
        {
            keys.Add(key);
            PlayerPrefs.SetString("SaveSystem_KeysIndex", string.Join("|", keys));
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Removes a key from the tracking index.
    /// </summary>
    /// <param name="key">The key to remove from the tracking index.</param>
    private static void RemoveKeyFromIndex(string key)
    {
        string keysIndex = PlayerPrefs.GetString("SaveSystem_KeysIndex", "");
        List<string> keys = new List<string>(keysIndex.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries));

        if (keys.Contains(key))
        {
            keys.Remove(key);
            PlayerPrefs.SetString("SaveSystem_KeysIndex", string.Join("|", keys));
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Saves the current game state to PlayerPrefs storage.
    /// </summary>
    /// <param name="lives">The number of player lives remaining.</param>
    /// <param name="points">The total points accumulated by the player.</param>
    /// <param name="enemiesKilled">The total number of enemies killed.</param>
    /// <param name="difficulty">The current game difficulty level.</param>
    /// <param name="lifeTimer">The elapsed time for the current player life.</param>
    /// <param name="trainingMode">Whether the game is in training mode.</param>
    public static void SaveGameState(int lives, float points, int enemiesKilled, int difficulty, float lifeTimer, bool trainingMode)
    {
        string content = $"{lives}\n{points}\n{enemiesKilled}\n{difficulty}\n{lifeTimer}\n{trainingMode}";
        WriteAllText("GameState", content);
    }

    /// <summary>
    /// Loads the game state from PlayerPrefs storage.
    /// </summary>
    /// <returns>A dictionary containing the game state parameters, or null if no saved state exists.</returns>
    /// <remarks>
    /// The dictionary contains the following keys:
    /// - "lives": The number of player lives (int)
    /// - "points": The total points accumulated (float)
    /// - "enemiesKilled": The total number of enemies killed (int)
    /// - "difficulty": The current game difficulty level (int)
    /// - "lifeTimer": The elapsed time for the current player life (float)
    /// - "trainingMode": Whether the game is in training mode (bool)
    /// </remarks>
    public static Dictionary<string, object> LoadGameState()
    {
        if (!FileExists("GameState"))
            return null;

        string[] data = ReadAllText("GameState").Split('\n');
        Dictionary<string, object> gameState = new Dictionary<string, object>();

        if (data.Length >= 6)
        {
            gameState["lives"] = int.Parse(data[0]);
            gameState["points"] = float.Parse(data[1]);
            gameState["enemiesKilled"] = int.Parse(data[2]);
            gameState["difficulty"] = int.Parse(data[3]);
            gameState["lifeTimer"] = float.Parse(data[4]);
            gameState["trainingMode"] = bool.Parse(data[5]);
        }

        return gameState;
    }

    /// <summary>
    /// Saves AI training data to PlayerPrefs storage.
    /// </summary>
    /// <param name="fileData">Array containing game state parameters in the following order:
    /// [0] currentPlayerLives, [1] totalPoints, [2] totalEnemiesKilled, 
    /// [3] currentDifficulty, [4] playerLifeTimer, [5] levelsBeat.</param>
    /// <param name="newDifficulty">The new difficulty level determined by the AI.</param>
    /// <remarks>
    /// If no training data exists, creates a new file with a header row.
    /// If training data exists, appends the new data as a new line.
    /// </remarks>
    public static void SaveTrainingData(string[] fileData, int newDifficulty)
    {
        if (!FileExists("GameData"))
        {
            WriteAllText("GameData", "currentPlayerLives,totalPoints,totalEnemiesKilled,currentDifficulty,playerLifeTimer,levelsBeat,newDifficulty\n" +
                $"{fileData[0]},{fileData[1]},{fileData[2]},{fileData[3]},{fileData[4]},{fileData[5]},{newDifficulty}");
        }
        else
        {
            string existingData = ReadAllText("GameData");
            string newLine = $"{fileData[0]},{fileData[1]},{fileData[2]},{fileData[3]},{fileData[4]},{fileData[5]},{newDifficulty}";
            WriteAllText("GameData", existingData + "\n" + newLine);
        }
    }

}
