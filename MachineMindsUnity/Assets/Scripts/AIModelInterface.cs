using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using System.Collections;
using System.Threading;

public class AIModelInterface : MonoBehaviour
{
    [Header("Model Parameters")]
    public int currentDifficulty = 1;
    public int currentPlayerLives = 3;
    public int levelsBeat = 0;
    public float playerLifeTimer = 0;
    public int totalEnemiesKilled = 0;
    public float totalPoints = 0;
    public int predictedDifficulty = -101;

    [System.Serializable]
    private class ModelRequest
    {
        public int currentDifficulty;
        public int currentPlayerLives;
        public int levelsBeat;
        public float playerLifeTimer;
        public int totalEnemiesKilled;
        public float totalPoints;
    }

    [System.Serializable]
    private class ModelResponse
    {
        public int prediction;
    }

    // Legacy method for backwards compatibility
    public int GetPredictedDifficulty()
    {
#if UNITY_WEBGL
        // For WebGL and Editor, use synchronous web request (not recommended but kept for compatibility)
        string jsonString = JsonUtility.ToJson(new ModelRequest
        {
            currentDifficulty = this.currentDifficulty,
            currentPlayerLives = this.currentPlayerLives,
            levelsBeat = this.levelsBeat,
            playerLifeTimer = this.playerLifeTimer,
            totalEnemiesKilled = this.totalEnemiesKilled,
            totalPoints = this.totalPoints
        });

        using (UnityWebRequest request = new UnityWebRequest("https://machinemindsbackend.onrender.com/predict", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            request.SendWebRequest();

            // Wait for completion (synchronously)
            while (!request.isDone) { }

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<ModelResponse>(request.downloadHandler.text);
                predictedDifficulty = response.prediction;
            }
            else
            {
                UnityEngine.Debug.LogError($"Request failed: {request.error}");
                predictedDifficulty = -101;
            }
        }
#else
        // Original Python implementation for standalone builds
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = GetPythonPath(),
            Arguments = $"-W ignore \"{GetScriptPath()}\" {currentDifficulty} {currentPlayerLives} {levelsBeat} {playerLifeTimer} {totalEnemiesKilled} {totalPoints}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (string.IsNullOrEmpty(error))
            {
                if (int.TryParse(output.Trim(), out int result))
                {
                    predictedDifficulty = result;
                    UnityEngine.Debug.Log($"Predicted difficulty change: {predictedDifficulty}");
                }
                else
                {
                    UnityEngine.Debug.LogError($"Failed to parse model output: {output}");
                    predictedDifficulty = -101;
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"Python error: {error}");
                predictedDifficulty = -101;
            }
        }
#endif
        return predictedDifficulty;
    }

    // Modern async method (recommended for new implementations)
    public System.Collections.IEnumerator GetPredictedDifficultyCoroutine()
    {
#if UNITY_WEBGL || UNITY_EDITOR
        var requestData = new ModelRequest
        {
            currentDifficulty = this.currentDifficulty,
            currentPlayerLives = this.currentPlayerLives,
            levelsBeat = this.levelsBeat,
            playerLifeTimer = this.playerLifeTimer,
            totalEnemiesKilled = this.totalEnemiesKilled,
            totalPoints = this.totalPoints
        };

        string jsonData = JsonUtility.ToJson(requestData, true);
        UnityEngine.Debug.Log($"Sending request with data: {jsonData}");

        using (UnityWebRequest request = new UnityWebRequest("https://machinemindsbackend.onrender.com/predict", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<ModelResponse>(request.downloadHandler.text);
                predictedDifficulty = response.prediction;
            }
            else
            {
                UnityEngine.Debug.LogError($"Request failed: {request.error}");
                predictedDifficulty = -101;
            }
        }
#else
        // Run Python process asynchronously
        yield return new WaitForEndOfFrame();

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = GetPythonPath(),
            Arguments = $"-W ignore \"{GetScriptPath()}\" {currentDifficulty} {currentPlayerLives} {levelsBeat} {playerLifeTimer} {totalEnemiesKilled} {totalPoints}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (string.IsNullOrEmpty(error) && int.TryParse(output.Trim(), out int result))
            {
                predictedDifficulty = result;
            }
            else
            {
                UnityEngine.Debug.LogError($"Python error: {error}");
                predictedDifficulty = -101;
            }
        }
