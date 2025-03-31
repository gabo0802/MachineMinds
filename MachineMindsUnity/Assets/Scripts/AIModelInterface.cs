using System.Diagnostics;
using System.IO;
using UnityEngine;

public class AIModelInterface : MonoBehaviour
{
    [Header("Model Parameters")]
    public int currentDifficulty = 1;
    public int currentPlayerLives = 3;
    public int levelsBeat = 0;
    public float playerLifeTimer = 0;
    public int totalEnemiesKilled = 0;
    public float totalPoints = 0;
    private int predictedDifficulty = -101;

    // Call this method whenever you need to get a prediction
    public int GetPredictedDifficulty()
    {
        RunPythonModel();
        return predictedDifficulty;
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
        return Path.Combine(Directory.GetCurrentDirectory(), "Assets\\StreamingAssets\\AI\\in_game_env\\Scripts\\python.exe");
#elif UNITY_STANDALONE_OSX
        UnityEngine.Debug.Log("Setting python path on OSX");
        return Path.Combine(Application.streamingAssetsPath, "AI/in_game_env/bin/python");
#else
        UnityEngine.Debug.Log("Setting python path on other platform");
        return Path.Combine(Directory.GetCurrentDirectory(), "Assets\\StreamingAssets\\AI\\in_game_env\\Scripts\\python");
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
    return Path.Combine(Directory.GetCurrentDirectory(), "Assets\\StreamingAssets\\AI\\run_model.py");
#elif UNITY_STANDALONE_OSX
    return Path.Combine(Application.streamingAssetsPath, "AI/run_model.py");
#else
        return Path.Combine(Directory.GetCurrentDirectory(), "Assets/StreamingAssets/AI/run_model.py");
#endif
    }



    private void RunPythonModel()
    {
        // Create process
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = GetPythonPath(),
            Arguments = $"-W ignore {GetScriptPath()} {currentDifficulty} {currentPlayerLives} {levelsBeat} {playerLifeTimer} {totalEnemiesKilled} {totalPoints}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        UnityEngine.Debug.Log(currentDifficulty + " " + currentPlayerLives + " " + levelsBeat + " " + playerLifeTimer + " " + totalEnemiesKilled + " " + totalPoints);

        // Execute process and get output
        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (string.IsNullOrEmpty(error))
            {
                // Parse the output to get the predicted difficulty
                if (int.TryParse(output.Trim(), out int result))
                {
                    predictedDifficulty = result;
                    UnityEngine.Debug.Log($"Predicted difficulty change: {predictedDifficulty}");
                }
                else
                {
                    UnityEngine.Debug.LogError($"Failed to parse model output: {output}");
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"Python error: {error}");
            }
        }
    }
}
