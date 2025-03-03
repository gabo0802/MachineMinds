using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class LevelManagerNew : MonoBehaviour{
    public GameObject surveyObject;
    private GameObject activeSurvey;
    private string saveFilePath = Application.dataPath + "/Resources/GameState.save";
    private string aiTrainingFilePath = Application.dataPath + "/Resources/game_data.csv";
    private bool isTrainingMode = true;

    public int numberLevelsCheckpoint = 3;
    
    //Current Level Variables:  
        private const string playerName = "player";
        private const string enemyName = "enemy";

        public UnityEngine.UI.Image backgroundImage;
        public TMPro.TextMeshProUGUI countdownUI;
        public TMPro.TextMeshProUGUI levelMessageUI;

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
        public float bulletReloadRatio = 0.1f;
        private float currentBulletsInMagazine;
        public int totalPlayerBullets = 10;
        private int currentPlayerBullets;

    //Player Boost Variables:
        public UnityEngine.UI.Image fuelBar = null;
        private float fuelBarSizeMuliplier = 175f;
        private const float bossBarSizeMax = 775f;
        private float bossBarSizeMulitplier = 775f;

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

    //Functions:
    private string[] getFileData(string filePath){
        string saveFileData = "";

        using (StreamReader saveFile = File.OpenText(filePath)){
            string currentLine;
            
            while ((currentLine = saveFile.ReadLine()) != null){
                saveFileData += currentLine + "\n";
            }
         }
        
        return saveFileData.Split('\n');
    }

    private void writeFileData(string filePath, string[] newFileData, bool overwriteExistingFileData){
        if (overwriteExistingFileData){
            using (StreamWriter sw = File.CreateText(filePath)){
                for(int i = 0; i < newFileData.Length; i++){
                    sw.WriteLine(newFileData[i]);
                }
            }
        }else{
            using(TextWriter tw = new StreamWriter(filePath, true)){
                for(int i = 0; i < newFileData.Length; i++){
                    tw.WriteLine(newFileData[i]);
                }
            }
        }
    }

    private void UpdateLivesUI(){
        // Load New Sprites
        Texture2D emptyTexture = Resources.Load<Texture2D>(spritePath); 
        Texture2D fullTexture = Resources.Load<Texture2D>(spritePathFull); 

        // Check if textures loaded properly
        if (emptyTexture == null || fullTexture == null){
            Debug.LogError($"Failed to load textures! Check paths: {spritePath}, {spritePathFull}");
            return;
        }

        Sprite emptyHeart = Sprite.Create(emptyTexture, new Rect(0, 0, emptyTexture.width, emptyTexture.height), new Vector2(0.5f, 0.5f));
        Sprite fullHeart = Sprite.Create(fullTexture, new Rect(0, 0, fullTexture.width, fullTexture.height), new Vector2(0.5f, 0.5f));
        if (emptyHeart == null || fullHeart == null){
            Debug.LogError($"Failed to load sprites! Check paths: {spritePath}, {spritePathFull}");
            return;
        }
        else{
            Debug.Log("Successfully loaded heart sprites!");
        }


        for(int i = 0; i < currentPlayerLives; i++){
            playerHearts[i].sprite = fullHeart;
        }

        for(int i = currentPlayerLives; i < playerHearts.Length; i++){
            playerHearts[i].sprite = emptyHeart;
        }
    }

    private void NewGameData(){
        writeFileData(saveFilePath, new string[]{
            "" + maxPlayerLives, //currentPlayerLives
            "0", //totalPoints
            "0", //totalEnemiesKilled
            "" + currentDifficulty,  //currentDifficulty
            "0",  //playerLifeTimer
            "" + isTrainingMode //isTrainingMode
        }, true);
    }

    private void LoadGameData(){
        if (File.Exists(saveFilePath)){
            string[] fileDataArray = getFileData(saveFilePath);
            
            currentPlayerLives = System.Int32.Parse(fileDataArray[0]);
            totalPoints = System.Single.Parse(fileDataArray[1]);
            totalEnemiesKilled = System.Int32.Parse(fileDataArray[2]);
            currentDifficulty = System.Int32.Parse(fileDataArray[3]);
            playerLifeTimer = System.Single.Parse(fileDataArray[4]);
            isTrainingMode = bool.Parse(fileDataArray[5]);
            
        }else{
            NewGameData();
        }
    }

    private void tryPlayerShoot(){
        if((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && (int)currentBulletsInMagazine > 0 && currentPlayerBullets > 0){
            currentPlayerBullets--;
            currentBulletsInMagazine -= 1f;
            currentAlivePlayer.SendMessage("ShootBullet");
        }

        if(currentBulletsInMagazine < maxBulletsInMagazine && currentBulletsInMagazine < currentPlayerBullets){
            currentBulletsInMagazine += Time.deltaTime * bulletReloadRatio;
        }
        ammoUI.text = (int)currentBulletsInMagazine + " / " + currentPlayerBullets;
    }

    private void tryBoostPlayer(){
            if(playerBoostTimer > 0 && Input.GetKey(KeyCode.LeftShift)){
                playerBoostTimer -= Time.deltaTime;
                currentAlivePlayer.SendMessage("AffectBoostSpeed", playerBoostSpeedMultiplier);
            }else{
                currentAlivePlayer.SendMessage("AffectBoostSpeed", 1f);
                
                if(playerBoostTimer < timePlayerCanBoost){
                    playerBoostTimer += (Time.deltaTime * boostCooldownRatio);
                }else if (playerBoostTimer > timePlayerCanBoost){
                    playerBoostTimer = timePlayerCanBoost;
                }
            }

            //Update UI:
            float greenBarCutoff = 0.75f;
            float yellowBarCutoff = 0.25f;

            fuelBar.rectTransform.sizeDelta = new Vector2(fuelBarSizeMuliplier * playerBoostTimer, 15);
            if(playerBoostTimer >= greenBarCutoff * timePlayerCanBoost){
                fuelBar.color = new Color(0f, 1f, 0f);
            }else if (playerBoostTimer >= yellowBarCutoff * timePlayerCanBoost){
                fuelBar.color = new Color(1f, 1f, 0f);
            }else{
                fuelBar.color = new Color(1f, 0f, 0f);
            }
            fuelBar.rectTransform.anchoredPosition = new Vector2((fuelBarSizeMuliplier * 0.9f) * (playerBoostTimer - timePlayerCanBoost), -3);
    }
    
    public void updateBossHealhBar(int[] bossParameters){
        int bossCurrentHealth = bossParameters[0];
        int bossMaxHealth = bossParameters[1];
        
        if(bossBarSizeMulitplier > bossBarSizeMax / bossMaxHealth){
            bossBarSizeMulitplier /= bossMaxHealth;
        }

        bossUIBarPercent.rectTransform.sizeDelta = new Vector2(bossBarSizeMulitplier * bossCurrentHealth, 40);
        bossUIBarPercent.rectTransform.anchoredPosition = new Vector2((bossBarSizeMulitplier * 0.9f) * (bossCurrentHealth - bossMaxHealth), -3);

        bossUIBar.color = new Color(1f, 1f, 1f, 1f);
        bossUIBarPercent.color = new Color(0.5f, 0f, 0f, 1f);
        bossUIBarText.text = "Boss Health";
    }

    private void goNextLevel(){
        if(!isTrainingMode){
            adjustGameDifficulty();
        }

        writeFileData(saveFilePath, new string[]{
            "" + currentPlayerLives, //currentPlayerLives
            "" + totalPoints, //totalPoints
            "" + totalEnemiesKilled, //totalEnemiesKilled
            "" + currentDifficulty,  //currentDifficulty
            "" + playerLifeTimer,  //playerLifeTimer
            "" + isTrainingMode //isTrainingMode
        }, true);     
        
        if(isTrainingMode){
            activeSurvey = (GameObject) Instantiate(surveyObject);
            activeSurvey.SendMessage("SetFileData", new string[]{
            "" + currentPlayerLives, //currentPlayerLives
            "" + totalPoints, //totalPoints
            "" + totalEnemiesKilled, //totalEnemiesKilled
            "" + currentDifficulty,  //currentDifficulty
            "" + playerLifeTimer,  //playerLifeTimer
            "" + currentLevelNumber //levelsBeat
            });
            
            activeSurvey.SendMessage("SetNextLevel", currentLevelNumber + 1);
        }else{
            SceneManager.LoadScene(currentLevelNumber + 1, LoadSceneMode.Single);
        }
    }

    private void onPlayerDeath(){
        currentPlayerLives -= 1;

        if(currentPlayerLives > 0){
            goRetryCurrentLevel();
        }else{
            goBackToCheckpointLevel();
        }
    }

    private void goRetryCurrentLevel(){
        string[] currentSaveData = getFileData(saveFilePath);
        
        //adjustGameDifficulty();

        writeFileData(saveFilePath, new string[]{
            "" + currentPlayerLives, //currentPlayerLives
            currentSaveData[1], //totalPoints
            currentSaveData[2], //totalEnemiesKilled
            currentDifficulty + "",  //currentDifficulty
            currentSaveData[4],  //playerLifeTimer
            currentSaveData[5] //isTrainingMode
        }, true);        

        SceneManager.LoadScene(currentLevelNumber, LoadSceneMode.Single);
    }

    private void goBackToCheckpointLevel(){
        int goLevelNumber = currentLevelNumber < numberLevelsCheckpoint ? 1 : currentLevelNumber - (currentLevelNumber % numberLevelsCheckpoint);

        if(isTrainingMode){
            activeSurvey = (GameObject) Instantiate(surveyObject);
            activeSurvey.SendMessage("SetFileData", new string[]{
            "" + currentPlayerLives, //currentPlayerLives
            "" + totalPoints, //totalPoints
            "" + totalEnemiesKilled, //totalEnemiesKilled
            "" + currentDifficulty,  //currentDifficulty
            "" + playerLifeTimer,  //playerLifeTimer
            "" + (currentLevelNumber - 1) //levelsBeat
            });

            int nextLevel = currentLevelNumber < numberLevelsCheckpoint ? 1 : currentLevelNumber - (currentLevelNumber % numberLevelsCheckpoint);
            activeSurvey.SendMessage("SetNextLevel", goLevelNumber);
            NewGameData();
        }else{
            adjustGameDifficulty();
            NewGameData();
            SceneManager.LoadScene(goLevelNumber, LoadSceneMode.Single);  
        }
    }

    void OnEnemyDeath(int enemyPointWorth){
        totalEnemiesKilled += 1;
        currentLevelEnemyTotal -= 1;
        totalPoints += (enemyPointWorth * Mathf.Pow(difficultyMultiplier, currentDifficulty - 1));
        
        pointsUI.text = totalPoints + " pts";

        if(currentLevelEnemyTotal <= 0 && currentAlivePlayer){
           wonLevel = true;
        }
    }

    private void findAllLevelObjects(){
        Collider2D[] allHitObjectsInScence = Physics2D.OverlapCircleAll(new Vector2(0, 0), Mathf.Infinity);
        List<GameObject> allEnemies = new List<GameObject>();

        foreach(Collider2D currentHitObject in allHitObjectsInScence){
            string currentObjectName = currentHitObject.transform.gameObject.name;

            if(currentObjectName.ToLower().Contains(enemyName)){
                allEnemies.Add(currentHitObject.transform.gameObject);
                currentLevelEnemyTotal += 1;
            }else if(currentObjectName.ToLower().Contains(playerName)){
                currentAlivePlayer = currentHitObject.transform.gameObject;
            }
        }

        foreach(GameObject enemyObject in allEnemies){
            enemyObject.SendMessageUpwards("SetGameObjects", new GameObject[]{gameObject, currentAlivePlayer});
            enemyObject.SendMessageUpwards("SetDifficultyLevel", currentDifficulty);
        }
    }

    private void adjustGameDifficulty(){
        //call AI
        //currentDifficulty = getAIData();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        fuelBarSizeMuliplier /= timePlayerCanBoost;
        currentLevelNumber = SceneManager.GetActiveScene().buildIndex;
        LoadGameData();
        
        currentPlayerBullets = totalPlayerBullets * currentDifficulty; 
        currentBulletsInMagazine = maxBulletsInMagazine < currentPlayerBullets ? maxBulletsInMagazine : currentPlayerBullets;
        playerBoostTimer = timePlayerCanBoost;

        findAllLevelObjects();

        pointsUI.text = totalPoints + " pts"; 
        countdownUI.text = "";
        levelMessageUI.text = "";
        backgroundImage.color = new Color(0.16f, 0.42f, 0.56f, 0f);
        UpdateLivesUI();

        bossUIBar.color = new Color(1f, 1f, 1f, 0f);
        bossUIBarPercent.color = new Color(0.5f, 0f, 0f, 0f);
        bossUIBarText.text = "";
    }

    // Update is called once per frame
    void Update(){
        if(!activeSurvey){
            if(currentAlivePlayer){
                tryBoostPlayer();
                tryPlayerShoot();

                playerLifeTimer += Time.deltaTime;

                if(wonLevel){
                    backgroundImage.color = new Color(0.16f, 0.42f, 0.56f, 1f);
                    countdownUI.text = Mathf.Round(playerCelebrateTime - currentWinTime) + "";
                    levelMessageUI.text = "[You Won]";

                    if(currentWinTime > playerCelebrateTime){
                        levelMessageUI.text = "[Loading]";
                        countdownUI.text = "";
                        goNextLevel();
                    }else{
                        currentWinTime += Time.deltaTime;
                    }
                }
            }else{
                backgroundImage.color = new Color(0.16f, 0.42f, 0.56f, 1f);
                countdownUI.text = Mathf.Round(playerRespawnTime - currentDeadTime) + "";
                levelMessageUI.text = "[You Lost]";

                if(currentDeadTime > playerRespawnTime){
                    levelMessageUI.text = "[Loading]";
                    countdownUI.text = "";
                    onPlayerDeath();
                }else{
                    currentDeadTime += Time.deltaTime;
                }
             } 
        }
    }
}
