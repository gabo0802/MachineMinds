using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Helper MonoBehaviour class to handle Firebase operations that require coroutines.
/// </summary>
public class SaveSystemHelper : MonoBehaviour
{
    public bool leaderboardsLoaded { get; private set; }
    public List<LeaderboardEntry> leaderboardEntries { get; private set; }

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void AddDocument(string collectionPath, string value, string objectName, string callback, string fallback);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetDocument(string collectionPath, string documentId, string value, string objectName, string callback, string fallback);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern bool IsFirebaseInitialized();
#endif

    // Singleton instance
    private static SaveSystemHelper _instance;

    private const string SAVE_COLLECTION = "player-saves";
    private const string TRAINING_COLLECTION = "ai-training-data";
    private const string LEADERBOARDS_COLLECTION = "leaderboards";

    /// <summary>
    /// Gets the singleton instance of SaveSystemHelper, creating it if necessary.
    /// </summary>
    public static SaveSystemHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SaveSystemHelper");
                _instance = go.AddComponent<SaveSystemHelper>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    /// <summary>
    /// Sends game state data to Firebase.
    /// </summary>
    /// <param name="jsonData">The game state data in JSON format.</param>
    public void SendGameStateToFirebase(string jsonData)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        StartCoroutine(TryFirebaseOperation("player-saves", "current-save", jsonData));
#endif
    }

    /// <summary>
    /// Sends training data to Firebase.
    /// </summary>
    /// <param name="jsonData">The training data in JSON format.</param>
    public void SendTrainingDataToFirebase(string jsonData)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        StartCoroutine(TryFirebaseAddOperation("ai-training-data", jsonData));
#endif
    }

    public void SendLeaderboardScore(string jsonData)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        StartCoroutine(TryFirebaseAddOperation(LEADERBOARDS_COLLECTION, jsonData));
#endif
    }

    public void OnLeaderboardsLoaded(string jsonData)
    {
        leaderboardsLoaded = true;
        // Parse the JSON data and convert to LeaderboardEntry objects
        leaderboardEntries = JsonUtility.FromJson<List<LeaderboardEntry>>(jsonData);
    }

    public void OnLeaderboardsError(string error)
    {
        leaderboardsLoaded = true;
        leaderboardEntries = new List<LeaderboardEntry>();
        Debug.LogError($"Failed to load leaderboards: {error}");
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    /// <summary>
    /// Attempts to perform a Firebase set document operation with retry logic.
    /// </summary>
    /// <param name="collectionPath">The Firebase collection path.</param>
    /// <param name="documentId">The document ID to set.</param>
    /// <param name="jsonData">The JSON data to save.</param>
    /// <returns>Coroutine IEnumerator.</returns>
    private IEnumerator TryFirebaseOperation(string collectionPath, string documentId, string jsonData)
    {
        // Wait for Firebase to initialize
        float checkInterval = 0.5f;
        float maxWaitTime = 30f;
        float elapsedTime = 0f;

        Debug.Log("Attempting Firebase operation, collection: " + collectionPath + ", document: " + documentId + ", data: " + jsonData);
        
        while (elapsedTime < maxWaitTime)
        {
            if (IsFirebaseInitialized())
            {
                Debug.Log("Firebase initialized, proceeding with set operation");
                SetDocument(collectionPath, documentId, jsonData, gameObject.name, "OnSaveSuccess", "OnSaveFailed");
                yield break;
            }
            
            yield return new WaitForSeconds(checkInterval);
            elapsedTime += checkInterval;
        }
        
        Debug.LogError("Firebase initialization timed out");
    }
    
    /// <summary>
    /// Attempts to perform a Firebase add document operation with retry logic.
    /// </summary>
    /// <param name="collectionPath">The Firebase collection path.</param>
    /// <param name="jsonData">The JSON data to save.</param>
    /// <returns>Coroutine IEnumerator.</returns>
    private IEnumerator TryFirebaseAddOperation(string collectionPath, string jsonData)
    {
        // Wait for Firebase to initialize
        float checkInterval = 0.5f;
        float maxWaitTime = 30f;
        float elapsedTime = 0f;

        Debug.Log("Attempting Firebase operation, collection: " + collectionPath + ", data: " + jsonData);
        
        while (elapsedTime < maxWaitTime)
        {
                Debug.Log($"Checking Firebase initialization: {elapsedTime}/{maxWaitTime} seconds elapsed");
                // Debug.Log("Firebase initialized, proceeding with add operation");
                AddDocument(collectionPath, jsonData, gameObject.name, "OnSaveSuccess", "OnSaveFailed");
                yield break;
            
            
            yield return new WaitForSeconds(checkInterval);
            elapsedTime += checkInterval;
        }
        
        Debug.LogError("Firebase initialization timed out");
    }
#endif

    /// <summary>
    /// Callback for successful Firebase operations.
    /// </summary>
    /// <param name="result">The result message.</param>
    private void OnSaveSuccess(string result)
    {
        Debug.Log("Firebase save successful: " + result);
    }

    /// <summary>
    /// Callback for failed Firebase operations.
    /// </summary>
    /// <param name="error">The error message.</param>
    private void OnSaveFailed(string error)
    {
        Debug.LogError("Firebase save failed: " + error);
    }
}