#endif
    }
    public IEnumerator GetPredictedDifficultyCoroutine(System.Action<int> callback)
    {
        yield return GetPredictedDifficultyCoroutine(); // reuse existing coroutine

        if (callback != null)
        {
            callback(predictedDifficulty);
        }
    }

    // Helper method for modern async usage
    public void GetPredictedDifficultyAsync(System.Action<int> callback)
    {
        StartCoroutine(GetPredictedDifficultyCoroutine(callback));
    }

    private string GetPythonPath()
    {
#if UNITY_EDITOR
#if UNITY_EDITOR_WIN
        UnityEngine.Debug.Log("Setting python path on Windows");
        return Path.Combine(Directory.GetCurrentDirectory(), "Assets\\StreamingAssets\\AI\\in_game_env\\Scripts\\python.exe");
#elif UNITY_EDITOR_OSX
        UnityEngine.Debug.Log("Setting python path on OSX");
        return Path.Combine(Directory.GetCurrentDirectory(), "Assets/StreamingAssets/AI/in_game_env/bin/python");
#else
        UnityEngine.Debug.Log("Setting python path on other platform");
        return Path.Combine(Directory.GetCurrentDirectory(), "Assets\\StreamingAssets\\AI\\in_game_env\\Scripts\\python.exe");
#endif
#else
#if UNITY_STANDALONE_WIN 
        UnityEngine.Debug.Log("Setting python path on Windows");
        UnityEngine.Debug.Log("Path: [" + Path.Combine(Application.streamingAssetsPath, "AI\\in_game_env\\Scripts\\python.exe") + "]");
        return Path.Combine(Application.streamingAssetsPath, "AI\\in_game_env\\Scripts\\python.exe");
#elif UNITY_STANDALONE_OSX
        UnityEngine.Debug.Log("Setting python path on OSX");
        return Path.Combine(Application.streamingAssetsPath, "AI/in_game_env/bin/python");
#elif UNITY_STANDALONE_WEBGL
        UnityEngine.Debug.Log("ERROR, Python path not supported on WebGL yet, must boot up a back end for it");
        return "";
#else
        UnityEngine.Debug.Log("Setting python path on other platform");
        //UnityEngine.Debug.Log(Path.Combine(Application.streamingAssetsPath, "AI\\in_game_env\\Scripts\\python"));
        return Path.Combine(Application.streamingAssetsPath, "AI/in_game_env/bin/python"); ;
#endif
#endif
    }

    private string GetScriptPath()
    {
#if UNITY_EDITOR_WIN
    return Path.Combine(Directory.GetCurrentDirectory(), "Assets\\StreamingAssets\\AI\\run_model.py");
#elif UNITY_EDITOR_OSX
    return Path.Combine(Directory.GetCurrentDirectory(), "Assets/StreamingAssets/AI/run_model.py");
#elif UNITY_STANDALONE_WIN
    UnityEngine.Debug.Log("Path 2: [" + Path.Combine(Application.streamingAssetsPath, "AI\\run_model.py") + "]");
    return Path.Combine(Application.streamingAssetsPath, "AI\\run_model.py");
#elif UNITY_STANDALONE_OSX
    return Path.Combine(Application.streamingAssetsPath, "AI/run_model.py");
#elif UNITY_STANDALONE_WEBGL
    UnityEngine.Debug.Log("ERROR, Python not supported on WebGL yet, must boot up a back end for it");
    return "";
#else
        return Path.Combine(Application.streamingAssetsPath, "AI/run_model.py");
#endif
    }
}
