using UnityEngine;

public class BeforeLevelObjectScript : MonoBehaviour{
    private const int levelsPerCheckpoint = 5;

    public TMPro.TextMeshProUGUI levelNumberUI;
    public TMPro.TextMeshProUGUI isCheckpointUI;
    public UnityEngine.UI.Image currentLevelBackground;
    public Sprite[] allLevelBackgrounds;

    private int currentLevelNumber;

    public UnityEngine.UI.Image[] playerHearts;
    private int currentPlayerLives = 3;
    public const string spritePath = "DynamicSprites/heart_empty"; // Path inside Resources folder
    public const string spritePathFull = "DynamicSprites/heart_full"; // Path inside Resources folder

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

    public void OnContinueButtonPress(){
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    void setPlayerLives(int newLives){
        currentPlayerLives = newLives;
        UpdateLivesUI();
    }

    void setLevelNumber(int newLevelNumber){
        currentLevelNumber = newLevelNumber;

        if(currentLevelNumber == 10){
            levelNumberUI.text = "Level #10 - Boss Battle";
            currentLevelBackground.sprite = allLevelBackgrounds[4];
        }else if (currentLevelNumber == 20){
            levelNumberUI.text = "Level #20 - Boss Battle";
            currentLevelBackground.sprite = allLevelBackgrounds[5];
        }else{
            levelNumberUI.text = "Level #" + currentLevelNumber;
            currentLevelBackground.sprite = allLevelBackgrounds[currentLevelNumber == 0 ? 0 : ((currentLevelNumber - 1) / levelsPerCheckpoint)];
        }
    }

    void setIsCheckpoint(bool isCheckpoint){
        isCheckpointUI.text = isCheckpoint ? "[Checkpoint]" : "";
    }
}
