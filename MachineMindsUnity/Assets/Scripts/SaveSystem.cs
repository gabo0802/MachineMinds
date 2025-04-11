using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

/// <summary>
/// Provides utility methods for handling save/load operations across platforms.
/// Uses PlayerPrefs for local storage and Firebase for WebGL.
/// </summary>
public static class SaveSystem
{

    // Collection paths for Firebase
    private const string SAVE_COLLECTION = "player-saves";
    private const string TRAINING_COLLECTION = "ai-training-data";
    private const string LEADERBOARDS_COLLECTION = "leaderboards";

    [DllImport("__Internal")]
    private static extern void GetCollection(string collectionPath, string objectName, string callback, string fallback);

    [DllImport("__Internal")]
    private static extern void AddDocument(string collectionPath, string value, string objectName, string callback, string fallback);

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
        string keysIndex = PlayerPrefs.GetString("SaveSystem_KeysIndex", "");
        List<string> allKeys = new List<string>(keysIndex.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries));

        // Filter keys based on the directory path pattern
        List<string> matchingKeys = new List<string>();
        foreach (string key in allKeys)
        {
            if (key.StartsWith(directoryPath))
            {
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
    /// Saves the current game state to storage.
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

        if (IsWebGLPlatform())
        {
            // Create a structured JSON object for Firebase
            string json = $"{{\"lives\":{lives},\"points\":{points},\"enemiesKilled\":{enemiesKilled},\"difficulty\":{difficulty},\"lifeTimer\":{lifeTimer},\"trainingMode\":{trainingMode.ToString().ToLower()}}}";
            SaveSystemHelper.Instance.SendGameStateToFirebase(json);
        }
    }

    /// <summary>
    /// Loads the game state from storage.
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
    /// Saves AI training data to storage.
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

        if (IsWebGLPlatform())
        {
            // Create a structured JSON object for Firebase
            string json = $"{{\"currentPlayerLives\":{fileData[0]},\"totalPoints\":{fileData[1]},\"totalEnemiesKilled\":{fileData[2]},\"currentDifficulty\":{fileData[3]},\"playerLifeTimer\":{fileData[4]},\"levelsBeat\":{fileData[5]},\"newDifficulty\":{newDifficulty}}}";
            SaveSystemHelper.Instance.SendTrainingDataToFirebase(json);
        }
    }

    /// <summary>
    /// Sends data to Firebase if running on WebGL.
    /// </summary>
    /// <param name="key">The key/path for the data.</param>
    /// <param name="data">The data to send.</param>
    private static void SendDataToFirebase(string key, string data)
    {
        if (IsWebGLPlatform())
        {
            // Convert data format based on the key
            if (key == "GameState")
            {
                string[] lines = data.Split('\n');
                if (lines.Length >= 6)
                {
                    string json = $"{{\"lives\":{lines[0]},\"points\":{lines[1]},\"enemiesKilled\":{lines[2]},\"difficulty\":{lines[3]},\"lifeTimer\":{lines[4]},\"trainingMode\":{lines[5].ToLower()}}}";
                    SaveSystemHelper.Instance.SendGameStateToFirebase(json);
                }
            }
            else if (key == "GameData")
            {
                // For training data, we'll send the last line only (most recent)
                string[] lines = data.Split('\n');
                if (lines.Length > 1) // Skip header
                {
                    string lastLine = lines[lines.Length - 1];
                    string[] values = lastLine.Split(',');
                    if (values.Length >= 7)
                    {
                        string json = $"{{\"currentPlayerLives\":{values[0]},\"totalPoints\":{values[1]},\"totalEnemiesKilled\":{values[2]},\"currentDifficulty\":{values[3]},\"playerLifeTimer\":{values[4]},\"levelsBeat\":{values[5]},\"newDifficulty\":{values[6]}}}";
                        SaveSystemHelper.Instance.SendTrainingDataToFirebase(json);
                    }
                }
            }
        }
    }

    public static void SaveToLeaderboards(float score)
    {
        string json = $"{{\"score\":{score},\"date\":\"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}\"}}";

        if (IsWebGLPlatform())
        {
            SaveSystemHelper.Instance.SendLeaderboardScore(json);
        }
        else
        {
            // For desktop builds, use REST API
            CoroutineHelper.StartCoroutineStatic(SaveLeaderboardScoreREST(json));
        }
    }

    private static IEnumerator SaveLeaderboardScoreREST(string json)
    {
        string projectId = "machinemindsunity";
        string url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/{LEADERBOARDS_COLLECTION}";

        // Deserialize the raw score/date JSON first
        var data = JsonUtility.FromJson<RawLeaderboardData>(json);

        // Now build the proper Firestore format
        string firestoreJson = $@"
    {{
        ""fields"": {{
            ""score"": {{ ""doubleValue"": {data.score} }},
            ""date"": {{ ""stringValue"": ""{data.date}"" }}
        }}
    }}";

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(firestoreJson);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to save leaderboard score: {www.error}\n{www.downloadHandler.text}");
            }
            else
            {
                Debug.Log("Score posted successfully.");
            }
        }
    }

    [Serializable]
    private class RawLeaderboardData
    {
        public float score;
        public string date;
    }


    public static IEnumerator GetLeaderboards(System.Action<List<LeaderboardEntry>> callback)
    {
        if (IsWebGLPlatform())
        {
            // Use existing JavaScript bridge
            GetCollection(LEADERBOARDS_COLLECTION, SaveSystemHelper.Instance.gameObject.name, "OnLeaderboardsLoaded", "OnLeaderboardsError");
            yield return new WaitUntil(() => SaveSystemHelper.Instance.leaderboardsLoaded);
            callback(SaveSystemHelper.Instance.leaderboardEntries);
        }
        else
        {
            // Use REST API for desktop
            yield return GetLeaderboardsREST(callback);
        }
    }

    private static IEnumerator GetLeaderboardsREST(System.Action<List<LeaderboardEntry>> callback)
    {
        string projectId = "machinemindsunity";
        string url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents:runQuery";

        string jsonBody = @"
    {
        ""structuredQuery"": {
            ""from"": [{ ""collectionId"": ""leaderboards"" }],
            ""orderBy"": [{
                ""field"": { ""fieldPath"": ""score"" },
                ""direction"": ""DESCENDING""
            }],
            ""limit"": 10
        }
    }";

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var entries = ParseLeaderboardResponse(www.downloadHandler.text);
                callback(entries);
                Debug.Log("Leaderboards loaded successfully");
            }
            else
            {
                Debug.LogError($"Failed to get leaderboards: {www.error}");
                callback(new List<LeaderboardEntry>());
            }
        }
    }


    private static List<LeaderboardEntry> ParseLeaderboardResponse(string jsonResponse)
    {
        var entries = new List<LeaderboardEntry>();
        var jsonArray = SimpleJSON.JSON.Parse(jsonResponse).AsArray;

        int rank = 1; // Start from 1

        foreach (var item in jsonArray)
        {
            var document = item.Value["document"];
            var fields = document["fields"];

            float score = 0;
            string date = "";

            if (fields["score"] != null)
            {
                if (fields["score"]["doubleValue"] != null)
                    score = fields["score"]["doubleValue"].AsFloat;
                else if (fields["score"]["integerValue"] != null)
                    score = fields["score"]["integerValue"].AsFloat;
            }

            if (fields["date"] != null)
            {
                date = fields["date"]["stringValue"];
            }

            entries.Add(new LeaderboardEntry
            {
                rank = rank++,
                score = score,
                date = date
            });
        }
        Debug.Log(entries.Count);
        return entries;
    }

}
