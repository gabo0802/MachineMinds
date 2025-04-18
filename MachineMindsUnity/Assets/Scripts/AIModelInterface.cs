using System.Diagnostics;
using System.IO;
using UnityEngine;

/// <summary>
/// AIModelInterface communicates with an external Python AI model
/// to predict game difficulty adjustments based on runtime player stats.
/// </summary>
public class AIModelInterface : MonoBehaviour
{
    [Header("Model Parameters")]
    public int currentDifficulty = 1;
    public int currentPlayerLives = 3;
    public int levelsBeat = 0;
    public float playerLifeTimer = 0f;
    public int totalEnemiesKilled = 0;
    public float totalPoints = 0f;
    private int predictedDifficulty = -101;

    /// <summary>
    /// Invokes the Python model and returns the predicted difficulty.
    /// </summary>
    public int GetPredictedDifficulty()
    {
        RunPythonModel();
        return predictedDifficulty;
    }

    /// <summary>
    /// Determines and returns the appropriate Python executable path
    /// based on the current platform and build settings.
    /// </summary>
    private string GetPythonPath()
    {
#if UNITY_EDITOR
#if UNITY_EDITOR_WIN
        return Path.Combine(Directory.GetCurrentDirectory(), "Assets\\StreamingAssets\\AI\\in_game_env\\Scripts\\python.exe");
#elif UNITY_EDITOR_OSX
        return Path.Combine(Directory.GetCurrentDirectory(), "Assets/StreamingAssets/AI/in_game_env/bin/python");
#else
        return Path.Combine(Directory.GetCurrentDirectory(), "Assets\\StreamingAssets\\AI\\in_game_env\\Scripts\\python.exe");
#endif
#else
#if UNITY_STANDALONE_WIN
        return Path.Combine(Application.streamingAssetsPath, "AI\\in_game_env\\Scripts\\python.exe");
#elif UNITY_STANDALONE_OSX
        return Path.Combine(Application.streamingAssetsPath, "AI/in_game_env/bin/python");
#elif UNITY_STANDALONE_WEBGL
        return string.Empty;
#else
        return Path.Combine(Application.streamingAssetsPath, "AI/in_game_env/bin/python");
#endif
#endif
    }

    /// <summary>
    /// Determines and returns the path to the Python script (run_model.py)
    /// within StreamingAssets or project directory.
    /// </summary>
    private string GetScriptPath()
    {
#if UNITY_EDITOR_WIN
        return Path.Combine(Directory.GetCurrentDirectory(), "Assets\\StreamingAssets\\AI\\run_model.py");
#elif UNITY_EDITOR_OSX
        return Path.Combine(Directory.GetCurrentDirectory(), "Assets/StreamingAssets/AI/run_model.py");
#elif UNITY_STANDALONE_WIN
        return Path.Combine(Application.streamingAssetsPath, "AI\\run_model.py");
#elif UNITY_STANDALONE_OSX
        return Path.Combine(Application.streamingAssetsPath, "AI/run_model.py");
#elif UNITY_STANDALONE_WEBGL
        return string.Empty;
#else
        return Path.Combine(Application.streamingAssetsPath, "AI/run_model.py");
#endif
    }

    /// <summary>
    /// Executes the external Python script as a separate process,
    /// reads its output, and updates the predictedDifficulty field.
    /// </summary>
    private void RunPythonModel()
    {
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
                }
                else
                {
                    Debug.LogError($"Failed to parse model output: {output}");
                }
            }
            else
            {
                Debug.LogError($"Python error: {error}");
            }
        }
    }
}