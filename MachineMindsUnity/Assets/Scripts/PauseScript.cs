using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides pause menu functionality for difficulty feedback and progression.
/// </summary>
public class PauseScript : MonoBehaviour
{
    private string saveFilePath = "GameState";
    private string aiTrainingFilePath = "GameData";
    private string[] fileData;
    private int currentDifficulty = 1;
    private int nextLevel = 1;
    private const int maxDifficulty = 10;

    public TMPro.TextMeshProUGUI contextMessage;

    /// <summary>
    /// Reads and returns lines of saved data from PlayerPrefs for the given key.
    /// </summary>
    private string[] getFileData(string filePath)
    {
        if (PlayerPrefs.HasKey(filePath))
            return PlayerPrefs.GetString(filePath).Split('\n');
        return new string[0];
    }

    /// <summary>
    /// Writes or appends an array of lines to a PlayerPrefs key.
    /// </summary>
    private void writeFileData(string filePath, string[] newFileData, bool overwriteExistingFileData)
    {
        string content = overwriteExistingFileData ? "" : PlayerPrefs.GetString(filePath) + (PlayerPrefs.GetString(filePath).EndsWith("\n") ? "" : "\n");
        content += string.Join("\n", newFileData);
        PlayerPrefs.SetString(filePath, content);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Saves AI training data with a capped difficulty adjustment.
    /// </summary>
    private void saveAITrainingData(int newDifficulty)
    {
        // Ensures difficulty remains within valid range and logs training data
        newDifficulty = Mathf.Clamp(newDifficulty, 1, maxDifficulty);
        if (!PlayerPrefs.HasKey(aiTrainingFilePath))
        {
            writeFileData(aiTrainingFilePath, new string[]{
                "currentPlayerLives,totalPoints,totalEnemiesKilled,currentDifficulty,playerLifeTimer,levelsBeat,newDifficulty",
                string.Join(",", fileData) + "," + newDifficulty
            }, true);
        }
        else
        {
            writeFileData(aiTrainingFilePath, new string[]{ string.Join(",", fileData) + "," + newDifficulty }, false);
        }
    }

    /// <summary>
    /// Updates saved game difficulty by writing new difficulty into the save file.
    /// </summary>
    private void adjustDifficulty(int newDifficulty)
    {
        string[] currentFileData = getFileData(saveFilePath);
        if (currentFileData.Length < 4) return; // Not enough data
        newDifficulty = Mathf.Clamp(newDifficulty, 1, maxDifficulty);
        writeFileData(saveFilePath, new string[]{
            currentFileData[0],
            currentFileData[1],
            currentFileData[2],
            newDifficulty.ToString(),
            currentFileData[3],
            "true"
        }, true);
    }

    /// <summary>
    /// Sets the index of the next level to load after feedback.
    /// </summary>
    public void SetNextLevel(int newNextLevel)
    {
        nextLevel = newNextLevel;
    }

    /// <summary>
    /// Initializes internal file data and current difficulty level.
    /// </summary>
    public void SetFileData(string[] newFileData)
    {
        fileData = newFileData;
        currentDifficulty = int.Parse(fileData[3]);
    }

    /// <summary>
    /// Called when player indicates the difficulty was too easy.
    /// Saves AI data, adjusts game difficulty upward, and loads the next level.
    /// </summary>
    public void TooEasyAdjustment()
    {
        saveAITrainingData(currentDifficulty + 1);
        adjustDifficulty(currentDifficulty + 1);
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
        Destroy(gameObject);
    }

    /// <summary>
    /// Called when player indicates the difficulty was just right.
    /// Saves AI data without adjusting difficulty and proceeds.
    /// </summary>
    public void JustRightAdjustment()
    {
        saveAITrainingData(currentDifficulty);
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
        Destroy(gameObject);
    }

    /// <summary>
    /// Called when player indicates the difficulty was too hard.
    /// Saves AI data, adjusts game difficulty downward, and loads the next level.
    /// </summary>
    public void TooHardAdjustment()
    {
        saveAITrainingData(currentDifficulty - 1);
        adjustDifficulty(currentDifficulty - 1);
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
        Destroy(gameObject);
    }

    /// <summary>
    /// Unity Start: updates the displayed context message with current difficulty.
    /// </summary>
    void Start()
    {
        contextMessage.text = "Current Difficulty Level: " + currentDifficulty;
    }
}
