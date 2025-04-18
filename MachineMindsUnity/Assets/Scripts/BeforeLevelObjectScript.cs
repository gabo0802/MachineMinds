using UnityEngine;
using System.Collections;

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
    public const string spritePath = "DynamicSprites/heart_empty"; // Path inside Resources folder
    public const string spritePathFull = "DynamicSprites/heart_full"; // Path inside Resources folder

    public TMPro.TextMeshProUGUI hintUI;
    public string[] allHints;

    public AudioSource musicPlayer;

    private void volumeAdjustments(){
        if (musicPlayer && PlayerPrefs.HasKey("MusicVolume")){
            musicPlayer.volume = PlayerPrefs.GetFloat("MusicVolume") * 0.25f;
        }
    }
    private void UpdateLivesUI()
    {
        // Load New Sprites
        Texture2D emptyTexture = Resources.Load<Texture2D>(spritePath);
        Texture2D fullTexture = Resources.Load<Texture2D>(spritePathFull);

        // Check if textures loaded properly
        if (emptyTexture == null || fullTexture == null)
        {
            Debug.LogError($"Failed to load textures! Check paths: {spritePath}, {spritePathFull}");
            return;
        }

        Sprite emptyHeart = Sprite.Create(emptyTexture, new Rect(0, 0, emptyTexture.width, emptyTexture.height), new Vector2(0.5f, 0.5f));
        Sprite fullHeart = Sprite.Create(fullTexture, new Rect(0, 0, fullTexture.width, fullTexture.height), new Vector2(0.5f, 0.5f));
        if (emptyHeart == null || fullHeart == null)
        {
            Debug.LogError($"Failed to load sprites! Check paths: {spritePath}, {spritePathFull}");
            return;
        }
        else
        {
            Debug.Log("Successfully loaded heart sprites!");
        }


        for (int i = 0; i < currentPlayerLives; i++)
        {
            playerHearts[i].sprite = fullHeart;
        }

        for (int i = currentPlayerLives; i < playerHearts.Length; i++)
        {
            playerHearts[i].sprite = emptyHeart;
        }
    }

    public void OnContinueButtonPress()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    void setPlayerLives(int newLives)
    {
        currentPlayerLives = newLives;
        UpdateLivesUI();
    }

    void setLevelParameters(int[] levelParameters)
    {
        currentLevelNumber = levelParameters[0];

        if (currentLevelNumber == 10)
        {
            levelNumberUI.text = "Level #10 - Boss Battle";
            currentLevelBackground.sprite = allLevelBackgrounds[4];
        }
        else if (currentLevelNumber == 20)
        {
            levelNumberUI.text = "Level #20 - Boss Battle";
            currentLevelBackground.sprite = allLevelBackgrounds[5];
        }
        else
        {
            levelNumberUI.text = "Level #" + currentLevelNumber;
            currentLevelBackground.sprite = allLevelBackgrounds[currentLevelNumber == 0 ? 0 : ((currentLevelNumber - 1) / levelsPerCheckpoint)];
        }

        currentDifficulty = levelParameters[1];

        bool isEasyMode = PlayerPrefs.GetInt("CheckpointLevelDeaths") > 5;
        levelDifficultyUI.text = isEasyMode ? "[Super Easy Mode]" : currentDifficulty == 11 ? "[Max Difficulty]": "Current Difficulty: " + currentDifficulty;
    }

    void setIsCheckpoint(bool isCheckpoint)
    {
        isCheckpointUI.text = isCheckpoint ? "Checkpoint Reached!" : "";
    }

    private IEnumerator Timer(float timeInSeconds)
    {
        Debug.Log("Timer start");
        yield return new WaitForSecondsRealtime(timeInSeconds);
        Debug.Log("Timer end");
        OnContinueButtonPress();
    }

    void Start()
    {   
        volumeAdjustments();
        hintUI.text = allHints[(int)Random.Range(0, allHints.Length)];
        StartCoroutine(Timer(3));
    }
}