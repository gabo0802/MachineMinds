using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


public class LevelManager : MonoBehaviour
{
    private const float difficultyScale = 1.25f;

    public GameObject surveyObject;
    private GameObject activeSurvey;
    public GameObject checkpointMessageObject;
    
    public GameObject pauseButton;

    private string saveKey = "GameState";
    private string aiTrainingKey = "GameTrainingData";
    private bool isTrainingMode = true;

    private int numberLevelsCheckpoint = 5;

    //Current Level Variables:  
    private const string playerName = "player";
    private const string enemyName = "enemy";

    public UnityEngine.UI.Image backgroundImage;
    public TMPro.TextMeshProUGUI countdownUI;
    public TMPro.TextMeshProUGUI levelMessageUI;
    public TMPro.TextMeshProUGUI levelDifficultyUI;

    private GameObject currentAlivePlayer;

    private int currentLevelEnemyTotal = 0;

    private int currentLevelNumber;
    private bool wonLevel = false;

    public float playerCelebrateTime = 3f;
    private float currentWinTime = 0f;

    public float playerRespawnTime = 3f;
    private float currentDeadTime = 0f;

    //Difficulty AI Metrics:
    public int currentDifficulty = 1;
    private int totalEnemiesKilled = 0;
    private float totalPoints = 0f;
    private float playerLifeTimer = 0f;

    //Player Points Variables:
    public TMPro.TextMeshProUGUI pointsUI;
    private float currentPlayerPoints = 0f;
    public float difficultyMultiplier = 2f;

    //Player Shooting Variables:
    public TMPro.TextMeshProUGUI ammoUI = null;
    public int maxBulletsInMagazine = 5;
    public float bulletReloadTime = 0.1f;
    private float bulletReloadTimer = 0.1f;
    private int currentBulletsInMagazine;
    public int totalPlayerBullets = 10;
    private int currentPlayerBullets;

    //Player Boost Variables:
    public UnityEngine.UI.Image fuelBar = null;
    private float fuelBarSizeMuliplier = 175f;
    private const float bossBarSizeMax = 830f;
    private float bossBarSizeMulitplier = 725f;

    public float boostCooldownRatio = 0.1f;
    public float timePlayerCanBoost = 5f;
    private float playerBoostTimer;
    public float playerBoostSpeedMultiplier = 2f;

    //Player Lives Variables:
    public UnityEngine.UI.Image[] playerHearts;
    public const string spritePath = "DynamicSprites/heart_empty"; // Path inside Resources folder
    public const string spritePathFull = "DynamicSprites/heart_full"; // Path inside Resources folder
    public int maxPlayerLives = 3;
    private int currentPlayerLives = 3;

    public UnityEngine.UI.Image bossUIBar;
    public UnityEngine.UI.Image bossUIBarPercent;
    public TMPro.TextMeshProUGUI bossUIBarText;

    // AI Model
    public AIModelInterface aiModel;

    private Thread adjustDifficultyThread;
    private bool adjustmentInProgress = false;
    private Mutex adjustmentMutex = new Mutex();

    private bool updatedDifficulty = false;

    public AudioSource musicPlayer;
    
    public AudioSource playerSoundEffects_Gun;
    public AudioClip gunShotSound;
    public AudioClip gunEmptySound;
    public AudioSource playerSoundEffects_Boost;

