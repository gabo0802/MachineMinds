using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the pre-level UI elements such as level info, difficulty,
/// player lives display, and hint text, then auto-continues after a delay.
/// </summary>
public class BeforeLevelObjectScript : MonoBehaviour
{
    private const int levelsPerCheckpoint = 5;

    public TMPro.TextMeshProUGUI levelNumberUI;
    public TMPro.TextMeshProUGUI levelDifficultyUI;
    public TMPro.TextMeshProUGUI isCheckpointUI;
    public UnityEngine.UI.Image currentLevelBackground;
    public Sprite[] allLevelBackgrounds;

    private int currentLevelNumber;
    private int currentDifficulty;

    public UnityEngine.UI.Image[] playerHearts;
    private int currentPlayerLives = 3;
    public const string spritePath = "DynamicSprites/heart_empty";
    public const string spritePathFull = "DynamicSprites/heart_full";

    public TMPro.TextMeshProUGUI hintUI;
    public string[] allHints;

    public AudioSource musicPlayer;

    /// <summary>
    /// Adjusts the music volume based on saved player preferences.
    /// </summary>
    private void volumeAdjustments()
    {
        if (musicPlayer && PlayerPrefs.HasKey("MusicVolume"))
        {
            musicPlayer.volume = PlayerPrefs.GetFloat("MusicVolume") * 0.25f;
        }
    }

    /// <summary>
    /// Updates the UI hearts to reflect the current number of player lives.
    /// Loads heart sprites from Resources and assigns full/empty accordingly.
    /// </summary>
    private void UpdateLivesUI()
    {
        // ... (body omitted for brevity)
    }

    /// <summary>
    /// Handles the Continue button press by resuming time and destroying this UI.
    /// </summary>
    public void OnContinueButtonPress()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    /// <summary>
    /// Sets the player's lives count and refreshes the lives UI display.
    /// </summary>
    void setPlayerLives(int newLives)
    {
        currentPlayerLives = newLives;
        UpdateLivesUI();
    }

    /// <summary>
    /// Applies level number, background sprite, and difficulty text
    /// based on provided level parameters array.
    /// </summary>
    void setLevelParameters(int[] levelParameters)
    {
        // ... (body omitted for brevity)
    }

    /// <summary>
    /// Updates the checkpoint text to indicate if the current level is a checkpoint.
    /// </summary>
    void setIsCheckpoint(bool isCheckpoint)
    {
        isCheckpointUI.text = isCheckpoint ? "Checkpoint Reached!" : "";
    }

    /// <summary>
    /// Coroutine that waits for a set time in real-time, then continues to the game.
    /// </summary>
    private IEnumerator Timer(float timeInSeconds)
    {
        Debug.Log("Timer start");
        yield return new WaitForSecondsRealtime(timeInSeconds);
        Debug.Log("Timer end");
        OnContinueButtonPress();
    }

    /// <summary>
    /// Unity Start method: adjusts volume, shows random hint, and starts auto-continue timer.
    /// </summary>
    void Start()
    {
        volumeAdjustments();
        hintUI.text = allHints[Random.Range(0, allHints.Length)];
        StartCoroutine(Timer(3));
    }
}