    //Functions:
    private string[] getFileData(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string saveFileData = PlayerPrefs.GetString(key);
            return saveFileData.Split('\n');
        }
        return new string[0];
    }

    private void writeFileData(string key, string[] newFileData, bool overwriteExistingFileData)
    {
        string saveData = "";

        if (!overwriteExistingFileData && PlayerPrefs.HasKey(key))
        {
            saveData = PlayerPrefs.GetString(key) + "\n";
        }

        for (int i = 0; i < newFileData.Length; i++)
        {
            saveData += newFileData[i];
            if (i < newFileData.Length - 1)
            {
                saveData += "\n";
            }
        }

        PlayerPrefs.SetString(key, saveData);
        PlayerPrefs.Save();
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

    private void LoadGameData()
    {
        var gameState = SaveSystem.LoadGameState();
        if (gameState != null)
        {
            currentPlayerLives = (int)gameState["lives"];
            totalPoints = (float)gameState["points"];
            totalEnemiesKilled = (int)gameState["enemiesKilled"];
            currentDifficulty = (int)gameState["difficulty"];
            playerLifeTimer = (float)gameState["lifeTimer"];
            isTrainingMode = (bool)gameState["trainingMode"];
        }
        else
        {
            NewGameData();
        }
    }

    private void NewGameData()
    {
        SaveSystem.SaveGameState(maxPlayerLives, 0, 0, currentDifficulty, 0, isTrainingMode);
    }

    private void tryPlayerShoot()
    {
        if (currentBulletsInMagazine > 0)
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && currentPlayerBullets > 0)
            {
                playerSoundEffects_Gun.clip = gunShotSound;
                playerSoundEffects_Gun.Play();
                currentPlayerBullets--;
                currentBulletsInMagazine--;
                currentAlivePlayer.SendMessage("ShootBullet");
            }
        }
        else
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))){
                if (currentPlayerBullets <= 0){
                    playerSoundEffects_Gun.clip = gunEmptySound;
                    playerSoundEffects_Gun.Play();
                }else{
                    playerSoundEffects_Gun.clip = gunEmptySound;
                    playerSoundEffects_Gun.Play();
                }
            }

            if (bulletReloadTimer >= bulletReloadTime && currentPlayerBullets > 0)
            {
                bulletReloadTimer = 0;
                currentBulletsInMagazine = maxBulletsInMagazine < currentPlayerBullets ? maxBulletsInMagazine : currentPlayerBullets;
            }
            else
            {   
                bulletReloadTimer += Time.deltaTime;
            }
        }

        ammoUI.text = currentPlayerBullets + " / " + totalPlayerBullets;
    }

    private void tryBoostPlayer()
    {
        if (playerBoostTimer > 0 && Input.GetKey(KeyCode.LeftShift) && (currentAlivePlayer.GetComponent<Rigidbody2D>().linearVelocity.magnitude > 0))
        {   
            if(!playerSoundEffects_Boost.isPlaying){
                playerSoundEffects_Boost.Play();
            }
            playerBoostTimer -= Time.deltaTime;
            currentAlivePlayer.SendMessage("AffectBoostSpeed", playerBoostSpeedMultiplier);

        }
        else
        {   
            playerSoundEffects_Boost.Stop();
            currentAlivePlayer.SendMessage("AffectBoostSpeed", 1f);

            if (playerBoostTimer < timePlayerCanBoost)
            {
                playerBoostTimer += (Time.deltaTime * boostCooldownRatio);
            }
            else if (playerBoostTimer > timePlayerCanBoost)
            {
                playerBoostTimer = timePlayerCanBoost;
            }
        }

        //Update UI:
        float greenBarCutoff = 0.75f;
        float yellowBarCutoff = 0.25f;

        fuelBar.rectTransform.sizeDelta = new Vector2(fuelBarSizeMuliplier * playerBoostTimer, 15);
        if (playerBoostTimer >= greenBarCutoff * timePlayerCanBoost)
        {
            fuelBar.color = new Color(0f, 1f, 0f);
        }
        else if (playerBoostTimer >= yellowBarCutoff * timePlayerCanBoost)
        {
            fuelBar.color = new Color(1f, 1f, 0f);
        }
        else
        {
            fuelBar.color = new Color(1f, 0f, 0f);
        }
        fuelBar.rectTransform.anchoredPosition = new Vector2((fuelBarSizeMuliplier * 0.9f) * (playerBoostTimer - timePlayerCanBoost), -3);
    }

    public void updateBossHealhBar(int[] bossParameters)
    {
        int bossCurrentHealth = bossParameters[0];
        int bossMaxHealth = bossParameters[1];

        // Calculate health percentage (0.0 to 1.0)
        float healthPercentage = (float)bossCurrentHealth / bossMaxHealth;

        // Use a constant maximum width for the health bar regardless of difficulty
        float maxBarWidth = bossBarSizeMax;  // Fixed maximum width

        // Set the bar width based on the health percentage
        bossUIBarPercent.rectTransform.sizeDelta = new Vector2(maxBarWidth * healthPercentage, 30);

        // Fixed position calculation that works for all health values
        float positionX = (maxBarWidth * (1 - healthPercentage) * -0.9f);
        bossUIBarPercent.rectTransform.anchoredPosition = new Vector2(positionX, 30);

        Debug.Log($"Boss Health: {bossCurrentHealth}/{bossMaxHealth}, Percentage: {healthPercentage}, Width: {maxBarWidth * healthPercentage}");
        Debug.Log($"Boss Health Bar: Position: {positionX}, Size: {maxBarWidth * healthPercentage}");

        // Set colors and text
        bossUIBar.color = new Color(1f, 1f, 1f, 1f);
        bossUIBarPercent.color = new Color(0.5f, 0f, 0f, 1f);
        bossUIBarText.text = "Boss Health";
    }



    private void goNextLevel()
    {
        writeFileData(saveKey, new string[]{
            "" + currentPlayerLives, //currentPlayerLives
            "" + totalPoints, //totalPoints
            "" + totalEnemiesKilled, //totalEnemiesKilled
            "" + currentDifficulty,  //currentDifficulty
            "" + playerLifeTimer,  //playerLifeTimer
            "" + isTrainingMode //isTrainingMode
        }, true);
        if (isTrainingMode)
        {
            Debug.Log("Training mode active, creating survey...");

            if (surveyObject == null)
            {
                Debug.LogError("Cannot create survey: surveyObject is null!");
                // Fallback to loading next level directly
                SceneManager.LoadScene(currentLevelNumber + 1, LoadSceneMode.Single);
                return;
            }

            // Check if survey prefab has a SurveyScript component
            bool hasSurveyScript = surveyObject.GetComponent<MonoBehaviour>() != null &&
                                surveyObject.GetComponent<MonoBehaviour>().GetType().Name.Contains("SurveyScript");

            if (!hasSurveyScript)
            {
                Debug.LogWarning("Survey prefab doesn't have a SurveyScript component! Check that it's properly set up.");

                // Log all components on the survey prefab
                Component[] components = surveyObject.GetComponents<Component>();
                string[] componentNames = new string[components.Length];
                for (int i = 0; i < components.Length; i++)
                {
                    componentNames[i] = components[i].GetType().Name;
                }
                Debug.Log("Components on survey prefab: " + string.Join(", ", componentNames));
            }

            // Determine if survey is a UI element or a regular GameObject
            bool isUIElement = surveyObject.GetComponent<RectTransform>() != null;

            if (isUIElement)
            {
                // Create the survey as a child of the main canvas
                Canvas mainCanvas = FindObjectOfType<Canvas>();
                if (mainCanvas == null)
                {
                    Debug.LogError("No Canvas found in scene! Creating a new Canvas for the UI survey.");
                    mainCanvas = new GameObject("MainCanvas").AddComponent<Canvas>();
                    mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    mainCanvas.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
                    mainCanvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                }

                activeSurvey = Instantiate(surveyObject, mainCanvas.transform);

                // Set the survey's position to the center of the screen
                RectTransform surveyRect = activeSurvey.GetComponent<RectTransform>();
                if (surveyRect != null)
                {
                    surveyRect.anchoredPosition = Vector2.zero;
                    surveyRect.anchorMin = new Vector2(0.5f, 0.5f);
                    surveyRect.anchorMax = new Vector2(0.5f, 0.5f);
                    surveyRect.pivot = new Vector2(0.5f, 0.5f);
                    Debug.Log($"UI Survey positioned at center of screen: {surveyRect.anchoredPosition}");
                }
            }
            else
            {
                // For non-UI objects, instantiate in world space
                activeSurvey = Instantiate(surveyObject);

                // Position at center of the screen in world space
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    // Place it in front of the camera
                    Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 10);
                    Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenCenter);
                    activeSurvey.transform.position = worldPos;
                    Debug.Log($"Non-UI Survey positioned at world position: {worldPos}");
                }
                else
                {
                    // Fallback position if no camera is found
                    activeSurvey.transform.position = Vector3.zero;
                    Debug.LogWarning("No main camera found. Positioning survey at world origin.");
                }
            }

            Debug.Log($"Survey instantiated: {activeSurvey.name}, Active: {activeSurvey.activeInHierarchy}");

            activeSurvey.SendMessage("SetFileData", new string[]{
                "" + currentPlayerLives, //currentPlayerLives
                "" + totalPoints, //totalPoints
                "" + totalEnemiesKilled, //totalEnemiesKilled
                "" + currentDifficulty,  //currentDifficulty
                "" + playerLifeTimer,  //playerLifeTimer
                "" + currentLevelNumber //levelsBeat
            });

            activeSurvey.SendMessage("SetNextLevel", currentLevelNumber + 1);
            Debug.Log("Survey data and next level set successfully.");
        }
        else
        {
            SceneManager.LoadScene(currentLevelNumber + 1, LoadSceneMode.Single);

        }
    }

    private void onPlayerDeath()
    {
        currentPlayerLives -= 1;

        if (currentPlayerLives > 0)
        {
            updatedDifficulty = false;
            goRetryCurrentLevel();
        }
        else
        {
            goBackToCheckpointLevel();
        }
    }

    private void goRetryCurrentLevel()
    {
        string[] currentSaveData = getFileData(saveKey);

        writeFileData(saveKey, new string[]{
            "" + currentPlayerLives, //currentPlayerLives
            currentSaveData.Length > 1 ? currentSaveData[1] : "0", //totalPoints
            currentSaveData.Length > 2 ? currentSaveData[2] : "0", //totalEnemiesKilled
            currentDifficulty + "",  //currentDifficulty
            currentSaveData.Length > 4 ? currentSaveData[4] : "0",  //playerLifeTimer
            currentSaveData.Length > 5 ? currentSaveData[5] : "false" //isTrainingMode
        }, true);

        SceneManager.LoadScene(currentLevelNumber, LoadSceneMode.Single);
    }

    private void goBackToCheckpointLevel()
    {
        int goLevelNumber = (currentLevelNumber - (currentLevelNumber % numberLevelsCheckpoint)) + 1;

        if (isTrainingMode)
        {
            activeSurvey = (GameObject)Instantiate(surveyObject);
            activeSurvey.SendMessage("SetFileData", new string[]{
            "" + currentPlayerLives, //currentPlayerLives
            "" + totalPoints, //totalPoints
            "" + totalEnemiesKilled, //totalEnemiesKilled
            "" + currentDifficulty,  //currentDifficulty
            "" + playerLifeTimer,  //playerLifeTimer
            "" + (currentLevelNumber - 1) //levelsBeat
            });

            activeSurvey.SendMessage("SetNextLevel", goLevelNumber);
            NewGameData();
        }
        else
        {
            adjustGameDifficulty();
            NewGameData();
            SceneManager.LoadScene(goLevelNumber, LoadSceneMode.Single);
        }
    }

    void OnEnemyDeath(int enemyPointWorth)
    {
        totalEnemiesKilled += 1;
        currentLevelEnemyTotal -= 1;
        totalPoints += (enemyPointWorth * Mathf.Pow(difficultyMultiplier, currentDifficulty - 1));

        pointsUI.text = totalPoints + " pts";

        if (currentLevelEnemyTotal <= 0 && currentAlivePlayer)
        {
            wonLevel = true;
        }
    }

    private void findAllLevelObjects()
    {
        Collider2D[] allHitObjectsInScence = Physics2D.OverlapCircleAll(new Vector2(0, 0), Mathf.Infinity);
        List<GameObject> allEnemies = new List<GameObject>();

        foreach (Collider2D currentHitObject in allHitObjectsInScence)
        {
            string currentObjectName = currentHitObject.transform.gameObject.name;

            if (currentObjectName.ToLower().Contains(enemyName))
            {
                allEnemies.Add(currentHitObject.transform.gameObject);
                currentLevelEnemyTotal += 1;
            }
            else if (currentObjectName.ToLower().Contains(playerName))
            {
                currentAlivePlayer = currentHitObject.transform.gameObject;
            }
        }

        foreach (GameObject enemyObject in allEnemies)
        {
            enemyObject.SendMessageUpwards("SetGameObjects", new GameObject[] { gameObject, currentAlivePlayer });
            enemyObject.SendMessageUpwards("SetDifficultyLevel", currentDifficulty);
        }
    }

    private void adjustGameDifficulty()
    {
        aiModel.currentDifficulty = currentDifficulty;
        aiModel.currentPlayerLives = currentPlayerLives;
        //aiModel.levelsBeat = wonLevel ? currentLevelNumber : currentLevelNumber - 1;
        aiModel.levelsBeat = currentLevelNumber;
        aiModel.playerLifeTimer = playerLifeTimer;
        aiModel.totalEnemiesKilled = totalEnemiesKilled;
        aiModel.totalPoints = totalPoints;

        int predictedDifficulyChangeValue = aiModel.GetPredictedDifficulty();

        if (predictedDifficulyChangeValue == -101)
        {
            Debug.Log("Issue With Script");
        }
        else
        {
            currentDifficulty += predictedDifficulyChangeValue;
            Debug.Log($"New Difficulty level in Level Manager: {currentDifficulty}");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (surveyObject == null)
        {
            Debug.LogError("Survey object is not assigned in LevelManagerNew! The survey will not display at the end of levels.");
        }
        else
        {
            Debug.Log($"Survey prefab assigned: {surveyObject.name}");

            // Check if survey prefab has a SurveyScript component
            bool hasSurveyScript = surveyObject.GetComponent<MonoBehaviour>() != null &&
                                surveyObject.GetComponent<MonoBehaviour>().GetType().Name.Contains("SurveyScript");

            if (!hasSurveyScript)
            {
                Debug.LogError("Survey prefab doesn't have a SurveyScript component! It won't function properly.");

                // Log all components on the survey prefab
                Component[] components = surveyObject.GetComponents<Component>();
                string[] componentNames = new string[components.Length];
                for (int i = 0; i < components.Length; i++)
                {
                    componentNames[i] = components[i].GetType().Name;
                }
                Debug.Log("Components on survey prefab: " + string.Join(", ", componentNames));
            }

            // Check if it's a UI element
            bool isUIElement = surveyObject.GetComponent<RectTransform>() != null;
            if (isUIElement)
            {
                Debug.Log("Survey prefab is a UI element.");
                if (surveyObject.GetComponent<CanvasRenderer>() == null)
                {
                    Debug.LogWarning("Survey prefab is missing CanvasRenderer component which is typically needed for UI elements.");
                }
            }
            else
            {
                Debug.Log("Survey prefab is a non-UI GameObject. It will be positioned in world space.");
            }
        }

        fuelBarSizeMuliplier /= timePlayerCanBoost;
        currentLevelNumber = SceneManager.GetActiveScene().buildIndex;
        LoadGameData();
        aiModel = new AIModelInterface();

        currentPlayerBullets = (int)(totalPlayerBullets * Mathf.Pow(difficultyScale, currentDifficulty - 1));
        totalPlayerBullets = currentPlayerBullets;

        maxBulletsInMagazine = (int)(maxBulletsInMagazine * Mathf.Pow(difficultyScale, currentDifficulty - 1));
        currentBulletsInMagazine = maxBulletsInMagazine < currentPlayerBullets ? maxBulletsInMagazine : currentPlayerBullets;

        playerBoostTimer = timePlayerCanBoost;

        findAllLevelObjects();

        pointsUI.text = totalPoints + " pts";
        ammoUI.text = currentPlayerBullets + " / " + totalPlayerBullets;
        countdownUI.text = "";
        levelMessageUI.text = "";
        backgroundImage.color = new Color(0.16f, 0.42f, 0.56f, 0f);
        UpdateLivesUI();

        bossUIBar.color = new Color(1f, 1f, 1f, 0f);
        bossUIBarPercent.color = new Color(0.5f, 0f, 0f, 0f);
        bossUIBarText.text = "";

        //Show Level Start
        GameObject beforeLevelObject = (GameObject) Instantiate(checkpointMessageObject);
        beforeLevelObject.SendMessageUpwards("setPlayerLives", currentPlayerLives);
        beforeLevelObject.SendMessageUpwards("setLevelNumber", currentLevelNumber);
        beforeLevelObject.SendMessageUpwards("setIsCheckpoint", (currentLevelNumber > 1 && currentLevelNumber % numberLevelsCheckpoint == 1));
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // We already have a debug for this but going to keep it for Debugging the actual build
        levelDifficultyUI.text = "Current Difficulty: " + currentDifficulty + "";
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale != 0f)
        {
            Instantiate(pauseButton);
        }

        if (Time.timeScale != 0f)
        {
            if(!musicPlayer.isPlaying){
                musicPlayer.Play();
            }

            if (activeSurvey)
            {
                Debug.Log($"Survey is active: {activeSurvey.name}, visible: {activeSurvey.activeInHierarchy}");

                // Determine if survey is a UI element
                RectTransform rt = activeSurvey.GetComponent<RectTransform>();
                if (rt != null)
                {
                    Debug.Log($"UI Survey position: {rt.anchoredPosition}, Size: {rt.sizeDelta}, Scale: {rt.localScale}");
                    Debug.Log($"Survey anchors: min={rt.anchorMin}, max={rt.anchorMax}, pivot={rt.pivot}");
                }
                else
                {
                    // For non-UI surveys, log the world position
                    Debug.Log($"Non-UI Survey world position: {activeSurvey.transform.position}, rotation: {activeSurvey.transform.rotation}");
                }

                // Log all components on the active survey for debugging
                Component[] components = activeSurvey.GetComponents<Component>();
                string[] componentNames = new string[components.Length];
                for (int i = 0; i < components.Length; i++)
                {
                    componentNames[i] = components[i].GetType().Name;
                }
                Debug.Log("Components on active survey: " + string.Join(", ", componentNames));
            }
            if (!activeSurvey)
            {
                if (currentAlivePlayer)
                {
                    tryBoostPlayer();
                    tryPlayerShoot();

                    playerLifeTimer += Time.deltaTime;

                    if (wonLevel)
                    {

                        bossUIBar.color = new Color(1f, 1f, 1f, 0f);
                        bossUIBarPercent.color = new Color(0.5f, 0f, 0f, 0f);
                        bossUIBarText.text = "";
                        backgroundImage.color = new Color(0.16f, 0.42f, 0.56f, 1f);
                        countdownUI.text = Mathf.Round(playerCelebrateTime - currentWinTime) + "";
                        levelMessageUI.text = "You Won";
                        if (!adjustmentInProgress && !updatedDifficulty)
                        {
                            adjustmentInProgress = true;
                            updatedDifficulty = true;
                            adjustDifficultyThread = new Thread(() =>
                            {
                                try
                                {
                                    adjustmentMutex.WaitOne();
                                    adjustGameDifficulty();
                                }
                                finally
                                {
                                    adjustmentMutex.ReleaseMutex();
                                    adjustmentInProgress = false;
                                    updatedDifficulty = true;
                                }
                            });

                            if (!isTrainingMode)
                            {
                                adjustDifficultyThread.Start();
                            }
                        }

                        if (currentWinTime > playerCelebrateTime - 0.5 && currentWinTime < playerCelebrateTime)
                        {
                            levelMessageUI.text = "Updating Difficulty Level";
                            countdownUI.text = "";
                            currentWinTime += Time.deltaTime;
                        }
                        else if (currentWinTime > playerCelebrateTime)
                        {
                            levelMessageUI.text = ((currentLevelNumber + 1) % numberLevelsCheckpoint) == 1 ? "Saving Checkpoint" : "Loading Next Level";
                            countdownUI.text = "";
                            if (!isTrainingMode && adjustDifficultyThread != null)
                            {
                                // Wait for thread to complete if it's still running
                                if (adjustDifficultyThread.IsAlive)
                                {
                                    adjustDifficultyThread.Join();
                                }
                            }
                            updatedDifficulty = false;
                            goNextLevel();
                        }
                        else
                        {
                            currentWinTime += Time.deltaTime;
                        }
                    }
                }
                else
                {   
                    playerSoundEffects_Boost.Stop();
                    playerSoundEffects_Gun.Stop();
                    backgroundImage.color = new Color(0.16f, 0.42f, 0.56f, 1f);
                    countdownUI.text = Mathf.Round(playerRespawnTime - currentDeadTime) + "";
                    levelMessageUI.text = "You Lost";

                    if (currentPlayerLives - 1 == 0 && currentDeadTime > playerRespawnTime - 0.5 && currentDeadTime < playerRespawnTime)
                    {
                        levelMessageUI.text = "Updating Difficulty Level";
                        countdownUI.text = "";
                        currentDeadTime += Time.deltaTime;

                    }
                    else if (currentDeadTime > playerRespawnTime)
                    {
                        levelMessageUI.text = currentPlayerLives > 1 ? "Reloading Level" : "Loading Checkpoint";
                        countdownUI.text = "";
                        onPlayerDeath();
                    }
                    else
                    {
                        currentDeadTime += Time.deltaTime;
                    }
                }
            }
        }
    }
}
